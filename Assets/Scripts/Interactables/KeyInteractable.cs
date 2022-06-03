using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Key that can be picked up
/// </summary>
public class KeyInteractable : PickUpInteractable
{
    public Room roomType;
    public PortalComponent portalType;
    public bool temporary;
}
