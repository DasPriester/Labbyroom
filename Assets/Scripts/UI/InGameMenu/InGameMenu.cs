using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Basic class for each ingame menu to implement
/// </summary>
public class InGameMenu : Hideable
{
    public KeyCode openKey;

    public delegate void OpenMenuDelegate();
    public delegate void CloseMenuDelegate();
    public OpenMenuDelegate OpenMenu;
    public CloseMenuDelegate CloseMenu;

    float menuCooldown = 0;
    PlayerController pc;

    static public InGameMenu instance;

    private void Awake()
    {
        pc = Camera.main.GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        if (menuCooldown > 0)
            menuCooldown -= Time.unscaledDeltaTime;
        else
            menuCooldown = 0;

        if ((instance == null || instance == this) && Input.GetKey(openKey) && menuCooldown == 0)
        {
            ToggleMenu();
        }
    }

    /// <summary>
    /// If the menu is toggled, invoke the proper method 
    /// </summary>
    public void ToggleMenu()
    {
        menuCooldown = 0.2f;

        Hidden = !Hidden;

        if (hidden)
        {
            CloseMenu?.Invoke();
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            OpenMenu?.Invoke();
            Cursor.lockState = CursorLockMode.None;
        }
        Cursor.visible = !hidden;

        GameObject.Find("UI/CenterDot").GetComponent<Image>().color = hidden ? Color.white : Color.clear;

        pc.enabled = hidden;
    }
}
