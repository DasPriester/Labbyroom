using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerCrafting : MonoBehaviour
{
    [SerializeField]
    KeyCode openKey;

    [SerializeField]
    Menu craftingMenu = null;

    [SerializeField]
    RectTransform entry = null;

    RectTransform content;

    PlayerInventory inv = null;
    Recipe[] recipes;
    PlayerController pc;

    float menuCooldown = 0;

    private void Awake()
    {
        recipes = Resources.LoadAll<Recipe>("Recipes"); ;
        content = GameObject.Find("UI/CraftingMenu/Scroll View/Viewport/Content").GetComponent<RectTransform>();

        UpdateCraftMenu();

        pc = Camera.main.GetComponentInParent<PlayerController>();
        inv = GetComponentInChildren<PlayerInventory>();
    }

    private void UpdateCraftMenu()
    {
        float i = 0;
        foreach (Recipe rec in recipes)
        {
            RectTransform nen = Instantiate(entry, content);

            nen.Find("Name").GetComponent<Text>().text = rec.name;
            nen.Find("CraftButton").GetComponent<Button>().onClick.AddListener(() => { inv.CraftRecipe(rec); });

            nen.transform.position += Vector3.down * 70 * i;
            i++;
        }

        entry.Find("Name").GetComponent<Text>().text = "Name";
        entry.Find("CraftButton").GetComponent<Button>().onClick.AddListener(() => { });
    }

    private void Update()
    {
        if (menuCooldown > 0)
            menuCooldown -= Time.deltaTime;
        else
            menuCooldown = 0;

        if ((Input.GetKey(KeyCode.Menu) || Input.GetKey(openKey)) && menuCooldown == 0)
        {
            menuCooldown = 0.2f;
            bool hidden = craftingMenu.ToggleHide();

            if (hidden)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;
            Cursor.visible = !hidden;

            GameObject.Find("UI/CenterDot").GetComponent<Image>().color = hidden ? Color.white : Color.clear;

            pc.enabled = hidden;
        }
    }

}
