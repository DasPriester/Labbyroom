using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingMenu : CraftingSubMenu
{
    protected override void StartCrafting()
    {
        inv.CraftRecipe(currentRecipe);
        transform.GetComponent<AudioSource>().clip = craftSound;
        transform.GetComponent<AudioSource>().Play();
    }

    private void Update()
    {
        Transform craftButton = transform.Find("Craft");
        if (inv.IsCraftable(currentRecipe))
        {
            craftButton.GetComponent<Image>().color = Color.green;
            craftButton.GetComponent<Button>().enabled = true;
        }
        else
        {
            craftButton.GetComponent<Image>().color = Color.gray;
            craftButton.GetComponent<Button>().enabled = false;
        }
        if (currentEntry)
            currentEntry.GetComponent<Image>().color = Color.white;
    }
}
