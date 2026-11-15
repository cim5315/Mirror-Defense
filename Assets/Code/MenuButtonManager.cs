using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuButtonManager : MonoBehaviour
{
    public string LevelName;
    public string Settings;

    public void PlayGame()
    {
        Debug.Log("load level");
        //SceneManager.LoadScene(LevelName);
    }

    public void GoSettings()
    {
        Debug.Log("load settings");
        //SceneManager.LoadScene(Settings);
    }

    public void QuitGame()
    {
        Debug.Log("quit");
        Application.Quit();
    }
}
