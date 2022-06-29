using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;

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
            sprite = Resources.Load<Sprite>("Sprites/Icons/" + item.name + "_tmp");

        }
        if(!sprite)
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
            string path = "Assets/Resources/Sprites/Icons/" + item.name + "_tmp.PNG";
            System.IO.File.WriteAllBytes(path, bytes);
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.AssetDatabase.ImportAsset(path);
            UnityEditor.TextureImporter importer = UnityEditor.AssetImporter.GetAtPath(path) as UnityEditor.TextureImporter;
            importer.textureType = UnityEditor.TextureImporterType.Sprite;
            UnityEditor.AssetDatabase.WriteImportSettingsIfDirty(path);
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

    public static Type[] GetImplementations<T>()
    {
        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());

        var interfaceType = typeof(T);
        return types.Where(p => interfaceType.IsAssignableFrom(p) && !p.IsAbstract).ToArray();
    }

    public static string GetKeyName(KeyCode key)
    {
        return key switch
        {
            KeyCode.Alpha0 => "0",
            KeyCode.Alpha1 => "1",
            KeyCode.Alpha2 => "2",
            KeyCode.Alpha3 => "3",
            KeyCode.Alpha4 => "4",
            KeyCode.Alpha5 => "5",
            KeyCode.Alpha6 => "6",
            KeyCode.Alpha7 => "7",
            KeyCode.Alpha8 => "8",
            KeyCode.Alpha9 => "9",
            KeyCode.Mouse0 => "LMB",
            KeyCode.Mouse1 => "RMB",
            _ => key.ToString(),
        };
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