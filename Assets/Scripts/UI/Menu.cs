using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    CanvasGroup canv;

    [SerializeField]
    bool hidden = true;

    private void Awake()
    {
        canv = GetComponent<CanvasGroup>();
        if (hidden)
        {
            Hide();
        } else
        {
            Unhide();
        }
    }

    public bool ToggleHide()
    {
        hidden = !hidden;
        if (hidden)
        {
            Hide();
        }
        else
        {
            Unhide();
        }

        return hidden;
    }

    public void Hide()
    {
        canv.alpha = 0f;
        canv.blocksRaycasts = false;
    }

    public void Unhide()
    {
        canv.alpha = 1f;
        canv.blocksRaycasts = true;
    }
}
