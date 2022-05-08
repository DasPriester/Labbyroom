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

    public struct Item
    {
        public GameObject prefab;
        public string name;
        public int amount;

        public Item(GameObject prefab, string name, int amount ) 
        { 
            this.prefab = prefab; 
            this.name = name;
            this.amount = amount; 
        }   
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
            invList[index].amount += 1;

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

            if (invList[index].amount >= 1)
            {
                slots[index].GetComponentInChildren<Text>().text = "" + invList[index].amount;
            }
            else
            {
                invList[index] = new Item(null, null, 0);

                GameObject slot = slots[index];
                Image image = slot.GetComponentsInChildren<Image>()[1];
                image.enabled = false;
                image.sprite = null;
                slot.GetComponentInChildren<Text>().enabled = false;
            }

            return true;
        }

    }
    public Item CurrentItem()
    {
        if(current >= invList.Length)
            return new Item(null, null, 0);

        return invList[current];
    }
}
