using UnityEngine;

using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "Scriptable Object/Recipe", order = 1)]
public class Recipe : ScriptableObject
{
    public string name;

    public SerializableDictionary<PickUpInteractable, int> Cost;
    public SerializableDictionary<PickUpInteractable, int> Yield;
}

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    private int size = 0;

    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<TValue> values = new List<TValue>();

    // save the dictionary to lists
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    // load dictionary from lists
    public void OnAfterDeserialize()
    {
        this.Clear();
        if (keys.Count > size)
            for (int i = size; i < keys.Count; i++)
            {
                keys[i] = default;
            }
            size = keys.Count;

        while (keys.Count < size)
            size = keys.Count;

        while (values.Count > size)
            values.RemoveAt(values.Count - 1);

        while (values.Count < size)
            values.Add(default);

        for (int i = 0; i < keys.Count; i++)
            this.Add(keys[i], values[i]);
    }
}