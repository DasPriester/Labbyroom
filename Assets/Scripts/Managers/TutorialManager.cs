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
    [SerializeField] private AudioClip completedClip;
    [SerializeField] private AudioClip acceptedClip;
    private Coroutine currentCoroutine = null;
    private GameObject currentPopUp = null;
    private Quest currentQuest = null;
    private bool startedQuest = false;
    private InGameMenu questMenu;
    private InGameMenu explanationMenu;
    public int step = 0;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        UI = GameObject.Find("UI");
        questMenu = player.settings.GetMenu("QuestMenu");
        explanationMenu = player.settings.GetMenu("ExplanationMenu");
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Highlight"))
        {
            if (go.TryGetComponent<PickUpInteractable>(out var pui))
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
        quests[3].Rewards = CraftRewards;

        quests[4].Explanations[2] = "To create a temporary room equip your key and move towards a wall. Click the " +
            Utility.GetKeyName(player.settings.buildKey)
            + " to place a door.";
        quests[5].Explanations[0] = "Place down objects and furniture with " +
            Utility.GetKeyName(player.settings.buildKey)
            + ".";
        for (int i = 4; i < quests.Count; i++)
        {
            quests[i].CloseUI = QuestFinished;
        }

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

        UI.transform.Find("Quest BG/Task").GetComponent<Text>().text = quests[step].Progress();
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
    private void CraftRewards()
    {
        player.canBuild = true;
    }


    private void QuestFinished()
    {
        if(InGameMenu.instance != null)
            UI.GetComponent<InventoryMenu>().GetInventoryMenu().ToggleMenu();
        StartCoroutine(FadeCanvas(UI.transform.Find("Quest BG").gameObject, 1f, 0f));

        player.GetComponent<AudioSource>().clip = completedClip;
        player.GetComponent<AudioSource>().Play();

        Quest quest = currentQuest;
        Quest nextQuest = quests[step + 1];
        questMenu.transform.Find("BG/RewardIcon").GetComponent<Image>().sprite = Utility.GetIconFor(quest.Reward);
        questMenu.transform.Find("BG/RewardName").GetComponent<Text>().text = quest.Reward.name;

        questMenu.transform.Find("BG/Description").GetComponent<Text>().text = nextQuest.Description;
        questMenu.transform.Find("BG/PreviewIcon").GetComponent<Image>().sprite = Utility.GetIconFor(new Item { name = nextQuest.Reward.name });
        questMenu.transform.Find("BG/Button").GetComponent<Button>().onClick.RemoveAllListeners();
        questMenu.transform.Find("BG/Button").GetComponent<Button>().onClick.AddListener(() =>
        {
            RecipeInteractable ri = quest.Reward.prefab.GetComponent<RecipeInteractable>();
            if (ri)
                ri.Recipe.unlocked = true;
            else
                player.GetComponent<Inventory>().AddItem(quest.Reward);
            player.GetComponent<AudioSource>().clip = acceptedClip;
            player.GetComponent<AudioSource>().Play();

            StartCoroutine(FadeCanvas(UI.transform.Find("Quest BG").gameObject, 1f, 1f));
            UI.transform.Find("Quest BG/Name").GetComponent<Text>().text = nextQuest.Name;
            UI.transform.Find("Quest BG/Description").GetComponent<Text>().text = nextQuest.Description;

            questMenu.ToggleMenu();
            InGameMenu.instance = null;

            if (nextQuest.NeedsExplanation)
            {
                explanationMenu.ToggleMenu();
                InGameMenu.instance = explanationMenu;
                int pos = 0;
                explanationMenu.transform.Find("BG/Text").GetComponent<Text>().text = nextQuest.Explanations[pos++];
                explanationMenu.transform.Find("BG/Button").GetComponent<Button>().onClick.RemoveAllListeners();
                explanationMenu.transform.Find("BG/Button").GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (pos == nextQuest.Explanations.Count - 1)
                        explanationMenu.transform.Find("BG/Button/Text").GetComponent<Text>().text = "Got It";
                    else if (pos == nextQuest.Explanations.Count)
                    {
                        explanationMenu.ToggleMenu();
                        InGameMenu.instance = null;
                        return;
                    }
                    explanationMenu.transform.Find("BG/Text").GetComponent<Text>().text = nextQuest.Explanations[pos++];
                });
            }
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
            if (i < content.childCount)
            {
                newRecipe = content.GetChild(i)?.gameObject;
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
