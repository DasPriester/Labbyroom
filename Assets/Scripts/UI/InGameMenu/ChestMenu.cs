using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestMenu : SubMenu
{
    // Inventory UI
    [SerializeField] private GameObject itemSlot = null;
    [SerializeField] private GameObject itemPref = null;
    private GameObject[] inventorySlots;
    private ItemUI[] itemUIs;
    private Vector2 spacing = new Vector2(10, -10);

    public Inventory Inv { get { return inv; }set { inv = value; } }

    
    /// <summary>
    /// Updates visuals for crafting menu and each recipe
    /// </summary>
    public void CreateInv()
    {

        inventorySlots = new GameObject[inv.InventorySize];
        Transform inventoryGrid = transform.Find("Scroll BG/Inv Grid");
        for (int i = 0; i < inv.InventorySize; i++)
            inventorySlots[i] = Instantiate(itemSlot, inventoryGrid);
    }
    public override void CloseMenu()
    {

        InventoryMenu inventoryMenu = GameObject.Find("UI").GetComponent<InventoryMenu>();
        inventoryMenu.SwitchMenu("CraftingMenu");
        inventoryMenu.GetInventoryMenu().openKey = inventoryMenu.oldKey;
    }

    public override void OpenMenu()
    {
    }
}
