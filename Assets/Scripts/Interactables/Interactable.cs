using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(QuickOutline))]
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

        if (UseToolTip)
        {
            toolTip = GetComponentInChildren<ToolTip>();
        }
    }


    /// <summary>
    /// Called if the player uses the interact key while targeting the object.
    /// </summary>
    /// <param name="pos">Location at which the hit occured</param>
    public virtual void OnInteract(Vector3 pos) { }
    
    /// <summary>
    /// Called if the player uses the build key while targeting the object.
    /// </summary>
    /// <param name="pos">Location at which the hit occured</param>
    public virtual void OnBuild(Vector3 pos) { }
    
    /// <summary>
    /// Called if the player uses the pickup key while targeting the object.
    /// </summary>
    /// <param name="pos">Location at which the hit occured</param>
    public virtual void OnPickUp(Vector3 pos) { }

   
    /// <summary>
    /// Called once if the player starts focusing the object.
    /// </summary>
    public virtual void OnFocus()
    {
        if(UseOutline)
            gameObject.GetComponent<QuickOutline>().enabled = true;
        if(UseToolTip)
            toolTip.Show();
    }

    /// <summary>
    /// Called once if the player stops focusing the object.
    /// </summary>
    public virtual void OnLoseFcous()
    {
        if (UseOutline)
            gameObject.GetComponent<QuickOutline>().enabled = false;
        if (UseToolTip)
            toolTip.Hide();
    }
}
