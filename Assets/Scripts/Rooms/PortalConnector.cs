using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to manage new Room and Portals connection
/// </summary>
public class PortalConnector : MonoBehaviour
{
    static int z = 0;

    public static int Z { get => z; set => z = value; }

    /// <summary>
    /// Insert portal into a wall
    /// </summary>
    /// <param name="pos">Postion where to spawn the portal</param>
    /// <param name="rot">rotation of the portal</param>
    /// <param name="roomType">Type of Room to spawn</param>
    /// <param name="portalType">Type of portal to summon</param>
    /// <param name="wall_material">Material of the Wall above the portal</param>
    protected void InsertPortal(Vector3 pos, Quaternion rot, Room roomType, PortalComponent portalType, Material wall_material)
    {
        PortalComponent p1 = Instantiate(portalType, pos, rot);
        p1.GetComponentInChildren<DoorInteractable>().enabled = false;
        p1.GetComponentInChildren<DoorInteractable>().gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        z += 1;
        Room room = Instantiate(roomType, 100 * z * Vector3.forward, new Quaternion());
        Transform coords = room.AddAccessDoor();
        PortalComponent p2 = Instantiate(portalType, coords);

        p1.linkedPortal = p2;
        p2.linkedPortal = p1;

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
