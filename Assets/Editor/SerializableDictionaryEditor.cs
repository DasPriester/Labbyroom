using System;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

abstract class SerializableDictionaryEditor : PropertyDrawer
{
    protected UnityEngine.Object[] _implementations_key;
    protected UnityEngine.Object[] _implementations_value;
    protected List<string> used_keys;
    protected List<string> used_values;
    public static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);
    private static GUIContent
         moveButtonContent = new GUIContent("\u21b4", "move down"),
         duplicateButtonContent = new GUIContent("+", "duplicate"),
         deleteButtonContent = new GUIContent("-", "delete");

    private void ShowButtons(SerializedProperty keys, SerializedProperty values, int index)
    {
        if (GUILayout.Button(moveButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth))
        {
            keys.MoveArrayElement(index, index + 1);
            values.MoveArrayElement(index, index + 1);
        }
        if (GUILayout.Button(duplicateButtonContent, EditorStyles.miniButtonMid, miniButtonWidth))
        {
            keys.InsertArrayElementAtIndex(index + 1);
            values.InsertArrayElementAtIndex(index + 1);
            keys.GetArrayElementAtIndex(index + 1);

            string[] names = _implementations_key.Select(impl => impl.name).ToArray();
            int _implementationTypeIndex = 0;
            string str = _implementations_key[_implementationTypeIndex].name;
            while (used_keys.Contains(str))
            {
                _implementationTypeIndex++;
                str = _implementations_key[_implementationTypeIndex].name;
            }
            keys.GetArrayElementAtIndex(index + 1).objectReferenceValue = _implementations_key[_implementationTypeIndex];
        }
        if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight, miniButtonWidth))
        {
            keys.DeleteArrayElementAtIndex(index);
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        Rect contentPosition = EditorGUI.PrefixLabel(position, label);

        SerializedProperty keys = property.FindPropertyRelative("keys");
        SerializedProperty values = property.FindPropertyRelative("values");

        used_keys = new List<string>();
        used_values = new List<string>();

        EditorGUI.indentLevel += 1;

        GUILayout.BeginHorizontal();

        if (searchPathKey() != "")
        {
            if (_implementations_key == null)
            {
                _implementations_key = Resources.LoadAll(searchPathKey());
            }

            EditorGUILayout.LabelField($"Found {_implementations_key.Count()} " + searchPathKey().ToLower(), GUILayout.MinWidth(80f));

            if (GUILayout.Button(new GUIContent("\u21ba", "Refresh " + searchPathKey().ToLower()), GUILayout.Width(40f)))
            {
                _implementations_key = Resources.LoadAll(searchPathKey());
            }

            for (int x = 0; x < keys.arraySize; x++)
            {
                if(keys.GetArrayElementAtIndex(x).objectReferenceValue)
                    used_keys.Add(keys.GetArrayElementAtIndex(x).objectReferenceValue.name);
            }
        }

        if (searchPathValue() != "")
        {
            if (_implementations_value == null)
            {
                _implementations_value = Resources.LoadAll(searchPathValue());
            }

            EditorGUILayout.LabelField($"Found {_implementations_key.Count()} " + searchPathValue().ToLower(), GUILayout.MinWidth(80f));

            if (GUILayout.Button(new GUIContent("\u21ba", "Refresh " + searchPathValue().ToLower()), GUILayout.Width(40f)))
            {
                _implementations_value = Resources.LoadAll(searchPathValue());
            }

            for (int x = 0; x < values.arraySize; x++)
            {
                if(values.GetArrayElementAtIndex(x).objectReferenceValue)
                    used_values.Add(values.GetArrayElementAtIndex(x).objectReferenceValue.name);
            }
        }

        GUILayout.EndHorizontal();

        if (keys.arraySize == 0)
        {
            if (GUILayout.Button(duplicateButtonContent))
            {
                keys.InsertArrayElementAtIndex(0);
                values.InsertArrayElementAtIndex(0);
                keys.GetArrayElementAtIndex(0);

                string[] names = _implementations_key.Select(impl => impl.name).ToArray();
                int _implementationTypeIndex = 0;
                string str = _implementations_key[_implementationTypeIndex].name;
                while (used_keys.Contains(str))
                {
                    _implementationTypeIndex++;
                    str = _implementations_key[_implementationTypeIndex].name;
                }
                keys.GetArrayElementAtIndex(0).objectReferenceValue = _implementations_key[_implementationTypeIndex];
            }
        }
        else
        {
            for (int i = 0; i < keys.arraySize; i++)
            {
                GUILayout.BeginHorizontal();

                ListField(keys, values, i);

                ShowButtons(keys, values, i);

                GUILayout.EndHorizontal();
            }
        }

        EditorGUI.indentLevel -= 1;
        EditorGUI.EndProperty();
    }


    protected void DropDown(SerializedProperty list, UnityEngine.Object[] _implementations, List<string> used, int i)
    {
        string[] names = _implementations.Select(impl => impl.name).ToArray();
        string oname = "";
        if (list.GetArrayElementAtIndex(i).objectReferenceValue)
            oname = list.GetArrayElementAtIndex(i).objectReferenceValue.name;
        int _implementationTypeIndex = 0;

        for (int x = 0; x < names.Length; x++)
        {
            if (names[x] == oname)
                _implementationTypeIndex = x;
        }

        int temp_implementationTypeIndex = EditorGUILayout.Popup(GUIContent.none,
            _implementationTypeIndex, names, GUILayout.MinWidth(120f));

        string str = _implementations[temp_implementationTypeIndex].name;
        if (!used.Contains(str) || list.GetArrayElementAtIndex(i).objectReferenceValue.name == str)
        {
            _implementationTypeIndex = temp_implementationTypeIndex;
            list.GetArrayElementAtIndex(i).objectReferenceValue = _implementations[_implementationTypeIndex];
        }
    }

    protected virtual string searchPathKey() { return ""; }
    protected virtual string searchPathValue() { return ""; }
    protected abstract void ListField(SerializedProperty keys, SerializedProperty values, int i);
}

[CustomPropertyDrawer(typeof(SerializableDictionary<PickUpInteractable, int>))]
class SerializableDictionaryEditor_PUI_int : SerializableDictionaryEditor
{
    override protected string searchPathKey()
    {
        return "Prefabs";
    }

    override protected void ListField(SerializedProperty keys, SerializedProperty values, int i)
    {
        EditorGUILayout.PropertyField(values.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(40f));

        EditorGUILayout.LabelField("x", GUILayout.Width(30f));

        DropDown(keys, _implementations_key, used_keys, i);
    }
}

[CustomPropertyDrawer(typeof(SerializableDictionary<Quest, bool>))]
class SerializableDictionaryEditor_Q_bool : SerializableDictionaryEditor
{
    override protected string searchPathKey()
    {
        return "Quests";
    }

    override protected void ListField(SerializedProperty keys, SerializedProperty values, int i)
    {
        DropDown(keys, _implementations_key, used_keys, i);

        EditorGUILayout.PropertyField(values.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(40f));
    }
}

[CustomEditor(typeof(QuestManager))]
class QMEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}

[CustomPropertyDrawer(typeof(SerializableDictionary<Room, PortalComponent>))]
class SerializableDictionaryEditor_R_PC : SerializableDictionaryEditor
{
    override protected string searchPathKey()
    {
        return "Rooms";
    }

    override protected string searchPathValue()
    {
        return "Portals";
    }

    override protected void ListField(SerializedProperty keys, SerializedProperty values, int i)
    {
        DropDown(keys, _implementations_key, used_keys, i);
        DropDown(values, _implementations_value, used_values, i);
    }
}

[CustomEditor(typeof(KeyInteractable))]
class KeyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}