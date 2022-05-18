using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private string mainScene = "Main";

    public void PlayGame()
    {
        SceneManager.LoadScene(mainScene);
    }

    public void NewGame()
    {
        SceneLoader.loadedFile = null;
        SceneManager.LoadScene(mainScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
