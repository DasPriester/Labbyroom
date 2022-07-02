using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CraftingSubMenu : SubMenu
{
    [SerializeField] protected RectTransform recipeEntryPrefab = null;
    [SerializeField] protected RectTransform recipeCostPrefab = null;
    [SerializeField] protected AudioClip craftSound = null;
    protected bool isForge = false;
    protected RectTransform content = null;
    protected Recipe[] recipes;
    protected int entryPos = 0;
    protected int costPos = 0;
    protected Recipe currentRecipe = null;
    protected RectTransform currentEntry = null;

    public override void Awake()
    {
        base.Awake();
        content = transform.Find("Scroll BG/Viewport/Content").GetComponent<RectTransform>();
        recipes = Resources.LoadAll<Recipe>("Recipes");
        transform.Find("Craft").GetComponent<Button>().onClick.AddListener(() =>
        {
            if (inv.IsCraftable(currentRecipe))
            {
                StartCrafting();
                OpenMenu();
                MoveEntries(false);
            }
        });
    }
    protected abstract void StartCrafting();

    protected void MoveEntries(bool up)
    {
        bool found = false;
        for (int child = 0; child < content.transform.childCount; child++)
        {
            if (found)
                content.transform.GetChild(child).transform.position += (up ? -1 : 1) * 70 * costPos * Vector3.down;

            if (content.transform.GetChild(child) == currentEntry.transform)
                found = true;
        }
    }
    protected void CreateCostEntries(Recipe rec, RectTransform entry)
    {
        foreach (PickUpInteractable cost in rec.Cost.Keys)
        {
            RectTransform costEntry = Instantiate(recipeCostPrefab, entry);
            Item item = new Item
            {
                prefab = cost.gameObject,
                name = cost.name,
                amount = rec.Cost[cost]
            };
            costEntry.Find("Cost").GetComponent<Text>().color = inv.CanRemoveItem(item) ? Color.white : Color.gray;
            costEntry.Find("Cost").GetComponent<Text>().text = rec.Cost[cost] + "x " + cost.name;
            costEntry.Find("Image").GetComponent<Image>().sprite = Utility.GetIconFor(item);
            costPos++;
            costEntry.transform.position += 70 * costPos * Vector3.down;
        }
    }
    /// <summary>
    /// Updates visuals for crafting menu and each recipe
    /// </summary>
    public override void OpenMenu()
    {

        costPos = 0;
        entryPos = 0;

        // clear content
        foreach (RectTransform g in content.GetComponentsInChildren<RectTransform>())
            if (g != content)
                Destroy(g.gameObject);

        foreach (Recipe rec in recipes)
        {
            if ((rec.requiresForge == isForge) && (rec.unlocked || rec.alwaysUnlocked))
            {
                RectTransform entry = Instantiate(recipeEntryPrefab, content);
                entry.GetComponent<RecipeEntry>().Recipe = rec;

                // keep stuff after crafting
                if (currentRecipe == rec)
                {
                    currentEntry = entry;
                    CreateCostEntries(currentRecipe, currentEntry);
                }


                // set text and icon
                Item item = new Item();
                foreach (PickUpInteractable p in rec.Yield.Keys)
                {
                    entry.Find("Yield").GetComponent<Text>().text = rec.Yield[p] + "x " + p.name;
                    item.prefab = p.gameObject;
                    item.name = p.name;
                    break;
                }
                entry.Find("Image").GetComponent<Image>().sprite = Utility.GetIconFor(item);
                entry.Find("Yield").GetComponent<Text>().color = inv.IsCraftable(rec) ? Color.white : Color.gray;

                entry.Find("SelectButton").GetComponent<Button>().onClick.AddListener(() =>
                {
                    // close old crafting cost
                    if (currentEntry)
                    {
                        foreach (RectTransform rect in currentEntry)
                        {
                            if (rect.name == "RecipeCost(Clone)")
                                Destroy(rect.gameObject);
                        }
                        currentEntry.GetComponent<Image>().color = Color.gray;
                        MoveEntries(true);
                        costPos = 0;
                    }

                    // reset if clicked second time
                    if (currentRecipe == rec)
                    {

                        content.sizeDelta = new Vector2(0, 70 * costPos + 120 * entryPos);
                        currentEntry.GetComponent<Image>().color = Color.gray;
                        currentRecipe = null;
                        currentEntry = null;
                        costPos = 0;
                        return;
                    }

                    currentRecipe = rec;
                    currentEntry = entry;

                    // create cost entries
                    CreateCostEntries(currentRecipe, currentEntry);

                    // move entries down
                    MoveEntries(false);

                    content.sizeDelta = new Vector2(0, 70 * costPos + 120 * entryPos);
                });

                entry.transform.position += 120 * entryPos * Vector3.down;
                entryPos++;
            }
        }

        content.sizeDelta = new Vector2(0, 70 * costPos + 120 * entryPos);

        if (entryPos > 0)
            transform.Find("Scroll BG/NoContent").GetComponent<Text>().text = "";
        else
            transform.Find("Scroll BG/NoContent").GetComponent<Text>().text = "You have to discover a recipe first...";
    }
    public override void CloseMenu()
    {
        currentRecipe = null;
        currentEntry = null;
    }
}
