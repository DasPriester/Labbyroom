using UnityEngine;

/// <summary>
/// Scriptable object to save cost and yield for each recipe
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "Scriptable Object/Recipe", order = 1)]
public class Recipe : ScriptableObject
{
    new public string name;
    public bool unlocked = false;
    public bool alwaysUnlocked = false;
    public bool requiresForge = false;

    public SerializableDictionary<PickUpInteractable, int> Cost;
    public SerializableDictionary<PickUpInteractable, int> Yield;

    private void OnEnable()
    {
        unlocked = false;
    }
}