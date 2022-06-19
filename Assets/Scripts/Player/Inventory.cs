using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Inventory manager
/// </summary>
public class Inventory : MonoBehaviour
{
    private static readonly int inventorySize = 63;
    private static readonly int hotbarSize = 9;

    private InventoryMenu InvUI;
    private Item[] invList = new Item[hotbarSize + inventorySize];

    private int current = 0;
    public Item[] Items {
        get { return invList; }
        set { invList = value; }
    }
    public int CurrentSlot
    {
        get { return current; }
        set { current = value; }
    }
    public int HotbarSize { get { return hotbarSize; } }
    public int InventorySize { get { return inventorySize; } }

    private void Awake()
    {
        InvUI = GameObject.Find("UI").GetComponent<InventoryMenu>();
    }


    public void RefreshUI()
    {
        InvUI.RefreshUI();
    }

    /// <summary>
    /// Add item by name and prefab to inventory
    /// </summary>
    /// <param name="prefab">Prefab of Item</param>
    /// <param name="name">Name of Item</param>
    public void AddItem(GameObject prefab, string name)
    {
        AddItem(new Item(prefab, name, 1));
    }

    /// <summary>
    /// Add item to inventory
    /// </summary>
    /// <param name="item">Item to be added</param>
    public void AddItem(Item item)
    {
        int index = Array.FindIndex(invList, x => x.name == item.name);

        if (index != -1)
        {
            invList[index].amount += item.amount;
            InvUI.UpdateCountUI(index);
        }
        else
        {
            for (int i = 0; i < inventorySize; i++)
            {
                
                if (invList[i].prefab == null)
                {
                    invList[i] = item;

                    InvUI.AddItemUI(item, i);
                    break;
                }
            }
        }
    }


    /// <summary>
    /// Remove item from inventory
    /// </summary>
    /// <param name="item">Item to be removed</param>
    /// <returns>True if sucessfully removed item else false</returns>
    public bool RemoveItem(Item item)
    {
        int index = Array.FindIndex(invList, x => x.name == item.name);

        if (index == -1)
            return false;

        if (invList[index].amount < item.amount)
            return false;
            

        invList[index].amount -= item.amount;

        if (invList[index].amount < 1)
        {
            invList[index] = new Item(null, null, 0);

            InvUI.RemoveItemUI(index);

        }
        else
        {
            InvUI.UpdateCountUI(index);
        }
        return true;
      
    }

    /// <summary>
    /// Check if Item can be removed
    /// </summary>
    /// <param name="item">Item to be checked for</param>
    /// <returns>Trueif removing is possible else false</returns>
    public bool CanRemoveItem(Item item)
    {
        int index = Array.FindIndex(invList, x => x.name == item.name);

        if (index == -1)
        {
            return false;
        }
        else
        {
            if (invList[index].amount >= item.amount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Craft recipe and add/remove corresponding items
    /// </summary>
    /// <param name="recipe">Recipe to craft</param>
    public void CraftRecipe(Recipe recipe)
    {
        Item[] invBackup = invList;
        bool canCraft = true;
        foreach (PickUpInteractable costName in recipe.Cost.Keys)
        {
            if (!RemoveItem(new Item(costName.gameObject, costName.name, recipe.Cost[costName])))
                canCraft = false;
        }

        if (canCraft)
        {
            foreach (PickUpInteractable yieldName in recipe.Yield.Keys)
            {
                AddItem(new Item(yieldName.gameObject, yieldName.name, recipe.Yield[yieldName]));
            }
        } 
        else
        {
            invList = invBackup;
        }
    }

    /// <summary>
    /// Check if recipe is craftable
    /// </summary>
    /// <param name="recipe">Recipe to check for</param>
    /// <returns>True if recipe is craftable else false</returns>
    public bool IsCraftable(Recipe recipe)
    {
        if (recipe == null)
            return false;

        bool canCraft = true;
        foreach (PickUpInteractable costName in recipe.Cost.Keys)
        {
            if (!CanRemoveItem(new Item(costName.gameObject, costName.name, recipe.Cost[costName])))
                canCraft = false;
        }
        return canCraft;
    }

    /// <summary>
    /// Get Currently selected item
    /// </summary>
    /// <returns>Currently selected item, nulled item if nothing selected</returns>
    public Item CurrentItem()
    {
        if(current >= hotbarSize)
            return new Item(null, null, 0);

        return invList[current];
    }
}
