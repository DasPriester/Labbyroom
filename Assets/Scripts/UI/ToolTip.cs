using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    [SerializeField] public KeyCode key = default;
    [SerializeField] public bool hidden = true;
    CanvasGroup canv;

    private void Awake()
    {
        GameObject.Find("Key").GetComponent<Text>().text = key.ToString();

        canv = GetComponent<CanvasGroup>();
        if (hidden)
        {
            Hide();
        }
        else
        {
            Unhide();
        }
    }

    private void Update()
    {
        transform.LookAt(Camera.main.transform.position);
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
    }

    public void Unhide()
    {
        canv.alpha = 1f;
    }
}
