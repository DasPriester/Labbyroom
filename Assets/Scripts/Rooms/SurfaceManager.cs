using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceManager : PortalConnector
{
    public virtual bool AddDoor(Vector3 pos, Room roomType, PortalComponent portalType)
    {
        return true;
    }

    public virtual void OnViewedAtWithKey(Vector3 pos, PortalComponent portalType)
    {
        return;
    }
}
