using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wall that a portal can be placed in
/// </summary>
public class WallInteractable : SurfaceInteractable
{
    public override void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Wall");

    }
}
