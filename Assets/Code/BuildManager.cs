using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.Tilemaps;

public class BuildManager : MonoBehaviour
{
    public static BuildManager main;

    [Header("Input")]
    [Tooltip("Assign the exact same A button InputActionReference used in HoldAToPause.")]
    [SerializeField] private InputActionReference placeAction;
    [Tooltip("Max seconds a press can last to count as a tap instead of a hold.")]
    [SerializeField] private float maxTapDuration = 0.35f;

    [Header("References")]
    [SerializeField] private GameObject[] mirrorPrefabs;
    [SerializeField] private GameObject tileSelector;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Tilemap targetTilemap;

    [Header("Prefabs")]
    [Tooltip("Prefab facing UP (0, 1) by default")]
    [SerializeField] private GameObject cardinalPrefab;
    [Tooltip("Prefab facing UP-RIGHT (1, 1) by default")]
    [SerializeField] private GameObject diagonalPrefab;

    [Header("Settings")]
    [SerializeField] private float deadzone = 0.2f;
    public VirtualMouseInput virtualMouseInput;
    public GameObject cursor;
    public GameObject rotationGuideActual;
    public float padding = 0;

    private Vector2 stickDirection;
    private Vector3Int currentCell;
    private Vector3Int previousCell = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
    private HashSet<Vector3Int> occupiedCells = new HashSet<Vector3Int>();

    private int selectedMirror = 0;
    private GameObject tileSelected;
    private GameObject rotationGuide;
    private GameObject previewObject;

    private int currentPreviewSlice = -1;
    private Vector3 lockedSpawnPosition;
    private Vector3Int lockedCellPosition;
    private Vector2 mouseScreenPos;
    private bool isCurrentlyPlacing = false;
    private float rotationOffset;

    public bool canPlace = true;
    public bool CanPlace { set { canPlace = value; } }

    private Coroutine cursorToggleCoroutine;

    private void Awake()
    {
        main = this;
    }
    private void Start()
    {
        // Keeps a normal hardware mouse confined to the game window in builds
        Cursor.lockState = CursorLockMode.Confined;
    }
    private void LateUpdate()
    {
        // Added '.added' check to ensure the virtual mouse is currently registered
        if (virtualMouseInput == null || virtualMouseInput.virtualMouse == null || !virtualMouseInput.virtualMouse.added)
            return;

        // 1. Read and clamp the virtual mouse position in screen pixels
        Vector2 pos = virtualMouseInput.virtualMouse.position.ReadValue();
        pos.x = Mathf.Clamp(pos.x, padding, Screen.width - padding);
        pos.y = Mathf.Clamp(pos.y, padding + 20f, Screen.height - padding);

        // 2. Write the clamped pixel position back to the Input System device
        InputState.Change(virtualMouseInput.virtualMouse.position, pos);

        // 3. Convert screen pixels to Canvas world coordinates for Screen Space - Camera
        RectTransform cursorRect = virtualMouseInput.cursorTransform;
        if (cursorRect != null && cursorRect.parent is RectTransform parentRect)
        {
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                parentRect,
                pos,
                mainCamera, // Must match the Render Camera assigned on your Canvas
                out Vector3 worldPos))
            {
                cursorRect.position = worldPos;
            }
        }
    }
    private void OnEnable()
    {
        if (placeAction != null && placeAction.action != null)
        {
            placeAction.action.Enable();
            placeAction.action.canceled += OnAButtonReleased;
        }
    }

    private void OnDisable()
    {
        if (placeAction != null && placeAction.action != null)
        {
            placeAction.action.canceled -= OnAButtonReleased;
        }
    }

    public GameObject GetSelectedMirror()
    {
        return mirrorPrefabs[selectedMirror];
    }

    void Update()
    {
        if (!canPlace || targetTilemap == null || Time.timeScale == 0f) return;

        if (isCurrentlyPlacing)
        {
            UpdatePlacementPreview();
            return;
        }

        UpdateCursorPosition();
        currentCell = GetCellUnderMouse();

        if (currentCell != previousCell)
        {
            if (tileSelected != null) Destroy(tileSelected);
            if (rotationGuide != null) Destroy(rotationGuide);

            previousCell = currentCell;
            Vector3 spawnPosition = targetTilemap.GetCellCenterWorld(currentCell);

            if (IsCellAvailable(currentCell))
            {
                tileSelected = Instantiate(tileSelector, spawnPosition, Quaternion.identity);
                rotationGuide = Instantiate(rotationGuideActual, spawnPosition, Quaternion.identity);
                rotationGuide.SetActive(false);
            }
        }
    }

    private void OnAButtonReleased(InputAction.CallbackContext context)
    {
        // Ignore if paused, disabled, or if the player was holding the button down
        if (!canPlace || Time.timeScale == 0f || context.duration > maxTapDuration)
            return;

        if (!isCurrentlyPlacing)
        {
            // 1st Press: Lock tile and move to rotation picking mode
            UpdateCursorPosition();
            Vector3Int cellPosition = GetCellUnderMouse();

            if (IsCellAvailable(cellPosition))
            {
                lockedCellPosition = cellPosition;
                lockedSpawnPosition = targetTilemap.GetCellCenterWorld(cellPosition);
                CurrentlyPlacing();
                ShowPreviewForSlice(2);
            }
        }
        else
        {
            // 2nd Press: Confirm rotation and spawn mirror
            int finalSlice = currentPreviewSlice != -1 ? currentPreviewSlice : 2;
            GetPrefabAndRotation(finalSlice, out GameObject prefabToSpawn, out Quaternion spawnRotation);

            Instantiate(prefabToSpawn, lockedSpawnPosition, spawnRotation);
            occupiedCells.Add(lockedCellPosition);
            NotCurrentlyPlacing();
            LevelManager.main.SelectMirror();
        }
    }

    // Call this from HoldAToPause.HeldButton() to cleanly exit placement mode when pausing
    public void CancelPlacement()
    {
        if (isCurrentlyPlacing)
        {
            NotCurrentlyPlacing();
        }
    }
    private void UpdateCursorPosition()
    {
        if (virtualMouseInput != null && virtualMouseInput.virtualMouse != null && virtualMouseInput.virtualMouse.added)
        {
            mouseScreenPos = virtualMouseInput.virtualMouse.position.ReadValue();
        }
        else if (Mouse.current != null)
        {
            mouseScreenPos = Mouse.current.position.ReadValue();
            mouseScreenPos.x = Mathf.Clamp(mouseScreenPos.x, padding, Screen.width - padding);
            mouseScreenPos.y = Mathf.Clamp(mouseScreenPos.y, padding + 20f, Screen.height - padding);
        }
    }

    private Vector3Int GetCellUnderMouse()
    {
        Vector3 screenPoint = new Vector3(mouseScreenPos.x, mouseScreenPos.y, -mainCamera.transform.position.z);
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPoint);
        worldPos.z = targetTilemap.transform.position.z;
        return targetTilemap.WorldToCell(worldPos);
    }

    private bool IsCellAvailable(Vector3Int cell)
    {
        return !targetTilemap.HasTile(cell) && !occupiedCells.Contains(cell);
    }

    private void UpdatePlacementPreview()
    {
        if (stickDirection.magnitude < deadzone) return;

        int slice = GetSliceFromDirection(stickDirection);
        if (slice != currentPreviewSlice)
        {
            ShowPreviewForSlice(slice);
        }
    }

    private void ShowPreviewForSlice(int slice)
    {
        currentPreviewSlice = slice;
        if (previewObject != null) Destroy(previewObject);

        GetPrefabAndRotation(slice, out GameObject prefab, out Quaternion rotation);
        previewObject = Instantiate(prefab, lockedSpawnPosition, rotation);

        foreach (Collider2D col in previewObject.GetComponentsInChildren<Collider2D>())
        {
            col.enabled = false;
        }
    }

    public void CurrentlyPlacing()
    {
        if (cursor != null)
        {
            if (cursorToggleCoroutine != null) StopCoroutine(cursorToggleCoroutine);
            cursorToggleCoroutine = StartCoroutine(SetCursorActiveNextFrame(false));
        }
        if (tileSelected != null) tileSelected.SetActive(false);

        if (rotationGuide == null && rotationGuideActual != null)
        {
            rotationGuide = Instantiate(rotationGuideActual, lockedSpawnPosition, Quaternion.identity);
        }
        if (rotationGuide != null) rotationGuide.SetActive(true);

        currentPreviewSlice = -1;
        isCurrentlyPlacing = true;
    }

    public void NotCurrentlyPlacing()
    {
        if (cursor != null)
        {
            if (cursorToggleCoroutine != null) StopCoroutine(cursorToggleCoroutine);
            cursor.SetActive(true);
        }
        if (rotationGuide != null) rotationGuide.SetActive(false);

        // Destroy the old selector so it doesn't linger on an occupied tile
        if (tileSelected != null) Destroy(tileSelected);

        // Reset previousCell so Update() re-checks cell availability on the very next frame
        previousCell = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);

        if (previewObject != null) Destroy(previewObject);

        currentPreviewSlice = -1;
        isCurrentlyPlacing = false;
    }

    private System.Collections.IEnumerator SetCursorActiveNextFrame(bool isActive)
    {
        yield return null;
        if (cursor != null) cursor.SetActive(isActive);
        cursorToggleCoroutine = null;
    }

    public void Direction(InputAction.CallbackContext context)
    {
        stickDirection = context.ReadValue<Vector2>();
    }

    private int GetSliceFromDirection(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;
        return Mathf.RoundToInt(angle / 45f) % 8;
    }

    private void GetPrefabAndRotation(int slice, out GameObject prefab, out Quaternion rotation)
    {
        float snappedAngle = slice * 45f;
        bool isDiagonal = (slice % 2 != 0);

        if (isDiagonal)
        {
            prefab = diagonalPrefab;
            rotationOffset = snappedAngle - 135f;
        }
        else
        {
            prefab = cardinalPrefab;
            rotationOffset = snappedAngle + 180f;
        }

        rotation = Quaternion.Euler(0f, 0f, rotationOffset);
    }
    public void SelectedBasicMirror()
    {
        if (Currency.mainCurrency.HaveEnoughShards(20))
        {
            selectedMirror = 0;
            LevelManager.main.PlaceMirror();

        }
    }
}
