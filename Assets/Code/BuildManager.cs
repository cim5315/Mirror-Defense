using UnityEngine;
using UnityEngine.InputSystem;
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

    private Vector2 stickDirection;
    private Vector3Int previousCell = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
    private int selectedMirror = 0;
    private GameObject tileSelected;
    private GameObject rotationGuide;
    private GameObject previewObject;
    private int currentPreviewSlice = -1;
    private Vector3 lockedSpawnPosition;
    private Vector2 mouseScreenPos;
    private bool isCurrentlyPlacing = false;

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
        if (targetTilemap == null) return;

        if (isCurrentlyPlacing)
        {
            UpdatePlacementPreview();
            return;
        }

        if (virtualMouseInput != null)
        {
            mouseScreenPos = virtualMouseInput.virtualMouse.position.ReadValue();
        }
        else
        {
            if (Mouse.current == null) return;
            mouseScreenPos = Mouse.current.position.ReadValue();
        }

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, -mainCamera.transform.position.z));
        worldPos.z = targetTilemap.transform.position.z;

        Vector3Int currentCell = targetTilemap.WorldToCell(worldPos);

        if (currentCell != previousCell)
        {
            if (tileSelected != null) Destroy(tileSelected);
            if (rotationGuide != null) Destroy(rotationGuide);

            previousCell = currentCell;
            Vector3 spawnPosition = targetTilemap.GetCellCenterWorld(currentCell);

            if (!targetTilemap.HasTile(currentCell))
            {
                tileSelected = Instantiate(tileSelector, spawnPosition, Quaternion.identity);
                rotationGuide = Instantiate(rotationGuideActual, spawnPosition, Quaternion.identity);
                NotCurrentlyPlacing();
            }
        }
    }

    public void Click(InputAction.CallbackContext context)
    {
        if (context.performed && !isCurrentlyPlacing)
        {
            if (virtualMouseInput != null)
            {
                mouseScreenPos = virtualMouseInput.virtualMouse.position.ReadValue();
            }

            Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
            Vector3Int cellPosition = targetTilemap.WorldToCell(mouseWorldPos);
            lockedSpawnPosition = targetTilemap.GetCellCenterWorld(cellPosition);

            if (!targetTilemap.HasTile(cellPosition))
            {
                CurrentlyPlacing();
                ShowPreviewForSlice(2);
            }
        }
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

        // Disable colliders so the preview doesn't block lasers or gameplay before placement
        foreach (Collider2D col in previewObject.GetComponentsInChildren<Collider2D>())
        {
            col.enabled = false;
        }
    }

    public void CurrentlyPlacing()
    {
        if (cursor != null) cursor.SetActive(false);
        if (rotationGuide != null) rotationGuide.SetActive(true);
        if (tileSelected != null) tileSelected.SetActive(false);
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

    public void Spawn(InputAction.CallbackContext context)
    {
        if (context.performed && isCurrentlyPlacing)
        {
            int finalSlice = currentPreviewSlice != -1 ? currentPreviewSlice : 2;
            GetPrefabAndRotation(finalSlice, out GameObject prefabToSpawn, out Quaternion spawnRotation);

            Instantiate(prefabToSpawn, lockedSpawnPosition, spawnRotation);
            NotCurrentlyPlacing();
        }
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

        prefab = isDiagonal ? diagonalPrefab : cardinalPrefab;

        // Added + 180f to flip the cardinal prefab to the exact opposite direction
        float rotationOffset = isDiagonal ? snappedAngle - 135f : snappedAngle + 180f;

        rotation = Quaternion.Euler(0f, 0f, rotationOffset);
    }
}