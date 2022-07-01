using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInteractable : PickUpInteractable
{
    private InventoryMenu inventoryMenu;

    public override void Start()
    {

        prefabName = GetComponent<Moveable>().PrefabName;
        prefab = (GameObject)Resources.Load("Prefabs/" + prefabName);
        toolTip.SetText(Utility.GetKeyName(pc.settings.interactKey));
    }

    public override void OnInteract(Vector3 pos)
    {

        inventoryMenu = GameObject.Find("UI").GetComponent<InventoryMenu>();
        inventoryMenu.SwitchMenu("ChestMenu", this);
        inventoryMenu.GetInventoryMenu().ToggleMenu();
        inventoryMenu.oldKey = pc.settings.GetMenu("InventoryMenu").openKey;
        inventoryMenu.GetInventoryMenu().openKey = pc.settings.interactKey;
    }
    public override void OnPickUp(Vector3 hit)
    {
        if (UseAudio)
        {
            PlayPickUpAudio();
        }

        if (UseParticle)
            Instantiate(pickUpParticle, transform.position, Quaternion.identity).Play();

        pc.GetComponent<Inventory>().AddItem(prefab, prefabName);

        Destroy(gameObject);
    }
}
