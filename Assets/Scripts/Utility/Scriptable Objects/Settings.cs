using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Scriptable object to save settings of a player
/// </summary>
[CreateAssetMenu(fileName = "Settings", menuName = "Scriptable Object/Settings", order = 1)]
public class Settings : ScriptableObject
{
    //Controlls
    //Movement
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode interactKey = KeyCode.E;
    public KeyCode placeKey = KeyCode.Q;

    //Inventory
    public KeyCode[] inventoryKeys = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9 };

    //Menus
    public InGameMenu[] menus = { };
    public Dictionary<string ,InGameMenu> liveMenus = new Dictionary<string, InGameMenu>();

    //Video
    public bool useHeadbob = true;
    public bool particlesActivated = true;

    //Audio
    public float masterVolume = 1f;
    public float effectsVolume = 1f;
    public float musicVolume = 1f;


    public bool useFootsteps = true;

    private void OnEnable()
    {
        if (menus.Length < 1)
            menus = Resources.LoadAll<InGameMenu>("Menus");

        liveMenus = new Dictionary<string, InGameMenu>();
    }


    public InGameMenu GetMenu(string name)
    {
        if (liveMenus.ContainsKey(name))
            if (liveMenus[name] == null)
            {
                foreach (InGameMenu menu in menus)
                {
                    if (menu.name == name)
                    {
                        liveMenus[name] = Instantiate(menu, FindObjectOfType<InventoryMenu>().transform);
                        return liveMenus[name];
                    }
                }

                return null;
            }
            else
                return liveMenus[name];
        else
        {
            foreach (InGameMenu menu in menus)
            {
                if (menu.name == name)
                {
                    liveMenus.Add(menu.name, Instantiate(menu, FindObjectOfType<InventoryMenu>().transform));
                    return liveMenus[name];
                }
            }

            return null;
        }
    }


    public void SetKey(string id, KeyCode key)
    {
        switch (id)
        {
            case "Sprint":
                sprintKey = key;
                break;
            case "Jump":
                jumpKey = key;
                break;
            case "Crouch":
                crouchKey = key;
                break;
            case "Interact":
                interactKey = key;
                break;
            case "Place":
                placeKey = key;
                break;
            case "Inventory 1":
                inventoryKeys[0] = key;
                break;
            case "Inventory 2":
                inventoryKeys[1] = key;
                break;
            case "Inventory 3":
                inventoryKeys[2] = key;
                break;
            case "Inventory 4":
                inventoryKeys[3] = key;
                break;
            case "Inventory 5":
                inventoryKeys[4] = key;
                break;
            case "Inventory 6":
                inventoryKeys[5] = key;
                break;
            case "Inventory 7":
                inventoryKeys[6] = key;
                break;
            case "Inventory 8":
                inventoryKeys[7] = key;
                break;
            case "Inventory 9":
                inventoryKeys[8] = key;
                break;
            case "Crafting":
                foreach (InGameMenu menu in menus)
                {
                    if (menu.name == "CraftingMenu")
                    {
                        menu.openKey = key;
                    }
                }
                break;
            case "Menu":
                foreach (InGameMenu menu in menus)
                {
                    if (menu.name == "PauseMenu")
                    {
                        menu.openKey = key;
                    }
                }
                break;
        }
    }

    public KeyCode GetKey(string id)
    {
        switch (id)
        {
            case "Sprint":
                return sprintKey;
            case "Jump":
                return jumpKey;
            case "Crouch":
                return crouchKey;
            case "Interact":
                return interactKey;
            case "Place":
                return placeKey;
            case "Inventory 1":
                return inventoryKeys[0];
            case "Inventory 2":
                return inventoryKeys[1];
            case "Inventory 3":
                return inventoryKeys[2];
            case "Inventory 4":
                return inventoryKeys[3];
            case "Inventory 5":
                return inventoryKeys[4];
            case "Inventory 6":
                return inventoryKeys[5];
            case "Inventory 7":
                return inventoryKeys[6];
            case "Inventory 8":
                return inventoryKeys[7];
            case "Inventory 9":
                return inventoryKeys[8];
            case "Crafting":
                foreach (InGameMenu menu in menus)
                {
                    if (menu.name == "CraftingMenu")
                    {
                        return menu.openKey;
                    }
                }
                break;
            case "Menu":
                foreach (InGameMenu menu in menus)
                {
                    if (menu.name == "PauseMenu")
                    {
                        return menu.openKey;
                    }
                }
                break;
        }

        return new KeyCode();
    }

    public void SetBool(string id, bool value)
    {
        switch (id)
        {
            case "Footsteps":
                useFootsteps = value;
                break;
            case "Headbob":
                useHeadbob = value;
                break;
            case "Particles":
                particlesActivated = value;
                break;
        }
    }

    public bool GetBool(string id)
    {
        switch (id)
        {
            case "Footsteps":
                return useFootsteps;
            case "Headbob":
                return useHeadbob;
            case "Particles":
                return particlesActivated;
            default:
                break;
        }

        return false;
    }

    public void SetValue(string id, float value)
    {
        switch (id)
        {
            case "Master Volume":
                masterVolume = value;
                break;
            case "Effects Volume":
                effectsVolume = value;
                break;
            case "Music Volume":
                musicVolume = value;
                break;
        }
    }

    public float GetValue(string id)
    {
        switch (id)
        {
            case "Master Volume":
                return masterVolume;
            case "Effects Volume":
                return effectsVolume;
            case "Music Volume":
                return musicVolume;
            default:
                break;
        }

        return 0f;
    }
}