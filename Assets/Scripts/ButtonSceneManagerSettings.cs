using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonSceneManagerSettings : MonoBehaviour
{

    public Button Back;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Back.onClick.AddListener(BackButton);
    }

    // Update is called once per frame
    void BackButton()
    {
        SceneManager.LoadScene("Menu");
    }
}
