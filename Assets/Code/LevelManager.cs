using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;
    public Transform startPoint;
    public Transform[] path;
    public bool isBuildingPhase = true;
    public bool isSelectMirrorMode = true;
    public bool isPlaceMirrorMode = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        main = this;
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (isBuildingPhase)
        {
            if (isSelectMirrorMode)
            {
                UISlideIn.uISlideIn.SlideIn();
                BuildManager.main.canPlace = true;
            }
            else
            {
                UISlideIn.uISlideIn.SlideOut();
                BuildManager.main.canPlace = false;
            }
        }
    }
}
