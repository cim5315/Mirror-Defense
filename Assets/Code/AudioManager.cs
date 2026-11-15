using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] GameObject audioSlider;

    private Slider slider;

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
