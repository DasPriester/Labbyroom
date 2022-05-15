using UnityEngine;
using System.Collections.Generic;

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
        public KeyCode[] inventoryKeys = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7 };

        //Menus
        public Menu[] menus = { };
        public Dictionary<string ,Menu> liveMenus = new Dictionary<string, Menu>();

    //Video
    public bool useHeadbob = true;

    //Audio
    public bool useFootsteps = true;

    private void OnEnable()
    {
        if (menus.Length < 1)
            menus = Resources.LoadAll<Menu>("Menus");

        liveMenus = new SerializableDictionary<string, Menu>();
    }


    public Menu GetMenu(string name)
    {
        if (liveMenus.ContainsKey(name))
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
}