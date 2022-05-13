using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseGame : MonoBehaviour
{

    [SerializeField]
    Menu pauseMenu = null;
    private string startScene = "Start";

    private void Awake()
    {
        pauseMenu.OpenMenu = PauseTime;
        pauseMenu.CloseMenu = ResumeTime;
    }

    private void PauseTime() { Time.timeScale = 0.0f; }
    private void ResumeTime() { Time.timeScale = 1.0f; }

    public void LoadMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(startScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
