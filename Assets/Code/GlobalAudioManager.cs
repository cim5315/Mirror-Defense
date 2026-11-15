using Unity.VisualScripting;
using UnityEngine;

public class GlobalAudioManager : MonoBehaviour
{
    public static GlobalAudioManager Instance;

    private float curVol;

    public float CurrentVolume
    {
        get { return curVol; }
        set
        {
            curVol = value;
        }
    }
    public void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}
