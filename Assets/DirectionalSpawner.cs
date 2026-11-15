using UnityEngine;
using UnityEngine.InputSystem;

public class DirectionalSpawner : MonoBehaviour
{
    [Header("Prefabs to Spawn")]
    [SerializeField] private GameObject upPrefab;
    [SerializeField] private GameObject downPrefab;
    [SerializeField] private GameObject leftPrefab;
    [SerializeField] private GameObject rightPrefab;
    [SerializeField] private GameObject neutralPrefab;

    [Header("Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float deadzone = 0.2f;

    private Vector2 stickDirection;

    // Automatically called by Player Input when the stick moves
    private void OnDirection(InputValue value)
    {
        stickDirection = value.Get<Vector2>();
    }

    // Automatically called by Player Input when the Spawn button is pressed
    private void OnSpawn(InputValue value)
    {
        if (value.isPressed)
        {
            GameObject prefabToSpawn = GetPrefabForDirection(stickDirection);
            if (prefabToSpawn != null)
            {
                Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);
            }
        }
    }

    private GameObject GetPrefabForDirection(Vector2 dir)
    {
        // Return neutral prefab if stick is barely tilted
        if (dir.magnitude < deadzone)
        {
            return neutralPrefab;
        }

        // Check whether horizontal or vertical tilt is stronger
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            return dir.x > 0 ? rightPrefab : leftPrefab;
        }
        else
        {
            return dir.y > 0 ? upPrefab : downPrefab;
        }
    }
}
