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
    private Color halfWhite = new Color(1, 1, 1, 0.4f);

    private readonly GameObject[] slots = new GameObject[7];
    private Item[] invList = new Item[7];
    private int current = 0;
    private PlayerController playerController;

    public Item[] Items {
        get { return invList; }
        set { invList = value; UpdateUI(); }
    }
    public int CurrentSlot
    {
        get { return current; }
        set { current = value; }
    }

    public void Awake()
    {
        int i = 0;
        foreach (Transform child in transform)
        {
            slots[i] = child.gameObject;
            i++;
        }
        slots[current].GetComponent<Image>().color = Color.white;

        playerController = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerController>();
    }


    public void Update()
    {
        for (int i = 0; i < 7; i++)
        {
            if (Input.GetKeyDown(playerController.settings.inventoryKeys[i]))
            {
                slots[current].GetComponent<Image>().color = halfWhite;
                current = i;
                slots[current].GetComponent<Image>().color = Color.white;

            }
        }

    }
    /// <summary>
    /// Refresh Inventory UI
    /// </summary>
    private void UpdateUI()
    {
        for (int i = 0; i < invList.Length; i++)
        {
            if (invList[i].prefab != null)
            {
                GameObject slot = slots[i];
                Image image = slot.GetComponentsInChildren<Image>()[1];
                image.enabled = true;

                image.sprite = Utility.GetIconFor(invList[i]);
                slot.GetComponentInChildren<Text>().enabled = true;
                slot.GetComponentInChildren<Text>().text = "" + invList[i].amount;
            }
        }
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

            slots[index].GetComponentInChildren<Text>().text = "" + invList[index].amount;
        }
        else
        {
            for (int i = 0; i < invList.Length; i++)
            {
                if (invList[i].prefab == null)
                {
                    invList[i] = item;

                    GameObject slot = slots[i];
                    Image image = slot.GetComponentsInChildren<Image>()[1];
                    image.enabled = true;

                    image.sprite = Utility.GetIconFor(item);
                    slot.GetComponentInChildren<Text>().enabled = true;
                    slot.GetComponentInChildren<Text>().text = "" + item.amount;

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
        {
            return false;
        }
        else
        {
            if (invList[index].amount >= item.amount)
            {
                invList[index].amount -= item.amount;
            } else
            {
                return false;
            }

            if (invList[index].amount < 1)
            {
                invList[index] = new Item(null, null, 0);

                GameObject slot = slots[index];
                Image image = slot.GetComponentsInChildren<Image>()[1];
                image.enabled = false;
                image.sprite = null;
                slot.GetComponentInChildren<Text>().enabled = false;
            }

            slots[index].GetComponentInChildren<Text>().text = "" + invList[index].amount;

            return true;
        }

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
        if(current >= invList.Length)
            return new Item(null, null, 0);

        return invList[current];
    }
}
