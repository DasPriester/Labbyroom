using System;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(SerializableDictionary<PickUpInteractable, int>))]
class SerializableDictionaryEditor : PropertyDrawer
{
    private PickUpInteractable[] _implementations;

    public static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);

    private static GUIContent
         moveButtonContent = new GUIContent("\u21b4", "move down"),
         //duplicateButtonContent = new GUIContent("+", "duplicate"),
         deleteButtonContent = new GUIContent("-", "delete");

    private static void ShowButtons(SerializedProperty keys, SerializedProperty values, int index)
    {
        if (GUILayout.Button(moveButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth))
        {
            keys.MoveArrayElement(index, index + 1);
            values.MoveArrayElement(index, index + 1);
        }
        if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight, miniButtonWidth))
        {
            keys.DeleteArrayElementAtIndex(index);
        }
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
        label = EditorGUI.BeginProperty(position, label, property);
		Rect contentPosition = EditorGUI.PrefixLabel(position, label);

        SerializedProperty keys = property.FindPropertyRelative("keys");
        SerializedProperty values = property.FindPropertyRelative("values");

        EditorGUI.indentLevel += 1;

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

        List<string> used = new List<string>();

        for (int x = 0; x < keys.arraySize; x++)
        {
            used.Add(keys.GetArrayElementAtIndex(x).objectReferenceValue.name);
        }

        Debug.Log(keys.arraySize);
        for (int i = 0; i < keys.arraySize; i++)
        {
            GUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(values.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(40f));

            EditorGUILayout.LabelField("x", GUILayout.Width(30f));

            string[] names = _implementations.Select(impl => impl.name).ToArray();
            string oname = keys.GetArrayElementAtIndex(i).objectReferenceValue.name;
            int _implementationTypeIndex = 0;

            for (int x = 0; x < names.Length; x++)
            {
                if (names[x] == oname)
                    _implementationTypeIndex = x;
            }

            int temp_implementationTypeIndex = EditorGUILayout.Popup(GUIContent.none,
                _implementationTypeIndex, names);

            string str = _implementations[temp_implementationTypeIndex].name;
            if (!used.Contains(str) || keys.GetArrayElementAtIndex(i).objectReferenceValue.name == str) {
                _implementationTypeIndex = temp_implementationTypeIndex;
                keys.GetArrayElementAtIndex(i).objectReferenceValue = _implementations[_implementationTypeIndex];
            }

            ShowButtons(keys, values, i);

            GUILayout.EndHorizontal();
        }

        EditorGUI.indentLevel -= 1;
        EditorGUI.EndProperty();
	}
}