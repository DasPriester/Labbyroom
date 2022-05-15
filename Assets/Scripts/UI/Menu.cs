using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : Hideable
{
    [SerializeField]
    public KeyCode openKey;

    public delegate void OpenMenuDelegate();
    public delegate void CloseMenuDelegate();
    public OpenMenuDelegate OpenMenu;
    public CloseMenuDelegate CloseMenu;

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
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        menuCooldown = 0.2f;
        bool hidden = ToggleHide();

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
