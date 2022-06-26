using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Key that can be picked up
/// </summary>
public class KeyInteractable : PickUpInteractable
{
    [SerializeField] private SerializableDictionary<Room, PortalComponent> rooms;
    public bool temporary;

    public (Room, PortalComponent) GetRandomRoom()
    {
        int rdm = Random.Range(0, rooms.Count);
        return (rooms.Keys.ToList()[rdm], rooms.Values.ToList()[rdm]);
    }

    public float MaxWidth()
    {
        float width = float.MinValue;
        foreach (PortalComponent pc in rooms.Values)
        {
            if(pc.width > width)
                width = pc.width;
        }
        return width;
    }
}