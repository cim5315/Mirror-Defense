using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsButtonManager : MonoBehaviour
{
    public string mainMenu;
    public void Back()
    {
        Debug.Log("go back");
        SceneManager.LoadScene(mainMenu);
    }
}
