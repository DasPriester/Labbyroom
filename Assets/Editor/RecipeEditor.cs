using System;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Recipe))]
class RecipeInspector : Editor
{
    private PickUpInteractable[] _implementations;
    private int _implementationTypeIndex;

    int numberOfItems = 1;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Recipe recipe = target as Recipe;
        //specify type
        if (recipe == null)
        {
            return;
        }

        GUILayout.BeginHorizontal();

        if (_implementations == null)
        {
            _implementations = Resources.LoadAll<PickUpInteractable>("Prefabs");
        }

        EditorGUILayout.LabelField($"Found {_implementations.Count()} items");

        if (GUILayout.Button("Refresh items"))
        {
            _implementations = Resources.LoadAll<PickUpInteractable>("Prefabs");
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        //select implementation from editor popup
        _implementationTypeIndex = EditorGUILayout.Popup(new GUIContent("Item"),
            _implementationTypeIndex, _implementations.Select(impl => impl.name).ToArray());

        EditorGUILayout.IntField(numberOfItems, new GUILayoutOption[] { });

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add to Cost"))
        {
            //set new value
            recipe.Cost.Add(_implementations[_implementationTypeIndex], numberOfItems);
        }

        if (GUILayout.Button("Add to Yield"))
        {
            //set new value
            recipe.Yield.Add(_implementations[_implementationTypeIndex], numberOfItems);
        }

        GUILayout.EndHorizontal();
    }
}