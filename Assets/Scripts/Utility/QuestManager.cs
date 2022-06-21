using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private SerializableDictionary<Quest, bool> quests = new SerializableDictionary<Quest, bool>();

    [SerializeField] private UnityEvent OnQuestAdded;
    [SerializeField] private UnityEvent OnQuestRemoved;
    [SerializeField] private UnityEvent OnQuestCompleted;

    public Dictionary<Quest, bool> Quests { get => quests; }

    public void AddQuest(Quest quest)
    {
        quests.Add(quest, false);
        OnQuestAdded.Invoke();
    }

    public void AddQuest(List<Quest> quest)
    {
        foreach (Quest q in quest)
        {
            quests.Add(q, false);
        }
        OnQuestAdded.Invoke();
    }

    public void AddQuest(Dictionary<Quest, bool> quest)
    {
        foreach (KeyValuePair<Quest, bool> q in quest)
        {
            quests.Add(q.Key, q.Value);
        }
        OnQuestAdded.Invoke();
    }

    public bool RemoveQuest(Quest quest)
    {
        if (quests.ContainsKey(quest))
        {
            quests.Remove(quest);
            OnQuestRemoved.Invoke();
            return true;
        } else
        {
            return false;
        }
    }

    public bool RemoveQuest(List<Quest> quest)
    {
        bool s = false;
        foreach (Quest q in quest)
        {
            if (quests.ContainsKey(q))
            {
                quests.Remove(q);
                s = true;
            }
        }
        if (s)
            OnQuestRemoved.Invoke();
        return s;
    }

    public bool RemoveQuest(string name)
    {
        foreach (Quest q in quests.Keys)
        {
            if (q.name == name)
            {
                quests.Remove(q);
                OnQuestRemoved.Invoke();
                return true;
            }
        }
        return false;
    }

    public bool RemoveQuest(List<string> names)
    {
        bool s = false;
        foreach (Quest q in quests.Keys)
        {
            if (names.Contains(q.name))
            {
                quests.Remove(q);
                s = true;
            }
        }
        if (s)
            OnQuestRemoved.Invoke();
        return s;
    }

    private void Update()
    {
        List<Quest> keys = new List<Quest>();

        foreach (Quest q in quests.Keys)
        {
            keys.Add(q);
        }

        foreach (Quest key in keys)
        {
            if (!quests[key] && key.IsDone())
            {
                OnQuestCompleted.Invoke();
                quests[key] = true;
            }
        }
    }
}
