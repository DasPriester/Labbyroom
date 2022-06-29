using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeInteractable : PickUpInteractable
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
        inventoryMenu.SwitchMenu("ForgeMenu");
        inventoryMenu.GetInventoryMenu().ToggleMenu();
    }
    public override void OnPickUp(Vector3 hit)
    {
        if (UseAudio)
        {
            PlayPickUpAudio();
        }

        if (UseParticle)
            Instantiate(pickUpParticle, transform.position, Quaternion.identity).Play();

        GameObject.Find("Player").GetComponent<Inventory>().AddItem(prefab, prefabName);

        Destroy(gameObject);
    }
}
