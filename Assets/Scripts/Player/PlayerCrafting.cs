using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerCrafting : MonoBehaviour
{
    [SerializeField]
    KeyCode openKey;

    [SerializeField]
    Menu craftingMenu = null;

    [SerializeField]
    RectTransform entry = null;

    RectTransform content;

    PlayerInventory inv = null;
    Recipe[] recipes;
    PlayerController pc;

    float menuCooldown = 0;

    private void Awake()
    {
        recipes = Resources.LoadAll<Recipe>("Recipes"); ;
        content = GameObject.Find("UI/CraftingMenu/Scroll View/Viewport/Content").GetComponent<RectTransform>();

        UpdateCraftMenu();

        pc = Camera.main.GetComponentInParent<PlayerController>();
        inv = GetComponentInChildren<PlayerInventory>();
    }

    private void UpdateCraftMenu()
    {
        if (!inv)
            inv = GetComponentInChildren<PlayerInventory>();

        float i = 0;
        foreach (Recipe rec in recipes)
        {
            if (rec.unlocked)
            {
                RectTransform nen = Instantiate(entry, content);

                nen.Find("Name").GetComponent<Text>().text = rec.name;
                nen.Find("Cost").GetComponent<Text>().text = "Cost: " + DictToString(rec.Cost);
                nen.Find("Yield").GetComponent<Text>().text = "Yield: " + DictToString(rec.Yield);
                string nameOfResult = "";
                foreach (PickUpInteractable p in rec.Yield.Keys)
                {
                    nameOfResult = p.name;
                    break;
                }
                nen.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + nameOfResult);
                Button button = nen.Find("CraftButton").GetComponent<Button>();
                button.onClick.AddListener(() => { inv.CraftRecipe(rec); UpdateCraftMenu(); });
                bool craftable = inv.IsCraftable(rec);
                button.GetComponent<Image>().color = craftable ? Color.green : Color.gray;
                button.interactable = craftable;

                nen.transform.position += Vector3.down * 70 * i;
                i++;
            }
        }

        entry.Find("Name").GetComponent<Text>().text = "Name";
        entry.Find("CraftButton").GetComponent<Button>().onClick.AddListener(() => { });
    }

    private string DictToString(Dictionary<PickUpInteractable, int> dict)
    {
        string[] o = new string[dict.Count];

        int i = 0;
        foreach (PickUpInteractable pi in dict.Keys)
        {
            o[i] = dict[pi] + "x " + pi.name;
            i++;
        }

        return string.Join(", ", o);
    }

    private void Update()
    {
        if (menuCooldown > 0)
            menuCooldown -= Time.deltaTime;
        else
            menuCooldown = 0;

        if ((Input.GetKey(KeyCode.Menu) || Input.GetKey(openKey)) && menuCooldown == 0)
        {
            menuCooldown = 0.2f;
            bool hidden = craftingMenu.ToggleHide();

            if (hidden)
                Cursor.lockState = CursorLockMode.Locked;
            else
            {
                Cursor.lockState = CursorLockMode.None;
                UpdateCraftMenu();
            }
            Cursor.visible = !hidden;

            GameObject.Find("UI/CenterDot").GetComponent<Image>().color = hidden ? Color.white : Color.clear;

            pc.enabled = hidden;
        }
    }

}
