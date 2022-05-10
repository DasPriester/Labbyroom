using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : Hideable
{
    [SerializeField]
    KeyCode openKey;

    public delegate void UpdateMenuDelegate();
    public UpdateMenuDelegate UpdateMenu;

    float menuCooldown = 0;
    PlayerController pc;

    private void Awake()
    {
        pc = Camera.main.GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        if (menuCooldown > 0)
            menuCooldown -= Time.deltaTime;
        else
            menuCooldown = 0;

        if (Input.GetKey(openKey) && menuCooldown == 0)
        {
            ToggleCraftingMenu();
        }
    }

    public void ToggleCraftingMenu()
    {
        menuCooldown = 0.2f;
        bool hidden = ToggleHide();

        if (hidden)
            Cursor.lockState = CursorLockMode.Locked;
        else
        {
            Cursor.lockState = CursorLockMode.None;
            UpdateMenu();
        }
        Cursor.visible = !hidden;

        GameObject.Find("UI/CenterDot").GetComponent<Image>().color = hidden ? Color.white : Color.clear;

        pc.enabled = hidden;
    }
}
