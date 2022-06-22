using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private PlayerController player;
    private GameObject UI;

    [SerializeField] private List<GameObject> popUps = new List<GameObject>();
    [SerializeField] private List<Quest> quests = new List<Quest>();
    [SerializeField] private PickUpInteractable[] highlightedObjects;
    [SerializeField] private Recipe recipeToUnlock;
    private GameObject currentPopUp = null;
    private Quest currentQuest = null;
    private int step = 0;
    private int popUpStep = 0;

    private bool blinkingRecipe = true;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        UI = GameObject.Find("UI");


        currentPopUp = Instantiate(popUps[popUpStep]);
        currentQuest = quests[step];

        foreach (PickUpInteractable pui in highlightedObjects)
        {
            if (pui != null)
            {
                pui.UseOutline = false;
                pui.GetComponent<Outline>().enabled = true;
                StartCoroutine(FlashingOutline(pui.GetComponent<Outline>(), 1f, 0f, 1f));
            }
        }
    }

    private void Update()
    {

        if (currentQuest.IsDone())
        {
            switch (step)
            {

                case 0: // moved
                    StartCoroutine(FadeCanvas(currentPopUp, 2f, 0f));
                    StartCoroutine(DestoryAfterTime(currentPopUp, 2f));

                    StartCoroutine(FadeCanvas(UI.transform.Find("Hotbar BG").gameObject, 2f, 1f));
                    player.canMove = true;
                    player.canSprint = true;
                    player.canInteract = true;
                    break;

                case 1: // collected sticks
                    popUpStep++;
                    currentPopUp = Instantiate(popUps[popUpStep]);
                    StartCoroutine(FadeCanvas(currentPopUp, 1f, 1f));
                    StartCoroutine(FadeCanvas(UI.transform.Find("InventoryIcon").gameObject, 1f, 1f));
                    recipeToUnlock.unlocked = true;
                    break;

                case 2: // opened inventory
                    StartCoroutine(FadeCanvas(currentPopUp, 1f, 0f));
                    StartCoroutine(DestoryAfterTime(currentPopUp, 1f));
                    GameObject firstRecipe = UI.transform.Find("InventoryMenu(Clone)/BG/Scroll View/Viewport/Content/RecipeEntry(Clone)").gameObject;
                    firstRecipe.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() => blinkingRecipe = false);
                    StartCoroutine(FlashingUIOutline(firstRecipe, 0.75f, 0f, 1f));
                    break;

                case 3: // crafted key
                    popUpStep++;
                    currentPopUp = Instantiate(popUps[popUpStep]);
                    StartCoroutine(FadeCanvas(currentPopUp, 0.025f, 1f));
                    StartCoroutine(DestoryAfterTime(currentPopUp, 4f));
                    break;

            }

            step++;
            if (step < quests.Count)
                currentQuest = quests[step];
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

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            currentAlpha = Mathf.MoveTowards(currentAlpha, desiredAlpha, Time.deltaTime / fadeDuration);
            ol.effectColor = new Color(ol.effectColor.r, ol.effectColor.g, ol.effectColor.b, currentAlpha);

            yield return null;
        }
    }

    private IEnumerator DestoryAfterTime(GameObject go, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(go);
    }

    private IEnumerator FlashingOutline(Outline ol, float fadeDuration, float minAlpha, float maxAlpha)
    {
        while (true)
        {
            yield return StartCoroutine(FadeOutline(ol, fadeDuration, maxAlpha));
            yield return StartCoroutine(FadeOutline(ol, fadeDuration, minAlpha));

        }
    }
    private IEnumerator FlashingUIOutline(GameObject go, float fadeDuration, float minAlpha, float maxAlpha)
    {
        go.GetComponent<UnityEngine.UI.Outline>().enabled = true;
        var ol = go.GetComponent<UnityEngine.UI.Outline>();
        while (blinkingRecipe)
        {
            yield return StartCoroutine(FadeUIOutline(ol, fadeDuration, maxAlpha));
            yield return StartCoroutine(FadeUIOutline(ol, fadeDuration, minAlpha));

        }
    }
}
