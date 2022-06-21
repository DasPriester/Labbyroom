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

        if (_implementations == null || GUILayout.Button("Refresh tasks"))
        {
            //this is probably the most imporant part:
            //find all implementations of Task using System.Reflection.Module
            _implementations = GetImplementations<Task>();
        }

        EditorGUILayout.LabelField($"Found {_implementations.Count()} tasks");

        //select implementation from editor popup
        _implementationTypeIndex = EditorGUILayout.Popup(new GUIContent("Implementation"),
            _implementationTypeIndex, _implementations.Select(impl => impl.FullName).ToArray());

        if (GUILayout.Button("Use task"))
        {
            //set new value
            quest.Task = (Task)Activator.CreateInstance(_implementations[_implementationTypeIndex]);
        }
    }

    private static Type[] GetImplementations<T>()
    {
        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());

        var interfaceType = typeof(T);
        return types.Where(p => interfaceType.IsAssignableFrom(p) && !p.IsAbstract).ToArray();
    }
}