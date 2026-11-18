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
    private bool prevBuildingPhase;
    private bool prevSelectMirrorMode;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        main = this;
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        ApplyPhaseState();
        prevBuildingPhase = isBuildingPhase;
        prevSelectMirrorMode = isSelectMirrorMode;
    }
    // Update is called once per frame
    void Update()
    {
        // Only proceed if at least one boolean changed since the last frame
        if (isBuildingPhase == prevBuildingPhase && isSelectMirrorMode == prevSelectMirrorMode)
        {
            return;
        }

        ApplyPhaseState();

        // Update the tracked states
        prevBuildingPhase = isBuildingPhase;
        prevSelectMirrorMode = isSelectMirrorMode;
    }
    private void ApplyPhaseState()
    {
        if (isBuildingPhase)
        {
            if (isSelectMirrorMode)
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
        isBuildingPhase = true;
        startRoundButton.SetActive(true);
        isSelectMirrorMode = true;
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
