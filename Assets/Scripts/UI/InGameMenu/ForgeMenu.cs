using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ForgeMenu : CraftingSubMenu
{
    [SerializeField] private GameObject itemPrefab = null;
    private bool forging = false;

    public override void Awake()
    {
        base.Awake();
        isForge = true;
    }

    protected override void StartCrafting()
    {
        StartCoroutine(Forging(currentRecipe, 2f));
    }

    private void Update()
    {
        Transform craftButton = transform.Find("Craft");
        if (inv.IsCraftable(currentRecipe) && !forging)
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

    private IEnumerator Forging(Recipe recipe, float duration)
    {
        transform.GetComponent<AudioSource>().clip = craftSound;
        transform.GetComponent<AudioSource>().Play();
        Image arrow = transform.Find("Arrow FG").GetComponent<Image>();
        float progress = 0f;
        forging = true;
        CreateForgingItemUI(recipe);

        while (progress < 1f)
        {
            arrow.fillAmount = progress;
            progress = Mathf.MoveTowards(progress, 1f, Time.deltaTime / duration);
            yield return null;
        }

        RemoveForgingItemUI(recipe);
        arrow.fillAmount = 0f;
        forging = false;
        inv.CraftRecipe(recipe);
    }

    private void CreateForgingItemUI(Recipe recipe)
    {  
        foreach (PickUpInteractable cost in recipe.Cost.Keys)
        {
            Item item = new Item
            {
                prefab = cost.gameObject,
                name = cost.name,
                amount = recipe.Cost[cost]
            };

            inv.RemoveItem(item);
            GameObject itemUI = Instantiate(itemPrefab, transform.Find("LeftSlot"));
            itemUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(50, -50);
            itemUI.GetComponent<ItemUI>().Item = item;
            itemUI.GetComponent<Image>().sprite = Utility.GetIconFor(item);
            itemUI.GetComponentInChildren<Text>().text = item.amount.ToString();
            break;
        }
        foreach (PickUpInteractable cost in recipe.Yield.Keys)
        {
            Item item = new Item
            {
                prefab = cost.gameObject,
                name = cost.name,
                amount = recipe.Yield[cost]
            };

            GameObject itemUI = Instantiate(itemPrefab, transform.Find("RightSlot"));
            itemUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(50, -50);
            itemUI.GetComponent<ItemUI>().Item = item;
            itemUI.GetComponent<Image>().sprite = Utility.GetIconFor(item);
            itemUI.GetComponentInChildren<Text>().text = item.amount.ToString();
            break;
        }
    }
    private void RemoveForgingItemUI(Recipe recipe)
    {
        
        foreach(Transform t in transform.Find("LeftSlot"))
        {
            Destroy(t.gameObject);
        }
        foreach(Transform t in transform.Find("RightSlot"))
        {
            Destroy(t.gameObject);
        }
        foreach (PickUpInteractable cost in recipe.Cost.Keys)
        {
            Item item = new Item
            {
                prefab = cost.gameObject,
                name = cost.name,
                amount = recipe.Cost[cost]
            };

            inv.AddItem(item);
        }
    }

    public override void CloseMenu()
    {

        InventoryMenu inventoryMenu = GameObject.Find("UI").GetComponent<InventoryMenu>();
        inventoryMenu.SwitchMenu("CraftingMenu");
        inventoryMenu.GetInventoryMenu().openKey = inventoryMenu.oldKey;
        currentRecipe = null;
        currentEntry = null;
    }
}
