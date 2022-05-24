using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

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
            var slot = Instantiate(saveSlot, content);
            slot.transform.position = new Vector3(content.position.x + content.rect.width / 2, content.position.y - i * 40 - 25, 0);
            slot.saveFile = sf;
            slot.title.text = sf.name;

            i++;
        }

        content.sizeDelta = new Vector2(0 ,10 + i * 40);
    }

    SaveFile[] GetSaveFiles()
    {
        #if (UNITY_EDITOR)
            return  Resources.LoadAll<SaveFile>("Saves/");
        #else
            List<SaveFile> o = new List<SaveFile>();

            foreach (string name in System.IO.Directory.GetFiles(Application.persistentDataPath))
            {
                string destination = Application.persistentDataPath + "/" + name + ".save";
                FileStream file;

                if (File.Exists(destination)) file = File.OpenRead(destination);
                else
                {
                    Debug.LogError("File not found");
                    continue;
                }

                BinaryFormatter bf = new BinaryFormatter();
                string data = (string)bf.Deserialize(file);
                SaveFile sf = new SaveFile();
                sf.data = data;
                sf.name = name;

                o.Add(sf);
                file.Close();
            }
        return o.ToArray();
        #endif
    }
}
