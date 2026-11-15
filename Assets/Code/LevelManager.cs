using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;
    public Transform startPoint;
    public Transform[] path;
    public bool isBuildingPhase = true;
    public bool isSelectMirrorMode = true;
    public bool isPlaceMirrorMode = false;
    public GameObject startRoundButton;
    
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
        if (isBuildingPhase == true)
        {
            if (isSelectMirrorMode == true)
            {
                UISlideIn.uISlideIn.SlideIn();
                BuildManager.main.CanPlace = false;
            }
            else
            {
                UISlideIn.uISlideIn.SlideOut();
                BuildManager.main.CanPlace = true;
            }
        }
        else
        {
            UISlideIn.uISlideIn.SlideOut();
            BuildManager.main.CanPlace = false;
        }
    }
    public void StartRound()
    {
        isBuildingPhase = false;
        startRoundButton.SetActive(false);
        EnemySpawner.main.StartNewWave();
    }
    public void EndRound()
    {
        isBuildingPhase = false;
        startRoundButton.SetActive(true);
    }
    public void PlaceMirror()
    {
        isSelectMirrorMode = false;
        isPlaceMirrorMode = true;
    }
    public void SelectMirror()
    {
        isSelectMirrorMode = true;
        isPlaceMirrorMode = false;
    }
}
