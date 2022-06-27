using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to manage new Room and Portals connection
/// </summary>
public class PortalConnector : MonoBehaviour
{
    static int z_temp = 0;
    static int z_perm = 0;

    public static int ZTemp { get => z_temp; set => z_temp = value; }
    public static int ZPerm { get => z_perm; set => z_perm = value; }

    /// <summary>
    /// Insert portal into a wall
    /// </summary>
    /// <param name="pos">Postion where to spawn the portal</param>
    /// <param name="rot">rotation of the portal</param>
    /// <param name="roomType">Type of Room to spawn</param>
    /// <param name="portalType">Type of portal to summon</param>
    /// <param name="wall_material">Material of the Wall above the portal</param>
    protected void InsertPortal(Vector3 pos, Quaternion rot, Room roomType, PortalComponent portalType, Material wall_material, bool temporary, Room currentRoom, Vector2 door, WallManager wallManager)
    {
        PortalComponent p1 = Instantiate(portalType, pos, rot);
        p1.GetComponentInChildren<DoorInteractable>().enabled = false;
        p1.GetComponentInChildren<DoorInteractable>().gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        if (temporary)
            z_temp += 1;
        else
            z_perm += 1;
        Room room = Instantiate(roomType, 100 * (temporary ? z_temp : z_perm) * Vector3.forward + 100 * (temporary ? 1 : 0) * Vector3.right, new Quaternion());
        Transform coords = room.AddAccessDoor();
        PortalComponent p2 = Instantiate(portalType, coords);
        room.IsTemporary = temporary;

        p1.linkedPortal = p2;
        p2.linkedPortal = p1;
        p1.IsTemporary = temporary;

        p1.Room = currentRoom;
        p2.Room = room;
        p1.WallManager = wallManager;
        p1.Door = door;

        foreach (MeshRenderer mr in p1.GetComponentsInChildren<MeshRenderer>())
        {
            if (mr.gameObject.name == "_Wall" || mr.gameObject.name == "_Hide")
                mr.material = wall_material;
        }
        foreach (MeshRenderer mr in p1.GetComponentsInChildren<MeshRenderer>())
        {
            if (mr.gameObject.name != "_Hide")
                mr.material.SetFloat("_TimeAppear", Time.time);
        }
        foreach (MeshRenderer mr in p2.GetComponentsInChildren<MeshRenderer>())
        {
            if (mr.gameObject.name == "_Hide")
                mr.enabled = false;
        }

        p1.GetComponentInChildren<DoorInteractable>().UpdateConnection();
        p2.GetComponentInChildren<DoorInteractable>().UpdateConnection();

        StartCoroutine(GrantAccess(p1));
    }

    /// <summary>
    /// DIsable portal for a second so the animation can play
    /// </summary>
    /// <param name="p1">Portal to disable</param>
    IEnumerator GrantAccess(PortalComponent p1)
    {
        yield return new WaitForSeconds(1);

        p1.GetComponentInChildren<DoorInteractable>().enabled = true;
        p1.GetComponentInChildren<DoorInteractable>().gameObject.layer = LayerMask.NameToLayer("Interactable");

        foreach (MeshRenderer mr in p1.GetComponentsInChildren<MeshRenderer>())
        {
            if (mr.gameObject.name == "_Hide")
                mr.enabled = false;
        }
    }
}
