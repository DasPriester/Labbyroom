using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    private Color halfWhite = new Color(1, 1, 1, 0.4f);

    private GameObject[] slots = new GameObject[7];
    private List<Item> invList = new List<Item>();
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
        int index = invList.FindIndex(x => x.name == name);

        if (index != -1)
        {
            Item item = invList[index];
            item.amount += 1;
            invList[index] = item;

            slots[index].GetComponentInChildren<Text>().text = "" + invList[index].amount;

        }
        else
        {
            invList.Add(new Item(prefab, name, 1));

            GameObject slot = slots[invList.Count - 1];
            Image image = slot.GetComponentsInChildren<Image>()[1];
            image.enabled = true;
            image.sprite = Resources.Load<Sprite>("Sprites/" + name);
            slot.GetComponentInChildren<Text>().enabled = true;

        }
    }

    public void RemoveItem(Item item)
    {
        int index = invList.IndexOf(item);

        if (item.amount > 1)
        {
            Item oldItem = invList[index];
            oldItem.amount -= 1;
            invList[index] = oldItem;

            slots[index].GetComponentInChildren<Text>().text = "" + invList[index].amount;
        }
        else
        {
            invList.Remove(item);

            GameObject slot = slots[index];
            Image image = slot.GetComponentsInChildren<Image>()[1];
            image.enabled = false;
            image.sprite = null;
            slot.GetComponentInChildren<Text>().enabled = false;
        }

    }
    public Item CurrentItem()
    {
        if(current >= invList.Count)
            return new Item(null, null, 0);

        return invList[current];
    }
}
