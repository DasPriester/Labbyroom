using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// In game crafting menu
/// </summary>
public class CraftingMenu : MonoBehaviour
{
    InGameMenu craftingMenu = null;

    [SerializeField]
    RectTransform entry = null;

    RectTransform content;

    Inventory inv = null;
    Recipe[] recipes;

    private void Awake()
    {
        craftingMenu = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerController>().settings.GetMenu("CraftingMenu");
        recipes = Resources.LoadAll<Recipe>("Recipes");

        craftingMenu.OpenMenu = UpdateCraftMenu;
    }

    /// <summary>
    /// Updates visuals for menu and each recipe
    /// </summary>
    private void UpdateCraftMenu()
    {
        if (!inv)
            inv = GetComponentInChildren<Inventory>();

        if (!content)
            content = GameObject.Find("UI/CraftingMenu(Clone)/Scroll View/Viewport/Content").GetComponent<RectTransform>();

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
                Item item = new Item();
                foreach (PickUpInteractable p in rec.Yield.Keys)
                {
                    item.name= p.name;
                    item.prefab = p.gameObject;
                    break;
                }
                nen.Find("Image").GetComponent<Image>().sprite = Utility.GetIconFor(item);

                Button button = nen.Find("CraftButton").GetComponent<Button>();
                button.onClick.AddListener(() => { inv.CraftRecipe(rec); UpdateCraftMenu(); });

                bool craftable = inv.IsCraftable(rec);
                button.GetComponent<Image>().color = craftable ? Color.green : Color.gray;
                button.interactable = craftable;

                nen.transform.position += 70 * i * Vector3.down;
                i++;
            }
        }

        if (i > 0)
            craftingMenu.transform.Find("Scroll View/NoContent").GetComponent<Text>().text = "";
        else
            craftingMenu.transform.Find("Scroll View/NoContent").GetComponent<Text>().text = "You have to discover a recipe first...";
    }

    /// <summary>
    /// Convert an inventory-dict to string
    /// </summary>
    /// <param name="dict">Dictionary to convert</param>
    /// <returns>String representation of dict</returns>
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
