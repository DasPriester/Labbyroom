using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpInteractable : Interactable
{
    public override void OnInteract()
    {
        Destroy(gameObject);
    }
    public override void OnFocus()
    {
        print("Looking at " + gameObject.name);
    }
    public override void OnLoseFcous()
    {
        print("Stopped looking at " + gameObject.name);
    }
}
