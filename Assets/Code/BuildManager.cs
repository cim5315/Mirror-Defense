using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.Tilemaps;

public class BuildManager : MonoBehaviour
{
    public static BuildManager main;

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

    private void Awake()
    {
        main = this;
    }

    public GameObject GetSelectedMirror()
    {
        return mirrorPrefabs[selectedMirror];
    }

    void Update()
    {
        if (!canPlace || targetTilemap == null) return;

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

    private void UpdateCursorPosition()
    {
        if (virtualMouseInput != null && virtualMouseInput.virtualMouse != null)
        {
            mouseScreenPos = virtualMouseInput.virtualMouse.position.ReadValue();
            mouseScreenPos.x = Mathf.Clamp(mouseScreenPos.x, padding, Screen.width - padding);
            mouseScreenPos.y = Mathf.Clamp(mouseScreenPos.y, padding + 20, Screen.height - padding);
            InputState.Change(virtualMouseInput.virtualMouse.position, mouseScreenPos);
        }
        else if (Mouse.current != null)
        {
            mouseScreenPos = Mouse.current.position.ReadValue();
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

    public void Click(InputAction.CallbackContext context)
    {
        if (!canPlace || !context.performed || isCurrentlyPlacing) return;

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

    public void Spawn(InputAction.CallbackContext context)
    {
        // Only spawn when the button is released (canceled)
        if (!canPlace || !context.canceled || !isCurrentlyPlacing) return;

        if (context.duration <= 1.0)
        {
            int finalSlice = currentPreviewSlice != -1 ? currentPreviewSlice : 2;
            GetPrefabAndRotation(finalSlice, out GameObject prefabToSpawn, out Quaternion spawnRotation);

            Instantiate(prefabToSpawn, lockedSpawnPosition, spawnRotation);
            occupiedCells.Add(lockedCellPosition);
        }

        NotCurrentlyPlacing();
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
        if (cursor != null) cursor.SetActive(false);
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
        if (cursor != null) cursor.SetActive(true);
        if (rotationGuide != null) rotationGuide.SetActive(false);
        if (tileSelected != null) tileSelected.SetActive(true);
        if (previewObject != null) Destroy(previewObject);

        currentPreviewSlice = -1;
        isCurrentlyPlacing = false;
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
}