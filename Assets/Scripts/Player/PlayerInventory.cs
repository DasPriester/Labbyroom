using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private GameObject invUI = default;


    struct Item
    {
        public string name;
        public int amount;

        public Item(string name, int amount) { this.name = name; this.amount = amount; }   
    }
    
    private List<Item> invList = new List<Item>();


    public void AddItem(string itemName)
    {
        int index = invList.FindIndex(x => x.name == itemName);

        if (index != -1)
        {
            Item item = invList[index];
            item.amount += 1;
            invList[index] = item;

            invUI.transform.GetChild(index).GetComponentInChildren<Text>().text = "" + invList[index].amount;

        }
        else
        {
            invList.Add(new Item(itemName, 1));

            Transform slot = invUI.transform.GetChild(invList.Count - 1);
            Image image = slot.GetComponentsInChildren<Image>()[1];
            image.enabled = true;
            image.sprite = Resources.Load<Sprite>("Sprites/" + itemName);
            slot.GetComponentInChildren<Text>().enabled = true;

        }
    }
}
