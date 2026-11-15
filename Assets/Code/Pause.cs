using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenu;
    public void PauseTime()
    {
        Time.timeScale = 0f;
    }
    public void UnPauseTime()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);

    }
}
