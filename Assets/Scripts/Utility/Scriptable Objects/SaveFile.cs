using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Data", menuName = "Scriptable Object/SaveFile", order = 1)]
public class SaveFile : ScriptableObject
{
    public string name;
    public string data;

    private string mainScene = "Main";

    public void Load()
    {
        Debug.Log("loading: " + name);
        SceneLoader.loadedFile = this;
        SceneManager.LoadScene(mainScene);
    }
}