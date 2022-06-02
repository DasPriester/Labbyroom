using UnityEngine;

using System;
using System.Collections.Generic;

/// <summary>
/// Utility functions used throughout the game
/// </summary>
public class Utility : MonoBehaviour
{
    public static Sprite GetIconFor(Item item)
    {
        Sprite sprite = Resources.Load<Sprite>("Sprites/Icons/" + item.name);

        if (!sprite)
        {
            Texture2D saved = Resources.Load<Texture2D>("Sprites/Icons/" + item.name + "_tmp");

            if (saved)
                sprite = Sprite.Create(saved, new Rect(0.0f, 0.0f, saved.width, saved.height), new Vector2(0.5f, 0.5f), 100.0f, 0, SpriteMeshType.Tight);

            #if (UNITY_EDITOR)
                else
                {
                    CreateIconFor(item);
                }
            #endif
        }


        return sprite;
    }

    #if (UNITY_EDITOR)
        public static void CreateIconFor(Item item)
        {
            Texture2D tex = UnityEditor.AssetPreview.GetAssetPreview(item.prefab);
            Color32[] colors = tex.GetPixels32();
            int i = 0;
            Color alpha = colors[i];
            for (; i < colors.Length; i++)
            {
                if (colors[i] == alpha)
                {
                    colors[i].a = 0;
                }
            }
            tex.SetPixels32(colors);
            byte[] bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes("Assets/Resources/Sprites/Icons/" + item.name + "_tmp.PNG", bytes);
        }
    #endif

    public static bool FastApproximately(float a, float b, float threshold)
    {
        if (threshold > 0f)
        {
            return Mathf.Abs(a - b) <= threshold;
        }
        else
        {
            return Mathf.Approximately(a, b);
        }
    }

    public static void QuitGame()
    {
        #if (UNITY_EDITOR)
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
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