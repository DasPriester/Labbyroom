using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private PlayerController player;
    private GameObject UI;

    [SerializeField] private List<Quest> quests = new List<Quest>();
    [SerializeField] private GameObject movePopUp;
    [SerializeField] private GameObject inventoryPopUp;
    [SerializeField] private GameObject finishPopUp;
    [SerializeField] private Recipe recipeToUnlock;
    private Coroutine currentCoroutine = null;
    private GameObject currentPopUp = null;
    private Quest currentQuest = null;
    private bool startedQuest = false;
    public int step = 0;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        UI = GameObject.Find("UI");

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Highlight"))
        {
            PickUpInteractable pui = go.GetComponent<PickUpInteractable>();
            if (pui != null)
            {
                pui.UseOutline = false;
                pui.GetComponent<Outline>().enabled = true;
                StartCoroutine(FlashingOutline(pui.GetComponent<Outline>(), 1f, 0f, 1f));
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
    }


    private void CollectRewards() { 
        StartCoroutine(FadeCanvas(UI.transform.Find("InventoryIcon").gameObject, 1f, 1f));
        recipeToUnlock.unlocked = true;

    }


    private void StartOpenUI() {
        UI.GetComponent<InventoryMenu>().UpdateCraftMenu();
        UI.GetComponent<InventoryMenu>().CloseCraftMenu();

        currentPopUp = Instantiate(inventoryPopUp);
        currentPopUp.transform.Find("Image/Text").GetComponent<UnityEngine.UI.Text>().text =
            "Open Inventory with \"" + player.settings.GetKey("Inventory").ToString() + "\"";
        StartCoroutine(FadeCanvas(currentPopUp, 1f, 1f));
    }
    private void CloseOpenUI() {
        StartCoroutine(FadeCanvas(currentPopUp, 1f, 0f));
        StartCoroutine(RemovePopUp(currentPopUp, 1f));
    }


    private void StartCraftUI()
    {
        currentCoroutine = StartCoroutine(FlashFirstRecipe());
        
    }
    private void CloseCraftUI()
    {
        currentPopUp = Instantiate(finishPopUp);
        StartCoroutine(FadeCanvas(currentPopUp, 0.025f, 1f));
        StartCoroutine(RemovePopUp(currentPopUp, 4f));
        StopCoroutine(currentCoroutine);
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
                pui.GetComponent<Outline>().enabled = true;
                StartCoroutine(FlashingOutline(pui.GetComponent<Outline>(), 1f, 0f, 1f));
            }
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
    private IEnumerator FadeOutline(Outline ol, float fadeDuration, float desiredAlpha)
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

    private IEnumerator FadeUIOutline(UnityEngine.UI.Outline ol, float fadeDuration, float desiredAlpha)
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

    private IEnumerator RemovePopUp(GameObject go, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(go);
        currentPopUp = null;
    }

    private IEnumerator FlashingOutline(Outline ol, float fadeDuration, float minAlpha, float maxAlpha)
    {
        while (true)
        {
            yield return StartCoroutine(FadeOutline(ol, fadeDuration, maxAlpha));
            yield return StartCoroutine(FadeOutline(ol, fadeDuration, minAlpha));

        }
    }

    private IEnumerator FlashFirstRecipe()
    {
        GameObject firstRecipe = UI.transform.Find("InventoryMenu(Clone)/BG/Scroll View/Viewport/Content/RecipeEntry(Clone)").gameObject;
        GameObject newRecipe = UI.transform.Find("InventoryMenu(Clone)/BG/Scroll View/Viewport/Content/RecipeEntry(Clone)").gameObject;

        Coroutine flashing = StartCoroutine(FlashingUIOutline(firstRecipe, 0.75f, 0f, 1f));
        firstRecipe.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            var ol = firstRecipe.GetComponent<UnityEngine.UI.Outline>();
            ol.effectColor = new Color(ol.effectColor.r, ol.effectColor.g, ol.effectColor.b, 0f);
            StopCoroutine(flashing);
        });

        while (true) { 
            newRecipe = UI.transform.Find("InventoryMenu(Clone)/BG/Scroll View/Viewport/Content/RecipeEntry(Clone)").gameObject;
            if (newRecipe && firstRecipe != newRecipe)
            {
                StopCoroutine(flashing);
                flashing = StartCoroutine(FlashingUIOutline(newRecipe, 0.75f, 0f, 1f));
                newRecipe.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() =>
                {
                    var ol = firstRecipe.GetComponent<UnityEngine.UI.Outline>();
                    ol.effectColor = new Color(ol.effectColor.r, ol.effectColor.g, ol.effectColor.b, 0f);
                    StopCoroutine(flashing);
                });
                firstRecipe = newRecipe;
            }
            yield return null;
        }
    }

    private IEnumerator FlashingUIOutline(GameObject go, float fadeDuration, float minAlpha, float maxAlpha)
    {
        go.GetComponent<UnityEngine.UI.Outline>().enabled = true;
        var ol = go.GetComponent<UnityEngine.UI.Outline>();
        while (true)
        {
            yield return StartCoroutine(FadeUIOutline(ol, fadeDuration, maxAlpha));
            yield return StartCoroutine(FadeUIOutline(ol, fadeDuration, minAlpha));

        }
    }
}
