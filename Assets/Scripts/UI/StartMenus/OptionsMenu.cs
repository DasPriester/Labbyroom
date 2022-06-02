using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Menu to change settings
/// </summary>
public class OptionsMenu : MonoBehaviour
{
    [SerializeField] InputSetterButton inputSetterButton = null; 
    [SerializeField] InputBooleanButton inputBooleanButton = null;
    [SerializeField] InputVolumeSlider inputVolumeSlider = null;
    [SerializeField] InputSlider inputSlider = null;

    enum Category
    {
        CONTROLLS,
        AUDIO,
        VIDEO
    }

    enum InputType
    {
        KEY,
        BOOL,
        VOLUME,
        SLIDER
    }
    Category currentCategory = Category.CONTROLLS;

    readonly Dictionary<string, InputType> controlls = new Dictionary<string, InputType> {
        {"Sprint", InputType.KEY},
        {"Jump", InputType.KEY},
        {"Crouch", InputType.KEY},
        {"Interact", InputType.KEY},
        {"Place", InputType.KEY},
        {"Inventory 1", InputType.KEY},
        {"Inventory 2", InputType.KEY},
        {"Inventory 3", InputType.KEY},
        {"Inventory 4", InputType.KEY},
        {"Inventory 5", InputType.KEY},
        {"Inventory 6", InputType.KEY},
        {"Inventory 7", InputType.KEY},
        {"Crafting", InputType.KEY},
        { "Menu",  InputType.KEY},
    };

    new readonly Dictionary<string, InputType> audio = new Dictionary<string, InputType> {
        {"Master Volume", InputType.VOLUME},
        {"Effects Volume", InputType.VOLUME},
        {"Music Volume", InputType.VOLUME},
        {"Footsteps", InputType.BOOL},
    };

    readonly Dictionary<string, InputType> video = new Dictionary<string, InputType> {
        {"Headbob", InputType.BOOL},
        {"Particles", InputType.BOOL},
    };

    private void Start()
    {
        SetMenu(currentCategory);
    }

    private void SetMenu(Category category)
    {
        switch (category)
        {
            case Category.CONTROLLS:
                SpawnMenu(controlls);
                currentCategory = Category.CONTROLLS;
                break;
            case Category.AUDIO:
                SpawnMenu(audio);
                currentCategory = Category.AUDIO;
                break;
            case Category.VIDEO:
                SpawnMenu(video);
                currentCategory = Category.VIDEO;
                break;
        }
    }

    public void SetMenu(string category)
    {
        switch (category)
        {
            case "Controlls":
                if (Category.CONTROLLS != currentCategory)
                    SetMenu(Category.CONTROLLS);
                break;
            case "Audio":
                if (Category.AUDIO != currentCategory)
                    SetMenu(Category.AUDIO);
                break;
            case "Video":
                if (Category.VIDEO != currentCategory)
                    SetMenu(Category.VIDEO);
                break;
        }
    }

    private void SpawnMenu(Dictionary<string, InputType> menu)
    {
        RectTransform content = GameObject.Find("Canvas/OptionsMenu/Scroll View/Viewport/Content").GetComponent<RectTransform>();

        foreach (RectTransform rt in content.GetComponentsInChildren<RectTransform>())
        {
            if (rt != content)
                Destroy(rt.gameObject);
        }

        Vector3 newPos(int i) => new Vector3(content.position.x + content.rect.width / 2 + 50, content.position.y - i * 40 - 25, 0);

        int i = 0;
        foreach (KeyValuePair<string, InputType> k in menu)
        {
            
            switch (k.Value)
            {
                case InputType.KEY:
                    var isb = Instantiate(inputSetterButton, content);
                    isb.transform.position = newPos(i);
                    isb.Sets = k.Key;
                    break;

                case InputType.BOOL:
                    var ibb = Instantiate(inputBooleanButton, content);
                    ibb.transform.position = newPos(i);
                    ibb.Sets = k.Key;
                    break;

                case InputType.SLIDER:
                    var ins = Instantiate(inputSlider, content);
                    ins.transform.position = newPos(i);
                    ins.Sets = k.Key;
                    break;

                case InputType.VOLUME:
                    var ivs = Instantiate(inputVolumeSlider, content);
                    ivs.transform.position = newPos(i);
                    ivs.AudioChannel = k.Key;
                    ivs.Sets = k.Key;
                    break;
            }
            i++;
        }

        content.sizeDelta = new Vector2(0 ,10 + i * 40);
    }
}
