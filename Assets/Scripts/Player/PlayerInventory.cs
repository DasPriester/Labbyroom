using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerInventory : MonoBehaviour
{
    private Color halfWhite = new Color(1, 1, 1, 0.4f);

    private GameObject[] slots = new GameObject[7];
    private Item[] invList = new Item[7];
    private int current = 0;

    public void Awake()
    {
        int i = 0;
        foreach (Transform child in transform)
        {
            slots[i] = child.gameObject;
            i++;
        }
        slots[current].GetComponent<Image>().color = Color.white;
    }


    public void Update()
    {
        for (int i = 1; i < 8; i++)
        {
            if (Input.GetKeyDown((KeyCode)(48 + i)))
            {
                slots[current].GetComponent<Image>().color = halfWhite;
                current = i-1;
                slots[current].GetComponent<Image>().color = Color.white;

            }
        }

    }

    public void AddItem(GameObject prefab, string name)
    {
        int index = Array.FindIndex<Item>(invList, x => x.name == name);

        if (index != -1)
        {
            invList[index].amount += 1;

            slots[index].GetComponentInChildren<Text>().text = "" + invList[index].amount;
        }
        else
        {
            for (int i = 0; i < invList.Length; i++)
            {
                if (invList[i].prefab == null)
                {
                    invList[i] = new Item(prefab, name, 1);

                    GameObject slot = slots[i];
                    Image image = slot.GetComponentsInChildren<Image>()[1];
                    image.enabled = true;
                    image.sprite = Resources.Load<Sprite>("Sprites/" + name);
                    slot.GetComponentInChildren<Text>().enabled = true;
                    slot.GetComponentInChildren<Text>().text = "" + 1;
                    break;
                }
            }
        }
    }

    public void AddItem(Item item)
    {
        int index = Array.FindIndex<Item>(invList, x => x.name == item.name);

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
                    image.sprite = Resources.Load<Sprite>("Sprites/" + item.name);
                    slot.GetComponentInChildren<Text>().enabled = true;
                    slot.GetComponentInChildren<Text>().text = "" + item.amount;

                    break;
                }
            }
        }
    }

    public bool RemoveItem(Item item)
    {
        int index = Array.FindIndex<Item>(invList, x => x.name == item.name);

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

    public bool CanRemoveItem(Item item)
    {
        int index = Array.FindIndex<Item>(invList, x => x.name == item.name);

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
        } else
        {
            invList = invBackup;
        }
    }

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

    public Item CurrentItem()
    {
        if(current >= invList.Length)
            return new Item(null, null, 0);

        return invList[current];
    }
}
