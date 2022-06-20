using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Load menu to continue a save file
/// </summary>
public class LoadMenu : MonoBehaviour
{
    [SerializeField] SaveSlot saveSlot = null;

    private void Start()
    {
        SpawnSaveSlots();
    }

    private void SpawnSaveSlots()
    {
        RectTransform content = GameObject.Find("Canvas/LoadMenu/Scroll View/Viewport/Content").GetComponent<RectTransform>();

        foreach (RectTransform rt in content.GetComponentsInChildren<RectTransform>())
        {
            if (rt != content)
                Destroy(rt.gameObject);
        }

        int i = 0;
        foreach (SaveFile sf in GetSaveFiles())
        {
            SaveSlot slot = Instantiate(saveSlot, content);
            slot.transform.position = new Vector3(content.position.x + content.rect.width / 2, content.position.y - i * 45 - 25, 0);
            slot.saveFile = sf;
            slot.title.text = sf.name;
            slot.transform.Find("Delete").GetComponent<Button>().onClick.AddListener(() =>
            {
                File.Delete(Application.persistentDataPath + Path.DirectorySeparatorChar + slot.saveFile.name + ".save");
                SpawnSaveSlots();
            });

            i++;
        }

        content.sizeDelta = new Vector2(0 ,10 + i * 40);
    }

    SaveFile[] GetSaveFiles()
    {
        
        List<SaveFile> o = new List<SaveFile>();
        foreach (string name in Directory.GetFiles(Application.persistentDataPath))
        {
            if (name.Replace(Application.persistentDataPath + Path.DirectorySeparatorChar, "").Replace(".save", "") == ".DS_Store")
                continue;

            FileStream file;

            if (File.Exists(name)) 
                file = File.OpenRead(name);
            else
            {
                Debug.LogError("File not found");
                continue;
            }

            SaveFile sf = ScriptableObject.CreateInstance<SaveFile>();
            sf.data = File.ReadAllText(name);
            sf.name = name.Replace(Application.persistentDataPath + Path.DirectorySeparatorChar, "").Replace(".save", "");

            o.Add(sf);
            file.Close();
        }
    return o.ToArray();
    }
}
