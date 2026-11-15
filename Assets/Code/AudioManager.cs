using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class AudioManager : MonoBehaviour
{
    [SerializeField] GameObject audioSlider;
    private Slider slider;
    public static AudioManager Instance;

    void Awake()
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

    void Start()
    {
        slider = audioSlider.GetComponent<Slider>();

        if (!PlayerPrefs.HasKey("audioVolume"))
        {
            PlayerPrefs.SetFloat("audioVolume", 1);
        }
        else
        {
            Load();
        }
    }

    public void ChangeVolume()
    {
        AudioListener.volume = slider.value;
    }

    private void Load()
    {
        slider.value = PlayerPrefs.GetFloat("audioVolume");
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("audioVolume", slider.value);
    }
}
