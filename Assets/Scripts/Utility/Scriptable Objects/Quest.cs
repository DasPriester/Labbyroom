using UnityEngine;
using System;

/// <summary>
/// Scriptable object to represent a Quest
/// </summary>
[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Object/Quest", order = 1)]
public class Quest : ScriptableObject
{
    [TextArea(3, 10)]
    [SerializeField] private string description = "";
    [SerializeReference] private Task task;
    [SerializeField] private Item reward;
    [SerializeField] private float goal = 1;
    [SerializeField] private bool maximise = true;

    public delegate void Action();
    public Action StartUI;
    public Action CloseUI;
    public Action Rewards;

    public Item Reward { get { return reward; } }
    public string Description { get { return description; } }
    public Task Task { get => task; set => task = value; }

    public bool IsDone()
    {
        if (maximise)
            return task.Done() >= goal;
        else
            return task.Done() <= goal;
    }
}