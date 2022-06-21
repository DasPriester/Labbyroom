using UnityEngine;

/// <summary>
/// Scriptable object to represent a Quest
/// </summary>
[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Object/Quest", order = 1)]
public class Quest : ScriptableObject
{
    [SerializeField] private string name = default;
    [TextArea(3, 10)]
    [SerializeField] private string description = "";

    [SerializeField] private Task task;
    [SerializeField] private int goal = 1;

    Quest(string pName, string pDescription, Task pTask, int pGoal = 1)
    {
        name = pName;
        description = pDescription;
        task = pTask;
        goal = pGoal;
    }

    public bool IsDone()
    {
        return task.Done() >= goal;
    }
}