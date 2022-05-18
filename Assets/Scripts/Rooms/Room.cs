using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private Transform access = null;
    [SerializeField] private bool hasAccessDoor = false;
    [SerializeField] private Vector2 accessDoor = Vector2.zero;
    [SerializeField] private WallManager accessWall = null;
    [SerializeField] private string prefabName = null;

    public string PrefabName { get => prefabName; set => prefabName = value; }

    public Transform AddAccessDoor()
    {
        if (hasAccessDoor)
        {
            accessWall.doors.Add(accessDoor);
            accessWall.UpdateWall();
        }

        return access;
    }
}
