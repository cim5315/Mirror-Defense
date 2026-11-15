using Unity.VisualScripting;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager main;

    [Header("References")]
    [SerializeField] private GameObject[] mirrorPrefabs;

    private int selectedMirror = 0;
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
        
    }
}
