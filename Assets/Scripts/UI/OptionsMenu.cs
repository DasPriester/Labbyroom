using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] InputSetterButton inputSetterButton = null; 
    [SerializeField] InputBooleanButton inputBooleanButton = null;

    string[] keys = {
        "Sprint",
        "Jump",
        "Crouch",
        "Interact",
        "Place",
        "Inventory 1",
        "Inventory 2",
        "Inventory 3",
        "Inventory 4",
        "Inventory 5",
        "Inventory 6",
        "Inventory 7",
        "Crafting",
        "Menu"
    };

    string[] bools =
    {
        "Footsteps",
        "Headbob"
    };

    private void Start()
    {
        RectTransform content = GameObject.Find("Canvas/OptionsMenu/Scroll View/Viewport/Content").GetComponent<RectTransform>();
        int i = 0;
        foreach (string k in keys)
        {
            InputSetterButton isb = Instantiate(inputSetterButton, content);

            isb.transform.position = new Vector3(content.position.x + content.rect.width / 2 + 50 , content.position.y - i * 40 - 25, 0);
            isb.Sets = k;
            i++;
        }

        foreach (string b in bools)
        {
            InputBooleanButton ibb = Instantiate(inputBooleanButton, content);

            ibb.transform.position = new Vector3(content.position.x + content.rect.width / 2 + 50, content.position.y - i * 40 - 25, 0);
            ibb.Sets = b;
            i++;
        }
    }
}
