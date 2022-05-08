using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : PickUpInteractable
{
    [SerializeField] public Room roomType;
    [SerializeField] public PortalComponent portalType;
}
