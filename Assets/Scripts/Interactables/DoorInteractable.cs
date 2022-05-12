using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorInteractable : Interactable
{
    [SerializeField] private AudioClip doorSound = default;
    [SerializeField] private Animator doorAnimator = default;

    private Animator door2Animator = null;
    private PortalComponent other;
    public bool blocked = false;

    public override void Awake()
    {
        gameObject.layer = 6;

        if (UseToolTip && !blocked)
        {
            toolTip = GetComponentInChildren<ToolTip>();

            if (toolTip)
            {
                toolTip.key = Camera.main.GetComponentInParent<PlayerController>().interactKey;
                RectTransform text = toolTip.GetComponentInChildren<Text>().GetComponent<RectTransform>();
                text.transform.localScale = new Vector3(-text.transform.localScale.x, text.transform.localScale.y, text.transform.localScale.z);
            }
        }

        other = doorAnimator.GetComponentInParent<PortalComponent>();
    }

    public void UpdateConnection()
    {
        other = doorAnimator.GetComponentInParent<PortalComponent>();
        door2Animator = other.linkedPortal.GetComponentInChildren<Animator>();
    }

    public override void OnInteract(Vector3 pos)
    {

        if (UseAudio && !blocked)
        {
            AudioSource.PlayClipAtPoint(doorSound, doorAnimator.gameObject.transform.position);
            if (door2Animator)
                AudioSource.PlayClipAtPoint(doorSound, door2Animator.gameObject.transform.position);
        }

        if (!blocked)
        {
            doorAnimator.SetTrigger("ToggleTrigger");
            if (door2Animator)
                door2Animator.SetTrigger("ToggleTrigger");
        }
        
    }

    public override void OnFocus(Vector3 pos)
    {
        if (UseOutline && !blocked)
            gameObject.GetComponent<Outline>().enabled = true;

        if (toolTip && UseToolTip && !blocked)
            toolTip.Unhide();
    }
}
