using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// interactable portal door 
/// </summary>
public class DoorInteractable : Interactable
{
    [SerializeField] private AudioClip doorSound = default;
    [SerializeField] private Animator doorAnimator = default;

    private Animator door2Animator = null;
    private PortalComponent portal;
    private AudioSource audioSource;
    private AudioSource audioSource2;
    private bool blocked = false;
    private bool focused = false;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = doorSound;
        portal = doorAnimator.GetComponentInParent<PortalComponent>();


        toolTip.SetText(Utility.GetKeyName(pc.settings.interactKey));
    }

    public void Update()
    {
        blocked = (doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("Opening") || doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("Closing"));
        if (!blocked && focused)
        {
            if (UseOutline && !blocked)
                gameObject.GetComponent<QuickOutline>().enabled = true;

            if (UseToolTip && toolTip && !blocked)
                toolTip.Show();
        }
        else
        {
            if (UseOutline)
                gameObject.GetComponent<QuickOutline>().enabled = false;
            if (UseToolTip)
                toolTip.Hide();
        }
        
    }

    /// <summary>
    /// Update portal connection after summoning a new one
    /// </summary>
    public void UpdateConnection()
    {
        portal = doorAnimator.GetComponentInParent<PortalComponent>();
        door2Animator = portal.linkedPortal.GetComponentInChildren<Animator>();
        audioSource2 = portal.linkedPortal.GetComponentInChildren<AudioSource>();
        audioSource2.clip = doorSound;
    }

    /// <summary>
    /// Play sounds and start open/close animaton
    /// </summary>
    public override void OnInteract(Vector3 hit)
    {
        if (!blocked)
        {
            if (UseAudio)
            {
                audioSource.Play();
                audioSource2.Play();
            }

            doorAnimator.SetTrigger("ToggleTrigger");
            door2Animator.SetTrigger("ToggleTrigger");
        } 
    }

    /// <summary>
    /// Enable outline if not blocked
    /// </summary>
    public override void OnFocus()
    {
        focused = true;
        if (UseOutline && !blocked)
            gameObject.GetComponent<QuickOutline>().enabled = true;

        if (UseToolTip && toolTip && !blocked)
            toolTip.Show();
    }

    /// <summary>
    /// Enable outline if not blocked
    /// </summary>
    public override void OnLoseFcous()
    {
        focused = false;
        if (UseOutline)
            gameObject.GetComponent<QuickOutline>().enabled = false;
        if (UseToolTip)
            toolTip.Hide();
    }
}
