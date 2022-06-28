using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeInteractable : Interactable
{
    private InventoryMenu inventoryMenu;
    

    public override void OnInteract(Vector3 pos)
    {

        inventoryMenu = GameObject.Find("UI").GetComponent<InventoryMenu>();
        inventoryMenu.SwitchMenu("ForgeMenu");
        inventoryMenu.GetInventoryMenu().ToggleMenu();
    }
}
