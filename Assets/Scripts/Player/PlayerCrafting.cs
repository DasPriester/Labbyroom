using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerCrafting : MonoBehaviour
{
    [SerializeField]
    Menu craftingMenu = null;

    [SerializeField]
    RectTransform entry = null;

    RectTransform content;

    PlayerInventory inv = null;
    Recipe[] recipes;

    private void Awake()
    {
        recipes = Resources.LoadAll<Recipe>("Recipes"); ;
        content = GameObject.Find("UI/CraftingMenu/Scroll View/Viewport/Content").GetComponent<RectTransform>();
        inv = GetComponentInChildren<PlayerInventory>();

        craftingMenu.UpdateMenu = UpdateCraftMenu;
    }

    private void UpdateCraftMenu()
    {
        if (!inv)
            inv = GetComponentInChildren<PlayerInventory>();

        foreach (RectTransform g in content.GetComponentsInChildren<RectTransform>())
            if (g != content)
                Destroy(g.gameObject);

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

        if (i > 0)
            craftingMenu.transform.Find("Scroll View/NoContent").GetComponent<Text>().text = "";
        else
            craftingMenu.transform.Find("Scroll View/NoContent").GetComponent<Text>().text = "You have to discover a recipe first...";
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

}
