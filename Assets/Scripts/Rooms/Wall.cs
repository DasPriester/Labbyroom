using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Interactable
{
    public WallManager manager;

    override public void OnInteract(Vector3 pos)
    {
        PlayerInventory inv = GameObject.Find("UI/Inventory").GetComponent<PlayerInventory>();
        var item = inv.CurrentItem();

        Key key;

        if (item.prefab != null)
        {
            key = item.prefab.GetComponent<Key>();
        }else
        {
            return;
        }

        if (key) {
            item.amount = 1;
            if (inv.RemoveItem(item))
            {
                if (!manager.AddDoor(pos, 2, key.roomType, key.portalType))
                    inv.AddItem(item);
            }
        }

    }

    override public void Awake()
    {
        gameObject.layer = 6;
    }

    override public void OnFocus(Vector3 pos)
    {
        return;
    }

    override public void OnLoseFcous()
    {
        return;
    }
}
