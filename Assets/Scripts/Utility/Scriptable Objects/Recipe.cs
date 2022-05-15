using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Scriptable Object/Recipe", order = 1)]
public class Recipe : ScriptableObject
{
    public string name;
    public bool unlocked = false;

    public SerializableDictionary<PickUpInteractable, int> Cost;
    public SerializableDictionary<PickUpInteractable, int> Yield;

    private void OnEnable()
    {
        unlocked = false;
    }
}