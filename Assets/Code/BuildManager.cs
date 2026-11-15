using System;
using Unity.VisualScripting;
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
    private Vector3Int previousCell = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
    private int selectedMirror = 0;
    private GameObject tileSelected;
    public VirtualMouseInput virtualMouseInput;
    private Vector2 mouseScreenPos;
    private bool isCurrentlyPlacing = false;
    public GameObject cursor;
    private GameObject rotationGuide;
    public GameObject rotationGuideActual;
    private void Awake()
    {
        main = this;
    }
    public GameObject GetSelectedMirror()
    {
        return mirrorPrefabs[selectedMirror];
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (virtualMouseInput != null)
        {
            mouseScreenPos = virtualMouseInput.virtualMouse.position.ReadValue();
            Debug.Log("works");
        }
        else
        {
            mouseScreenPos = Mouse.current.position.ReadValue();
        }
        if (Mouse.current == null || targetTilemap == null) return;


        // 2. Convert to world position (adjust Z distance for perspective cameras)
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, -Camera.main.transform.position.z));
        worldPos.z = targetTilemap.transform.position.z;

        // 3. Convert world position to Tilemap grid cell position
        Vector3Int currentCell = targetTilemap.WorldToCell(worldPos);

        // 4. Only trigger when moving to a new cell
        if (currentCell != previousCell)
        {
            Destroy(tileSelected);
            previousCell = currentCell;

            // Convert mouse screen position to world position
            Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

            Vector3Int cellPosition = targetTilemap.WorldToCell(mouseWorldPos);

            Vector3 spawnPosition = targetTilemap.GetCellCenterWorld(cellPosition);

            if (!targetTilemap.HasTile(currentCell) && !isCurrentlyPlacing)
            {
                TileBase hoveredTile = targetTilemap.GetTile(currentCell);
                tileSelected = Instantiate(tileSelector, spawnPosition, Quaternion.identity);
                rotationGuide = Instantiate(rotationGuideActual, spawnPosition, Quaternion.identity);
                NotCurrentlyPlacing();

            }
        }
    }
    public void Click(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (virtualMouseInput != null)
            {
                mouseScreenPos = virtualMouseInput.virtualMouse.position.ReadValue();
            }
            // Convert mouse screen position to world position
            Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

            // Convert world position to tilemap cell position
            Vector3Int cellPosition = targetTilemap.WorldToCell(mouseWorldPos);
            
            Vector3 spawnPosition = targetTilemap.GetCellCenterWorld(cellPosition);
            
            if (!targetTilemap.HasTile(cellPosition) && !isCurrentlyPlacing)
            {
                Instantiate(GetSelectedMirror(), spawnPosition, Quaternion.identity);
                CurrentlyPlacing();
            }
        }
    }
    public void CurrentlyPlacing()
    {
        cursor.SetActive(false);
        rotationGuide.SetActive(true);
        tileSelected.SetActive(false);
    }
    public void NotCurrentlyPlacing()
    {
        cursor.SetActive(true);
        rotationGuide.SetActive(false);
        tileSelected.SetActive(true);
    }
}
