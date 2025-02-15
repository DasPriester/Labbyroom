using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class ItemUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;


    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector2 oldPosition;
    private Transform oldParent;

    public Item Item { get; set; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        oldPosition = rectTransform.anchoredPosition;
        oldParent = rectTransform.parent;
        rectTransform.SetParent(GameObject.Find("UI").transform);
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        GameObject target = eventData.pointerCurrentRaycast.gameObject;

        if (!target || !(target.GetComponent<ItemSlot>() || target.GetComponent<ItemUI>()))
        {
            rectTransform.anchoredPosition = oldPosition;
            rectTransform.SetParent(oldParent, false);
        }
        else if (target.GetComponent<ItemSlot>())
        {
            InventoryMenu menu = GameObject.Find("UI").GetComponent<InventoryMenu>();
            menu.DroppedOff(this, target);
            rectTransform.SetParent(target.transform.parent.parent, false);
        }
        else if (target.GetComponent<ItemUI>())
        {
            InventoryMenu menu = GameObject.Find("UI").GetComponent<InventoryMenu>();
            menu.DroppedOff(this, target.GetComponent<ItemUI>());


            rectTransform.SetParent(target.transform.parent, false);
            target.transform.SetParent(oldParent, false);

            rectTransform.anchoredPosition = target.GetComponent<RectTransform>().anchoredPosition;
            target.GetComponent<RectTransform>().anchoredPosition = oldPosition;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {

    }
}
