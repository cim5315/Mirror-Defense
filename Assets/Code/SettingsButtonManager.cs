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
}
