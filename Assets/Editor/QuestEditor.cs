using System;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Quest))]
class QuestInspector : Editor
{
    private Type[] _implementations;
    private int _implementationTypeIndex;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Quest quest = target as Quest;
        //specify type
        if (quest == null)
        {
            return;
        }

        GUILayout.BeginHorizontal();

        if (_implementations == null)
        {
            _implementations = Utility.GetImplementations<Task>();
        }

        EditorGUILayout.LabelField($"Found {_implementations.Count()} tasks");

        if (GUILayout.Button("Refresh tasks"))
        {
            _implementations = Utility.GetImplementations<Task>();
        }

        GUILayout.EndHorizontal();

        //select implementation from editor popup
        _implementationTypeIndex = EditorGUILayout.Popup(new GUIContent("Tasks"),
            _implementationTypeIndex, _implementations.Select(impl => impl.FullName).ToArray());

        if (GUILayout.Button("Use task"))
        {
            //set new value
            quest.Task = (Task)Activator.CreateInstance(_implementations[_implementationTypeIndex]);
        }
    }
}