using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

/// <summary>
/// In game crafting menu
/// </summary>
public class InventoryMenu : MonoBehaviour
{
    private PlayerController playerController;
    private Inventory inv = null;

    // Inventory UI
    [SerializeField] private GameObject itemSlot = null;
    [SerializeField] private GameObject itemPref = null;
    private InGameMenu inventoryMenu = null;
    private GameObject[] inventorySlots;
    private ItemUI[] itemUIs;

    // Crafting UI
    [SerializeField] private RectTransform entry = null;
    private RectTransform content = null;
    private Recipe[] recipes;

    // Hotbar UI
    private static readonly Color halfWhite = new Color(1, 1, 1, 0.4f);
    private GameObject[] hotbarSlots;


    private void Awake()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        inventoryMenu = playerController.settings.GetMenu("InventoryMenu");
        inv = playerController.GetComponent<Inventory>();
        recipes = Resources.LoadAll<Recipe>("Recipes");

        hotbarSlots = new GameObject[inv.HotbarSize];
        inventorySlots = new GameObject[inv.HotbarSize + inv.InventorySize];
        itemUIs = new ItemUI[inv.HotbarSize + inv.InventorySize];


        inventoryMenu.OpenMenu = UpdateCraftMenu;
        inventoryMenu.CloseMenu = CloseCraftMenu;

        for (int i = 0; i < inv.HotbarSize; i++)
            hotbarSlots[i] = Instantiate(itemSlot, this.transform.Find("Hotbar BG/Hotbar Grid"));

        for (int i = 0; i < inv.InventorySize; i++)
            inventorySlots[i] = Instantiate(itemSlot, inventoryMenu.transform.Find("Inv BG/Inv Grid"));

        hotbarSlots[inv.CurrentSlot].GetComponent<Image>().color = Color.white;
    }

    public void Update()
    {
        for (int i = 0; i < 7; i++)
        {
            if (Input.GetKeyDown(playerController.settings.inventoryKeys[i]))
            {
                hotbarSlots[inv.CurrentSlot].GetComponent<Image>().color = halfWhite;
                inv.CurrentSlot = i;
                hotbarSlots[inv.CurrentSlot].GetComponent<Image>().color = Color.white;

            }
        }
    }

    public void AddItemUI(Item item, int index){

        GameObject itemUI = Instantiate(itemPref,
            index < inv.HotbarSize ?
            this.transform.Find("Hotbar BG") : 
            inventoryMenu.transform.Find("Inv BG"));
        
        itemUI.GetComponent<RectTransform>().anchoredPosition =
            index < inv.HotbarSize ?
            hotbarSlots[index].GetComponent<RectTransform>().anchoredPosition :
            inventorySlots[index-inv.HotbarSize].GetComponent<RectTransform>().anchoredPosition;
            
        
        itemUI.GetComponent<ItemUI>().Item = item;
        itemUI.GetComponent<Image>().sprite = Utility.GetIconFor(item);
        itemUI.GetComponentInChildren<Text>().text = item.amount.ToString();

        itemUIs[index] = itemUI.GetComponent<ItemUI>();
    }
    public void UpdateCountUI(int index)
    {
        itemUIs[index].GetComponentInChildren<Text>().text = inv.Items[index].amount.ToString();
    }

    public void RemoveItemUI(int index)
    {
        Destroy(itemUIs[index].gameObject);
        itemUIs[index] = null;
    }

    public void DroppedOff(ItemUI Item, GameObject SlotObj)
    {
        int oldIndex = Array.FindIndex(itemUIs, x => x == Item);
        int newIndex = Array.FindIndex(hotbarSlots, x => x.gameObject == SlotObj);
        if(newIndex == -1)
            newIndex = inv.HotbarSize + Array.FindIndex(inventorySlots, x => x.gameObject == SlotObj);
        (itemUIs[oldIndex], itemUIs[newIndex]) = (itemUIs[newIndex], itemUIs[oldIndex]);
        (inv.Items[oldIndex], inv.Items[newIndex]) = (inv.Items[newIndex], inv.Items[oldIndex]);
    }


    public void RefreshUI()
    {
        // TODO
    }

    /// <summary>
    /// Updates visuals for crafting menu and each recipe
    /// </summary>
    private void UpdateCraftMenu()
    {

        InGameMenu.instance = inventoryMenu;

        if (!content)
            content = inventoryMenu.transform.Find("Scroll View/Viewport/Content").GetComponent<RectTransform>();

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

                nen.transform.position += 120 * i * Vector3.down;
                i++;
            }
        }

        if (i > 0)
            inventoryMenu.transform.Find("Scroll View/NoContent").GetComponent<Text>().text = "";
        else
            inventoryMenu.transform.Find("Scroll View/NoContent").GetComponent<Text>().text = "You have to discover a recipe first...";
    }

    private void CloseCraftMenu()
    {
        InGameMenu.instance = null;
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
