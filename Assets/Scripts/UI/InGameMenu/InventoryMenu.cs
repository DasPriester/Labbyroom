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
    [SerializeField] private RectTransform recipeEntryPrefab = null;
    [SerializeField] private RectTransform recipeCostPrefab = null;
    private RectTransform content = null;
    private Recipe[] recipes;
    private Recipe currentRecipe = null;
    private RectTransform currentEntry = null;
    private int entryPos = 0;
    private int costPos = 0;

    // Hotbar UI
    private static readonly Color halfWhite = new Color(1, 1, 1, 0.4f);
    private GameObject[] hotbarSlots;


    private void Awake()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        inventoryMenu = playerController.settings.GetMenu("InventoryMenu");
        content = inventoryMenu.transform.Find("BG/Scroll View/Viewport/Content").GetComponent<RectTransform>();
        inv = playerController.GetComponent<Inventory>();
        recipes = Resources.LoadAll<Recipe>("Recipes");

        
        hotbarSlots = new GameObject[inv.HotbarSize];
        inventorySlots = new GameObject[inv.InventorySize];
        itemUIs = new ItemUI[inv.HotbarSize + inv.InventorySize];


        inventoryMenu.OpenMenu = UpdateCraftMenu;
        inventoryMenu.CloseMenu = CloseCraftMenu;

        Transform hotbarGrid = this.transform.Find("Hotbar BG/Hotbar Grid");
        Transform inventoryGrid = inventoryMenu.transform.Find("BG/Inv Grid");
        for (int i = 0; i < inv.HotbarSize; i++)
            hotbarSlots[i] = Instantiate(itemSlot, hotbarGrid);

        for (int i = 0; i < inv.InventorySize; i++)
            inventorySlots[i] = Instantiate(itemSlot, inventoryGrid);


        inventoryMenu.transform.Find("BG/Craft").GetComponent<Button>().onClick.AddListener(() =>
        {
            if (inv.IsCraftable(currentRecipe))
            {
                inv.CraftRecipe(currentRecipe);
                UpdateCraftMenu();
            }
        });
        hotbarSlots[inv.CurrentSlot].GetComponent<Image>().color = Color.white;

        // force griditems to be proper position on first frame (else their position in set too late after loading a savefile)
        LayoutRebuilder.ForceRebuildLayoutImmediate(inventoryGrid.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(hotbarGrid.GetComponent<RectTransform>());

    }

    public void Update()
    {
        for (int i = 0; i < playerController.settings.inventoryKeys.Length; i++)
        {
            if (Input.GetKeyDown(playerController.settings.inventoryKeys[i]) && Time.timeScale != 0)
            {
                hotbarSlots[inv.CurrentSlot].GetComponent<Image>().color = halfWhite;
                inv.CurrentSlot = i;
                hotbarSlots[inv.CurrentSlot].GetComponent<Image>().color = Color.white;

            }
        }
        Transform craftButton = inventoryMenu.transform.Find("BG/Craft");
        if (inv.IsCraftable(currentRecipe))
        { 
            craftButton.GetComponent<Image>().color = Color.green;
            craftButton.GetComponent<Button>().enabled = true;
        }
        else
        {
            craftButton.GetComponent<Image>().color = Color.gray;
            craftButton.GetComponent<Button>().enabled = false;
        }
        if(currentEntry)
            currentEntry.GetComponent<Image>().color = Color.white;
    }

    public void AddItemUI(Item item, int index)
    {
        GameObject itemUI = Instantiate(itemPref,
            index <= inv.HotbarSize ?
            this.transform.Find("Hotbar BG") :
            inventoryMenu.transform.Find("BG"));

        itemUI.GetComponent<RectTransform>().anchoredPosition =
            index <= inv.HotbarSize ?
            hotbarSlots[index].GetComponent<RectTransform>().anchoredPosition :
            inventorySlots[index - inv.HotbarSize].GetComponent<RectTransform>().anchoredPosition;

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
        foreach (GameObject hotbarSlot in hotbarSlots)
        {
            hotbarSlot.GetComponent<Image>().color = halfWhite;
        }
        hotbarSlots[inv.CurrentSlot].GetComponent<Image>().color = Color.white;

        for (int i = 0; i < inv.Items.Length; i++)
        {
            if (inv.Items[i].prefab != null)
            {
                AddItemUI(inv.Items[i], i);
            }
        }
    }

    /// <summary>
    /// Updates visuals for crafting menu and each recipe
    /// </summary>
    private void UpdateCraftMenu()
    {

        InGameMenu.instance = inventoryMenu;

        costPos = 0;
        entryPos = 0;

        // clear content
        foreach (RectTransform g in content.GetComponentsInChildren<RectTransform>())
            if (g != content)
                Destroy(g.gameObject);

        foreach (Recipe rec in recipes)
        {
            if (rec.unlocked)
            {
                RectTransform entry = Instantiate(recipeEntryPrefab, content);

                // keep stuff after crafting
                if (currentRecipe == rec)
                {
                    currentEntry = entry;
                    CreateCostEntries();
                }


                // set text and icon
                Item item = new Item();
                foreach (PickUpInteractable p in rec.Yield.Keys)
                {
                    entry.Find("Yield").GetComponent<Text>().text = rec.Yield[p] + "x " + p.name;
                    item.prefab = p.gameObject;
                    item.name = p.name;
                    break;
                }
                entry.Find("Image").GetComponent<Image>().sprite = Utility.GetIconFor(item);

                // local function to move entries below 
                void MoveEntries(bool up)
                {
                    bool found = false;
                    for (int child = 0; child < content.transform.childCount; child++)
                    {
                        if (found)
                            content.transform.GetChild(child).transform.position += (up ? -1 : 1) * 70 * costPos * Vector3.down;

                        if (content.transform.GetChild(child) == currentEntry.transform)
                            found = true;
                    }
                }

                // local function to create Cost entries
                void CreateCostEntries()
                {
                    foreach (PickUpInteractable cost in rec.Cost.Keys)
                    {
                        RectTransform costEntry = Instantiate(recipeCostPrefab, entry);
                        Item item = new Item
                        {
                            prefab = cost.gameObject,
                            name = cost.name,
                            amount = rec.Cost[cost]
                        };
                        costEntry.Find("Cost").GetComponent<Text>().color =
                            inv.CanRemoveItem(item) ?
                            Color.white :
                            Color.gray;
                        costEntry.Find("Cost").GetComponent<Text>().text = rec.Cost[cost] + "x " + cost.name;
                        costEntry.Find("Image").GetComponent<Image>().sprite = Utility.GetIconFor(item);
                        costPos++;
                        costEntry.transform.position += 70 * costPos * Vector3.down;
                    }
                }

                entry.Find("SelectButton").GetComponent<Button>().onClick.AddListener(() =>
                {
                    // close old crafting cost
                    if (currentEntry)
                    {
                        foreach (RectTransform rect in currentEntry)
                        {
                            if (rect.name == "RecipeCost(Clone)")
                                Destroy(rect.gameObject);
                        }
                        currentEntry.GetComponent<Image>().color = Color.gray;
                        MoveEntries(true);
                        costPos = 0;
                    }

                    // reset if clicked second time
                    if (currentRecipe == rec) {                     
                        currentEntry.GetComponent<Image>().color = Color.gray;
                        currentRecipe = null;
                        currentEntry = null;
                        costPos = 0;
                        entryPos = 0;
                        return;
                    }

                    currentRecipe = rec;
                    currentEntry = entry;

                    // create cost entries
                    CreateCostEntries();

                    // move entries down
                    MoveEntries(false);
                    
                });

                entry.transform.position += 120 * entryPos * Vector3.down;
                entryPos++;
            }
        }

        if (entryPos > 0)
            inventoryMenu.transform.Find("BG/Scroll View/NoContent").GetComponent<Text>().text = "";
        else
            inventoryMenu.transform.Find("BG/Scroll View/NoContent").GetComponent<Text>().text = "You have to discover a recipe first...";
    }

    private void CloseCraftMenu()
    {
        InGameMenu.instance = null;
        currentRecipe = null;
        currentEntry = null;
    }
}
