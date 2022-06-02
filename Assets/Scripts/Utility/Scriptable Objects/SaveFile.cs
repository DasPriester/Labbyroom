using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Scriptable object to save a save file
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "Scriptable Object/SaveFile", order = 1)]
public class SaveFile : ScriptableObject
{
    new public string name;
    public string data;

    private readonly string mainScene = "Main";

    public void Load()
    {
        Debug.Log("loading: " + name);
        SceneLoader.loadedFile = this;
        SceneManager.LoadScene(mainScene);
    }
}