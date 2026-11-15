using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsButtonManager : MonoBehaviour
{
    public string mainMenu;

    public void GoMenu()
    {
        Debug.Log("load menu");
        SceneManager.LoadScene(mainMenu);
    }

    public void Volume()
    {
        Debug.Log("Pretend this changes the volume of something");
    }
}
