using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalConnector : MonoBehaviour
{
    static float x = 0;

    protected void InsertPortal(Vector3 pos, Quaternion rot, Room roomType, PortalComponent portalType, Material wall_material)
    {
        PortalComponent p1 = Instantiate(portalType, pos, rot);
        p1.GetComponentInChildren<DoorInteractable>().blocked = true;
        p1.GetComponentInChildren<DoorInteractable>().enabled = false;
        x += 100;
        Room room = Instantiate(roomType, Vector3.forward * x, new Quaternion());
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

    IEnumerator GrantAccess(PortalComponent p1)
    {
        yield return new WaitForSeconds(1);

        p1.GetComponentInChildren<DoorInteractable>().blocked = false;
        p1.GetComponentInChildren<DoorInteractable>().enabled = true;

        foreach (MeshRenderer mr in p1.GetComponentsInChildren<MeshRenderer>())
        {
            if (mr.gameObject.name == "_Hide")
                mr.enabled = false;
        }
    }
}
