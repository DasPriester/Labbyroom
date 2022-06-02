using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class for all hideable UI elements
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class Hideable : MonoBehaviour
{
    [SerializeField] protected bool hidden = true;

    protected CanvasGroup canv;

    public bool Hidden
    {
        
        get { return hidden; }
        set { 
            hidden = value;
            UpdateCanvas();
        }
    }

    private void Start()
    {
        canv = GetComponent<CanvasGroup>();
        UpdateCanvas();
    }

    /// <summary>
    /// Update visibilty of canvas group
    /// </summary>
    private void UpdateCanvas()
    {
        if (hidden)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }
    
    /// <summary>
    /// Hide canvas group
    /// </summary>
    public void Hide()
    {   
        if(!canv)
            canv = GetComponent<CanvasGroup>();
        canv.alpha = 0f;
        canv.blocksRaycasts = false;   
    }

    /// <summary>
    /// Show canvas group
    /// </summary>
    public void Show()
    {
        if (!canv)
            canv = GetComponent<CanvasGroup>();
        canv.alpha = 1f;
        canv.blocksRaycasts = true; 
    }
}
