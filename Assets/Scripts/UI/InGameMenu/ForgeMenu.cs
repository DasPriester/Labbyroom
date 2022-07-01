using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ForgeMenu : SubMenu
{
    [SerializeField] private RectTransform recipeEntryPrefab = null;
    [SerializeField] private RectTransform recipeCostPrefab = null;
    [SerializeField] private GameObject itemPrefab = null;
    [SerializeField] private AudioClip audioClip;
    private RectTransform content = null;
    private Recipe[] recipes;
    private Recipe currentRecipe = null;
    private RectTransform currentEntry = null;
    private int entryPos = 0;
    private int costPos = 0;
    private bool forging = false;

    public override void Awake()
    {
        inv = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().GetComponent<Inventory>();

        content = transform.Find("Scroll BG/Viewport/Content").GetComponent<RectTransform>();
        recipes = Resources.LoadAll<Recipe>("Recipes");
        transform.Find("Craft").GetComponent<Button>().onClick.AddListener(() =>
        {
            if (inv.IsCraftable(currentRecipe))
            {
                StartCoroutine(Forging(currentRecipe, 2f));
            }
        });
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
        transform.GetComponent<AudioSource>().clip = audioClip;
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
        foreach (PickUpInteractable cost in recipe.Yield.Keys)
        {
            Item item = new Item
            {
                prefab = cost.gameObject,
                name = cost.name,
                amount = recipe.Yield[cost]
            };

            inv.AddItem(item);
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
            if (rec.requiresForge && (rec.unlocked || rec.alwaysUnlocked))
            {
                RectTransform entry = Instantiate(recipeEntryPrefab, content);
                entry.GetComponent<RecipeEntry>().Recipe = rec;

                // keep stuff after crafting
                if (currentRecipe == rec)
                {
                    currentEntry = entry;
                    CreateCostEntries();
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

                // local function to move entries below 
                void MoveEntries(bool up)
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

                // local function to create Cost entries
                void CreateCostEntries()
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
                        costEntry.Find("Cost").GetComponent<Text>().color =
                            inv.CanRemoveItem(item) ?
                            Color.white :
                            Color.gray;
                        costEntry.Find("Cost").GetComponent<Text>().text = rec.Cost[cost] + "x " + cost.name;
                        costEntry.Find("Image").GetComponent<Image>().sprite = Utility.GetIconFor(item);
                        costPos++;
                        costEntry.transform.position += 70 * costPos * Vector3.down;
                    }
                }

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
                        currentEntry.GetComponent<Image>().color = Color.gray;
                        currentRecipe = null;
                        currentEntry = null;
                        costPos = 0;
                        entryPos = 0;
                        return;
                    }

                    currentRecipe = rec;
                    currentEntry = entry;

                    // create cost entries
                    CreateCostEntries();

                    // move entries down
                    MoveEntries(false);

                });

                entry.transform.position += 120 * entryPos * Vector3.down;
                entryPos++;
            }
        }

        if (entryPos > 0)
            transform.Find("Scroll BG/NoContent").GetComponent<Text>().text = "";
        else
            transform.Find("Scroll BG/NoContent").GetComponent<Text>().text = "You have to discover a recipe first...";
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
