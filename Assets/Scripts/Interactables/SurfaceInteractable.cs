using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class of surfaces that portals can be placed on
/// </summary>
public abstract class SurfaceInteractable : Interactable
{
    public SurfaceManager manager;
    private bool isTemporary = false;

    public bool IsTemporary { get => isTemporary; set => isTemporary = value; }

    /// <summary>
    /// Add Door if possible
    /// </summary>
    /// <param name="pos">Position of raycast-hit to place door at</param>
    override public void OnBuild(Vector3 pos)
    {
        Inventory inv = GameObject.Find("Player").GetComponent<Inventory>();
        Item item = inv.CurrentItem();

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
            if (inv.CanRemoveItem(item) && !(PlayerController.hasTempPath && key.temporary && !isTemporary))
            {
                (Room r, PortalComponent p) = key.GetRandomRoom();
                if (manager.AddDoor(pos, r, p, key.temporary))
                    if(!key.temporary)
                        inv.RemoveItem(item);
            }
        }
    }

    /// <summary>
    /// Show outline if viewed at with key
    /// </summary>
    /// <param name="pos">Raycast-hit position</param>
    /// <param name="portalType">Type of Portal outline</param>
    public void OnViewedAtWithKey(Vector3 pos, float width)
    {
        manager.OnViewedAtWithKey(pos, width);
    }

    override public void OnFocus()
    {
        return;
    }

    override public void OnLoseFcous()
    {
        return;
    }

    public override void OnInteract(Vector3 pos)
    {
        return;
    }
}
