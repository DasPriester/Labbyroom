using UnityEngine;
using System;

/// <summary>
/// Scriptable object to represent a Quest
/// </summary>
[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Object/Quest", order = 1)]
[Serializable]
public class Quest : ScriptableObject
{
    [SerializeField] private string name = default;
    [TextArea(3, 10)]
    [SerializeField] private string description = "";

    [SerializeReference] private Task task;
    [SerializeField] private float goal = 1;
    [SerializeField] private bool maximise = true;

    Quest(string pName, string pDescription, Task pTask, float pGoal = 1f, bool pMaximise = true)
    {
        name = pName;
        description = pDescription;
        task = pTask;
        goal = pGoal;
        maximise = pMaximise;
    }

    public Task Task { get => task; set => task = value; }

    public bool IsDone()
    {
        if (maximise)
            return task.Done() >= goal;
        else
            return task.Done() <= goal;
    }
}