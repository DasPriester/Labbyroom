using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSurface : Interactable
{
    public SurfaceManager manager;

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
                if (!manager.AddDoor(pos, key.roomType, key.portalType))
                    inv.AddItem(item);
            }
        }
    }

    public void OnViewedAtWithKey(Vector3 pos, PortalComponent portalType)
    {
        manager.OnViewedAtWithKey(pos, portalType);
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
