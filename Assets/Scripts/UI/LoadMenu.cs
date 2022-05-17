using System.Collections.Generic;
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
        foreach (SaveFile sf in Resources.LoadAll<SaveFile>("Saves/"))
        {
            var slot = Instantiate(saveSlot, content);
            slot.transform.position = new Vector3(content.position.x + content.rect.width / 2, -(i + 1) * 40, 0);
            slot.saveFile = sf;
            slot.title.text = sf.name;

            i++;
        }

        content.sizeDelta = new Vector2(0 ,10 + i * 40);
    }
}
