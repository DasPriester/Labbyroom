using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] InputSetterButton inputSetterButton = null; 
    [SerializeField] InputBooleanButton inputBooleanButton = null;
    [SerializeField] InputSlider inputSlider = null;

    enum Category
    {
        CONTROLLS,
        AUDIO,
        VIDEO
    }

    enum ButtonType
    {
        KEY,
        BOOL,
        SLIDER
    }

    Category currentCategory = Category.CONTROLLS;

    readonly Dictionary<string, ButtonType> controlls = new Dictionary<string, ButtonType> {
        {"Sprint", ButtonType.KEY},
        {"Jump", ButtonType.KEY},
        {"Crouch", ButtonType.KEY},
        {"Interact", ButtonType.KEY},
        {"Place", ButtonType.KEY},
        {"Inventory 1", ButtonType.KEY},
        {"Inventory 2", ButtonType.KEY},
        {"Inventory 3", ButtonType.KEY},
        {"Inventory 4", ButtonType.KEY},
        {"Inventory 5", ButtonType.KEY},
        {"Inventory 6", ButtonType.KEY},
        {"Inventory 7", ButtonType.KEY},
        {"Crafting", ButtonType.KEY},
        { "Menu",  ButtonType.KEY},
    };

    new readonly Dictionary<string, ButtonType> audio = new Dictionary<string, ButtonType> {
        {"Master Volume", ButtonType.SLIDER},
        {"Effects Volume", ButtonType.SLIDER},
        {"Music Volume", ButtonType.SLIDER},
        {"Footsteps", ButtonType.BOOL},
    };

    readonly Dictionary<string, ButtonType> video = new Dictionary<string, ButtonType> {
        {"Headbob", ButtonType.BOOL},
        {"Particles", ButtonType.BOOL},
    };

    private void Start()
    {
        SetMenu(currentCategory);
    }

    private void SetMenu(Category cathegory)
    {
        switch (cathegory)
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

    public void SetMenu(string cathegory)
    {
        switch (cathegory)
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

    private void SpawnMenu(Dictionary<string, ButtonType> menu)
    {
        RectTransform content = GameObject.Find("Canvas/OptionsMenu/Scroll View/Viewport/Content").GetComponent<RectTransform>();

        foreach (RectTransform rt in content.GetComponentsInChildren<RectTransform>())
        {
            if (rt != content)
                Destroy(rt.gameObject);
        }

        int i = 0;
        foreach (KeyValuePair<string, ButtonType> k in menu)
        {
            switch (k.Value)
            {
                case ButtonType.KEY:
                    var isb = Instantiate(inputSetterButton, content);
                    isb.transform.position = new Vector3(content.position.x + content.rect.width / 2 + 50, content.position.y - i * 40 - 25, 0);
                    isb.Sets = k.Key;
                    break;
                case ButtonType.BOOL:
                    var ibb = Instantiate(inputBooleanButton, content);
                    ibb.transform.position = new Vector3(content.position.x + content.rect.width / 2 + 50, content.position.y - i * 40 - 25, 0);
                    ibb.Sets = k.Key;
                    break;
                case ButtonType.SLIDER:
                    var inps = Instantiate(inputSlider, content);
                    inps.transform.position = new Vector3(content.position.x + content.rect.width / 2 + 50, content.position.y - i * 40 - 25, 0);
                    inps.Sets = k.Key;
                    break;
            }
            i++;
        }

        content.sizeDelta = new Vector2(0 ,10 + i * 40);
    }
}
