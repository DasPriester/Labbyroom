using UnityEngine;
using System;

/// <summary>
/// Scriptable object to represent a Task (used in Quests)
/// </summary>
[Serializable]
public abstract class Task
{
    public abstract float Done();
}