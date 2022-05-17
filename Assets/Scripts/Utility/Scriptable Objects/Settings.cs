using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Settings", menuName = "Scriptable Object/Settings", order = 1)]
public class Settings : ScriptableObject
{
    //Controlls
        //Movement
        public static KeyCode sprintKey = KeyCode.LeftShift;
        public static KeyCode jumpKey = KeyCode.Space;
        public static KeyCode crouchKey = KeyCode.LeftControl;
        public static KeyCode interactKey = KeyCode.E;
        public static KeyCode placeKey = KeyCode.Q;

        //Inventory
        public static KeyCode[] inventoryKeys = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7 };

        //Menus
        public static Menu[] menus = { };
        public Dictionary<string ,Menu> liveMenus = new Dictionary<string, Menu>();

    //Video
    public static bool useHeadbob = true;
    public static bool particlesActivated = true;

    //Audio
    public static float masterVolume = 1f;
    public static float effectsVolume = 1f;
    public static float musicVolume = 1f;

    public static bool useFootsteps = true;

    private void OnEnable()
    {
        if (menus.Length < 1)
            menus = Resources.LoadAll<Menu>("Menus");

        liveMenus = new Dictionary<string, Menu>();
    }

    public Menu GetMenu(string name)
    {
        if (liveMenus.ContainsKey(name))
            if (liveMenus[name] == null)
            {
                foreach (Menu menu in menus)
                {
                    if (menu.name == name)
                    {
                        liveMenus[name] = Instantiate<Menu>(menu, FindObjectOfType<PlayerCrafting>().transform);
                        return liveMenus[name];
                    }
                }

                return null;
            }
            else
                return liveMenus[name];
        else
        {
            foreach (Menu menu in menus)
            {
                if (menu.name == name)
                {
                    liveMenus.Add(menu.name, Instantiate<Menu>(menu, FindObjectOfType<PlayerCrafting>().transform));
                    return liveMenus[name];
                }
            }

            return null;
        }
    }

    public static void SetKey(string id, KeyCode key)
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
            case "Crafting":
                foreach (Menu menu in menus)
                {
                    if (menu.name == "CraftingMenu")
                    {
                        menu.openKey = key;
                    }
                }
                break;
            case "Menu":
                foreach (Menu menu in menus)
                {
                    if (menu.name == "PauseMenu")
                    {
                        menu.openKey = key;
                    }
                }
                break;
        }
    }

    public static KeyCode GetKey(string id)
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
            case "Crafting":
                foreach (Menu menu in menus)
                {
                    if (menu.name == "CraftingMenu")
                    {
                        return menu.openKey;
                    }
                }
                break;
            case "Menu":
                foreach (Menu menu in menus)
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

    public static void SetBool(string id, bool value)
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

    public static bool GetBool(string id)
    {
        switch (id)
        {
            case "Footsteps":
                return useFootsteps;
            case "Headbob":
                return useHeadbob;
            case "Particles":
                return particlesActivated;
        }

        return false;
    }

    public static void SetValue(string id, float value)
    {
        switch (id)
        {
            case "Volume":
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

    public static float GetValue(string id)
    {
        switch (id)
        {
            case "Volume":
                return masterVolume;
            case "Effects Volume":
                return effectsVolume;
            case "Music Volume":
                return musicVolume;
        }

        return 0f;
    }
}