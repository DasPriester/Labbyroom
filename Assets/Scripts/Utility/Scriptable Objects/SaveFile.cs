using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Scriptable Object/SaveFile", order = 1)]
public class SaveFile : ScriptableObject
{
    public string name;

    public void Load()
    {
        Debug.Log("loading: " + name);
    }
}