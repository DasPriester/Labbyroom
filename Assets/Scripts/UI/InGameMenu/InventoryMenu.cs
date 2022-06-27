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
    private Vector2 spacing = new Vector2(10, -10);

    // Hotbar UI
    private static readonly Color halfWhite = new Color(1, 1, 1, 0.4f);
    private GameObject[] hotbarSlots;

    // Crafting UI
    [SerializeField] private CraftingMenu craftingMenuPrefab;
    private CraftingMenu craftingMenu;

    private void Awake()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        inventoryMenu = playerController.settings.GetMenu("InventoryMenu");
        inv = playerController.GetComponent<Inventory>(); 
       
        hotbarSlots = new GameObject[inv.HotbarSize];
        inventorySlots = new GameObject[inv.InventorySize];
        itemUIs = new ItemUI[inv.HotbarSize + inv.InventorySize];
        craftingMenu = Instantiate(craftingMenuPrefab, inventoryMenu.transform);

        inventoryMenu.OpenMenu = craftingMenu.UpdateCraftMenu;
        inventoryMenu.CloseMenu = craftingMenu.CloseCraftMenu;

        Transform hotbarGrid = this.transform.Find("Hotbar BG/Hotbar Grid");
        Transform inventoryGrid = inventoryMenu.transform.Find("BG/Inv Grid");
        for (int i = 0; i < inv.HotbarSize; i++)
            hotbarSlots[i] = Instantiate(itemSlot, hotbarGrid);

        for (int i = 0; i < inv.InventorySize; i++)
            inventorySlots[i] = Instantiate(itemSlot, inventoryGrid);

        hotbarSlots[inv.CurrentSlot].GetComponent<Image>().color = Color.white;

        transform.Find("InventoryIcon/Image/Text").GetComponent<Text>().text = playerController.settings.GetKey("Inventory").ToString();
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

        if(Input.mouseScrollDelta != Vector2.zero && InGameMenu.instance == null)
        {
            hotbarSlots[inv.CurrentSlot].GetComponent<Image>().color = halfWhite;
            inv.CurrentSlot -= (int)Input.mouseScrollDelta.y;
            if (inv.CurrentSlot >= inv.HotbarSize)
                inv.CurrentSlot = 0;
            else if (inv.CurrentSlot < 0)
                inv.CurrentSlot = inv.HotbarSize - 1;
            hotbarSlots[inv.CurrentSlot].GetComponent<Image>().color = Color.white;
        }
    }

    public void AddItemUI(Item item, int index)
    {
        GameObject itemUI = Instantiate(itemPref,
            index <= inv.HotbarSize ?
            this.transform.Find("Hotbar BG") :
            inventoryMenu.transform.Find("BG"));

        itemUI.GetComponent<RectTransform>().anchoredPosition =
            (index <= inv.HotbarSize ?
            hotbarSlots[index].GetComponent<RectTransform>().anchoredPosition :
            inventorySlots[index - inv.HotbarSize].GetComponent<RectTransform>().anchoredPosition)
            + spacing;

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

    public void DroppedOff(ItemUI item, GameObject slotObj)
    {
        int oldIndex = Array.FindIndex(itemUIs, x => x == item);
        int newIndex = Array.FindIndex(hotbarSlots, x => x.gameObject == slotObj);
        if(newIndex == -1)
            newIndex = inv.HotbarSize + Array.FindIndex(inventorySlots, x => x.gameObject == slotObj);
        (itemUIs[oldIndex], itemUIs[newIndex]) = (itemUIs[newIndex], itemUIs[oldIndex]);
        (inv.Items[oldIndex], inv.Items[newIndex]) = (inv.Items[newIndex], inv.Items[oldIndex]);
    }

    public void DroppedOff(ItemUI item1, ItemUI item2)
    {
        int oldIndex = Array.FindIndex(itemUIs, x => x == item1);
        int newIndex = Array.FindIndex(itemUIs, x => x == item2);
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

        StartCoroutine(AddItemsWithDelay());

        craftingMenu.UpdateCraftMenu();
        craftingMenu.CloseCraftMenu();
    }

    private IEnumerator AddItemsWithDelay()
    {
        yield return new WaitForSeconds(0);

        for (int i = 0; i < inv.Items.Length; i++)
        {
            if (inv.Items[i].prefab != null)
            {
                AddItemUI(inv.Items[i], i);
            }
        }

    }
    public void OpenInventoryMenu()
    {

        InGameMenu.instance = inventoryMenu;
    }
    public void CloseInventoryMenu()
    {
        InGameMenu.instance = null;
    }

    public void UpdateCraftMenu()
    {
        craftingMenu.UpdateCraftMenu();
    }

    public void CloseCraftMenu()
    {
        craftingMenu.CloseCraftMenu();
    }
}
