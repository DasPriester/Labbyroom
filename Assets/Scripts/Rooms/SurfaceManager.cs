using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class of a manager of a surface to place portals on
/// </summary>
public abstract class SurfaceManager : PortalConnector
{
    /// <summary>
    /// Add a door to the managed surface
    /// </summary>
    /// <param name="pos">Position where to add the portal</param>
    /// <param name="roomType">Type of room to summon</param>
    /// <param name="portalType">Type of portal to summon</param>
    /// <returns></returns>
    public abstract bool AddDoor(Vector3 pos, Room roomType, PortalComponent portalType, bool temporary);

    /// <summary>
    /// Display preview of portal
    /// </summary>
    /// <param name="pos">Position of the preview</param>
    /// <param name="portalType">Type of portal to preview</param>
    public abstract void OnViewedAtWithKey(Vector3 pos, PortalComponent portalType);
}
