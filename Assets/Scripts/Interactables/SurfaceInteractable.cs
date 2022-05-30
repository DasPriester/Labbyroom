using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class of surfaces that portals can be placed on
/// </summary>
public abstract class SurfaceInteractable : Interactable
{
    public SurfaceManager manager;

    /// <summary>
    /// Add Door if possible
    /// </summary>
    /// <param name="pos">Position of raycast-hit to place door at</param>
    override public void OnInteract(Vector3 pos)
    {
        Inventory inv = GameObject.Find("UI/Inventory").GetComponent<Inventory>();
        var item = inv.CurrentItem();

        KeyInteractable key;

        if (item.prefab != null)
        {
            key = item.prefab.GetComponent<KeyInteractable>();
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

    /// <summary>
    /// Show outline if viewed at with key
    /// </summary>
    /// <param name="pos">Raycast-hit position</param>
    /// <param name="portalType">Type of Portal outline</param>
    public void OnViewedAtWithKey(Vector3 pos, PortalComponent portalType)
    {
        manager.OnViewedAtWithKey(pos, portalType);
    }

    override public void OnFocus()
    {
        return;
    }

    override public void OnLoseFcous()
    {
        return;
    }
}
