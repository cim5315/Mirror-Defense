using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuButtonManager : MonoBehaviour
{
    public string levelName;
    public string settings;

    public void PlayGame()
    {
        Debug.Log("load level");
        SceneManager.LoadScene(levelName);
    }

    public void GoSettings()
    {
        Debug.Log("load settings");
        SceneManager.LoadScene(settings);
    }

    public void QuitGame()
    {
        Debug.Log("quit");
        Application.Quit();
    }
}
