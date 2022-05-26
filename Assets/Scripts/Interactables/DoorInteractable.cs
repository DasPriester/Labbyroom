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
    public bool blocked = false;

    /// <summary>
    /// Fetch portal from animator 
    /// </summary>
    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = doorSound;
        portal = doorAnimator.GetComponentInParent<PortalComponent>();
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
                if (door2Animator)
                    audioSource2.Play();
            }
        }

        if (!blocked)
        {
            doorAnimator.SetTrigger("ToggleTrigger");
            if (door2Animator)
                door2Animator.SetTrigger("ToggleTrigger");
        }
        
    }

    /// <summary>
    /// Enable outline if not blocked
    /// </summary>
    public override void OnFocus()
    {
        if (UseOutline && !blocked)
            gameObject.GetComponent<Outline>().enabled = true;

        if (UseToolTip && toolTip && !blocked)
            toolTip.Unhide();
    }
}
