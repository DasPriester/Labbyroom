using UnityEngine;

/// <summary>
/// Scriptable object to represent a Task (used in Quests)
/// </summary>
public abstract class Task : ScriptableObject
{
    public abstract int Done();
}