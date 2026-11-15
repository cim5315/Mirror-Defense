using UnityEngine;
using UnityEngine.UI;

public class AudioSliderManager : MonoBehaviour
{
    [SerializeField] public Slider slider;
    [SerializeField] public GameObject audioManager;
    private GlobalAudioManager globalAudioManager;

    public void Start()
    {
        globalAudioManager = audioManager.GetComponent<GlobalAudioManager>();
        slider.value = globalAudioManager.CurrentVolume;
        AudioListener.volume = globalAudioManager.CurrentVolume;
    }

    public void ChangeVolume()
    {
        globalAudioManager.CurrentVolume = slider.value;
        AudioListener.volume = slider.value;
    }
}
