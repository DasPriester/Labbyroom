using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    private PlayerController player;
    private GameObject UI;

    [SerializeField] private List<Quest> quests = new List<Quest>();
    [SerializeField] private GameObject movePopUp;
    [SerializeField] private GameObject inventoryPopUp;
    [SerializeField] private Recipe keyRecipe;
    private Coroutine currentCoroutine = null;
    private GameObject currentPopUp = null;
    private Quest currentQuest = null;
    private bool startedQuest = false;
    private InGameMenu questMenu;
    public int step = 0;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        UI = GameObject.Find("UI");
        questMenu = player.settings.GetMenu("QuestMenu");
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Highlight"))
        {
            PickUpInteractable pui = go.GetComponent<PickUpInteractable>();
            if (pui != null)
            {
                pui.UseOutline = false;
                pui.GetComponent<QuickOutline>().enabled = true;
                StartCoroutine(FlashingOutline(pui.GetComponent<QuickOutline>(), 1f, 0f, 1f));
            }
        }
        
        quests[0].StartUI = StartMoveUI;
        quests[0].CloseUI = CloseMoveUI;
        quests[0].Rewards = MoveRewards;

        quests[1].Rewards = CollectRewards;

        quests[2].StartUI = StartOpenUI;
        quests[2].CloseUI = CloseOpenUI;
        quests[2].Task = new PressButton(new List<KeyCode>() { player.settings.GetKey("Inventory") });

        quests[3].StartUI = StartCraftUI;
        quests[3].CloseUI = CloseCraftUI;
        quests[3].Rewards = CraftReward;

        currentQuest = quests[step];

    }

    private void Update()
    {
        if (!startedQuest)
        {
            currentQuest.StartUI?.Invoke();
            startedQuest = true;
        }

        if (currentQuest && currentQuest.IsDone())
        {
            currentQuest.CloseUI?.Invoke();
            currentQuest.Rewards?.Invoke();

            startedQuest = false;
            step++;
            if (step < quests.Count)
                currentQuest = quests[step];
            else
            {
                currentQuest = null;
                startedQuest = true;
            }
        }
    }

    private void StartMoveUI()
    {
        currentPopUp = Instantiate(movePopUp);
        currentQuest = quests[step]; 
    }
    private void CloseMoveUI() {
        StartCoroutine(FadeCanvas(currentPopUp, 1.5f, 0f));
        StartCoroutine(RemovePopUp(currentPopUp, 1.5f));
    }
    private void MoveRewards() { 
        StartCoroutine(FadeCanvas(UI.transform.Find("Hotbar BG").gameObject, 1.5f, 1f));
        player.canMove = true;
        player.canSprint = true;
        player.canInteract = true;
        player.canPickUp = true;
        player.canBuild = true;
    }


    private void CollectRewards() { 
        StartCoroutine(FadeCanvas(UI.transform.Find("InventoryIcon").gameObject, 1f, 1f));
        keyRecipe.unlocked = true;
    }


    private void StartOpenUI() {
        UI.GetComponent<InventoryMenu>().OpenInventoryMenu();
        UI.GetComponent<InventoryMenu>().CloseInventoryMenu();

        currentPopUp = Instantiate(inventoryPopUp);
        currentPopUp.transform.Find("Image/Text").GetComponent<Text>().text =
            "Open Inventory with \"" + player.settings.GetKey("Inventory").ToString() + "\"";
        StartCoroutine(FadeCanvas(currentPopUp, 1f, 1f));
    }
    private void CloseOpenUI() {
        StartCoroutine(FadeCanvas(currentPopUp, 1f, 0f));
        StartCoroutine(RemovePopUp(currentPopUp, 1f));
    }


    private void StartCraftUI() {
        currentCoroutine = StartCoroutine(FlashRecipe(keyRecipe));
    }
    private void CloseCraftUI()
    {
        StopCoroutine(currentCoroutine);
        QuestFinished();

    }
    private void CraftReward()
    {
        keyRecipe.unlocked = false;
    }


    public void QuestFinished()
    {
        if(InGameMenu.instance != null)
            UI.GetComponent<InventoryMenu>().GetInventoryMenu().ToggleMenu();

        Quest quest = quests[step];
        questMenu.transform.Find("BG/RewardIcon").GetComponent<Image>().sprite = Utility.GetIconFor(quest.Reward);
        questMenu.transform.Find("BG/RewardName").GetComponent<Text>().text = quest.Reward.name;

        questMenu.transform.Find("BG/Description").GetComponent<Text>().text = quests[step+1].Description;
        questMenu.transform.Find("BG/PreviewIcon").GetComponent<Image>().sprite = Utility.GetIconFor(new Item { name = quests[step+1].Reward.name });
        questMenu.transform.Find("BG/Button").GetComponent<Button>().onClick.AddListener(() =>
        {
            RecipeInteractable ri = quest.Reward.prefab.GetComponent<RecipeInteractable>();
            if (ri)
                ri.Recipe.unlocked = true;
            else
                player.GetComponent<Inventory>().AddItem(quest.Reward);
            questMenu.ToggleMenu();
            InGameMenu.instance = null;
        });


        questMenu.ToggleMenu();
        InGameMenu.instance = questMenu;
    }

    public void Refresh()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        UI = GameObject.Find("UI");


        if (step < quests.Count)
            currentQuest = quests[step];
        else
        {
            currentQuest = null;
            startedQuest = true;
        }

        for (int i = 0; i < step; i++)
        {
            quests[i].Rewards?.Invoke();
        }
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Highlight"))
        {
            PickUpInteractable pui = go.GetComponent<PickUpInteractable>();
            if (pui != null)
            {
                pui.UseOutline = false;
                pui.GetComponent<QuickOutline>().enabled = true;
                StartCoroutine(FlashingOutline(pui.GetComponent<QuickOutline>(), 1f, 0f, 1f));
            }
        }
    }
    private IEnumerator FlashRecipe(Recipe recipe)
    {
        Transform content = UI.transform.Find("InventoryMenu(Clone)/CraftingMenu(Clone)/Scroll BG/Viewport/Content");

        int i = 0;
        foreach(Transform child in content)
        {
            if (child.GetComponent<RecipeEntry>().Recipe == recipe)
                break;
            i++;
        }
        GameObject oldRecipe = content.GetChild(i).gameObject;
        GameObject newRecipe;

        Coroutine flashing = StartCoroutine(FlashingUIOutline(oldRecipe, 0.75f, 0f, 1f));
        oldRecipe.GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
            StopCoroutine(flashing);
            var ol = oldRecipe.GetComponent<Outline>();
            StartCoroutine(FadeUIOutline(ol, 0.75f, 0f));

        });

        while (true)
        {
            newRecipe = content.GetChild(i).gameObject;
            if (newRecipe && oldRecipe != newRecipe)
            {
                StopCoroutine(flashing);
                flashing = StartCoroutine(FlashingUIOutline(newRecipe, 0.75f, 0f, 1f));
                newRecipe.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    StopCoroutine(flashing);
                    var ol = oldRecipe.GetComponent<Outline>();
                    StartCoroutine(FadeUIOutline(ol, 0.75f, 0f));

                });
                oldRecipe = newRecipe;
            }
            yield return null;
        }
    }
    private IEnumerator FadeCanvas(GameObject uiElement, float fadeDuration, float desiredAlpha)
    {
        CanvasGroup cg = uiElement.GetComponent<CanvasGroup>();
        float currentAlpha = cg.alpha;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            currentAlpha = Mathf.MoveTowards(currentAlpha, desiredAlpha, Time.deltaTime / fadeDuration);
            cg.alpha = currentAlpha;

            yield return null;
        }
    }
    private IEnumerator FadeOutline(QuickOutline ol, float fadeDuration, float desiredAlpha)
    {
        float currentAlpha = ol.OutlineColor.a;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            currentAlpha = Mathf.MoveTowards(currentAlpha, desiredAlpha, Time.deltaTime / fadeDuration);
            ol.OutlineColor = new Color(ol.OutlineColor.r, ol.OutlineColor.g, ol.OutlineColor.b, currentAlpha);

            yield return null;
        }
    }
    private IEnumerator FadeUIOutline(Outline ol, float fadeDuration, float desiredAlpha)
    {

        float currentAlpha = ol.effectColor.a;

        float elapsedTime = 0f;

        while (ol && elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            currentAlpha = Mathf.MoveTowards(currentAlpha, desiredAlpha, Time.deltaTime / fadeDuration);
            ol.effectColor = new Color(ol.effectColor.r, ol.effectColor.g, ol.effectColor.b, currentAlpha);

            yield return null;
        }
    }
    private IEnumerator FlashingOutline(QuickOutline ol, float fadeDuration, float minAlpha, float maxAlpha)
    {
        while (true)
        {
            yield return StartCoroutine(FadeOutline(ol, fadeDuration, maxAlpha));
            yield return StartCoroutine(FadeOutline(ol, fadeDuration, minAlpha));

        }
    }

    private IEnumerator FlashingUIOutline(GameObject go, float fadeDuration, float minAlpha, float maxAlpha)
    {
        go.GetComponent<Outline>().enabled = true;
        var ol = go.GetComponent<Outline>();
        while (true)
        {
            yield return StartCoroutine(FadeUIOutline(ol, fadeDuration, maxAlpha));
            yield return StartCoroutine(FadeUIOutline(ol, fadeDuration, minAlpha));

        }
    }

    private IEnumerator RemovePopUp(GameObject go, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(go);
        currentPopUp = null;
    }
}
