using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Interactable
{
    public WallManager manager;

    override public void OnInteract(Vector3 pos)
    {
        manager.AddDoor(pos, 2);
    }

    override public void OnFocus(Vector3 pos)
    {
        manager.DrawDoor(pos, 2, GetComponent<MeshRenderer>().material);
    }

    override public void OnLoseFcous()
    {
        return;
    }
}
