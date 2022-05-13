using UnityEngine;

public class Utility : MonoBehaviour
{

    public static Sprite GetIconFor(Item item)
    {
        Sprite sprite = Resources.Load<Sprite>("Sprites/" + item.name);
        if (!sprite)
        {
            Texture2D saved = Resources.Load<Texture2D>("Sprites/" + item.name + "_tmp");

            if (saved)
                sprite = Sprite.Create(saved, new Rect(0.0f, 0.0f, saved.width, saved.height), new Vector2(0.5f, 0.5f), 100.0f, 0, SpriteMeshType.Tight);
            else
            {
                CreateIconFor(item);
            }
        }

        return sprite;
    }

    public static void CreateIconFor(Item item)
    {
        Texture2D tex = UnityEditor.AssetPreview.GetAssetPreview(item.prefab);
        Color[] colors = tex.GetPixels();
        int i = 0;
        Color alpha = colors[i];
        for (; i < colors.Length; i++)
        {
            if (colors[i] == alpha)
            {
                colors[i].a = 0;
            }
        }
        tex.SetPixels(colors);
        byte[] bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes("Assets/Resources/Sprites/" + item.name + "_tmp.PNG", bytes);
    }

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
}