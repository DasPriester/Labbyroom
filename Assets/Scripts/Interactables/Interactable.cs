using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
/// <summary>
/// base class from which each Interactable Object derives from.
/// </summary>
public abstract class Interactable : MonoBehaviour
{
    [Header("Juiciness")]
    public bool UseAudio = true;
    public bool UseOutline = true;
    public bool UseParticle = true;
    public bool UseAnimation = true;
    public bool UseToolTip = true;

    protected ToolTip toolTip;
    protected PlayerController pc;

    public virtual void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if(UseToolTip)
            toolTip = GetComponentInChildren<ToolTip>();
    }

    private void Update()
    {
        if (toolTip)
            toolTip.Key = pc.settings.interactKey;
    }

    /// <summary>
    /// Called if the player uses the interact key while targeting the object.
    /// </summary>
    /// <param name="pos">Location at which the hit occured</param>
    public abstract void OnInteract(Vector3 pos);

   
    /// <summary>
    /// Called once if the player starts focusing the object.
    /// </summary>
    public virtual void OnFocus()
    {
        if(UseOutline)
            gameObject.GetComponent<Outline>().enabled = true;
        if(UseToolTip)
            toolTip.Unhide();
    }

    /// <summary>
    /// Called once if the player stops focusing the object.
    /// </summary>
    public virtual void OnLoseFcous()
    {
        if (UseOutline)
            gameObject.GetComponent<Outline>().enabled = false;
        if (UseToolTip)
            toolTip.Hide();
    }
}
