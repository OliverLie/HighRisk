using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonSceneManager : MonoBehaviour
{
    public Button Play;
    public Button Settings;
    public Button Quit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Play.onClick.AddListener(LoadGame);
        Settings.onClick.AddListener(LoadSettings);
        Quit.onClick.AddListener(QuitGame);
    }


    void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }

    void LoadSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    void QuitGame()
    {
        Application.Quit();
    }
    
    
    
    
}
