using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : Interactable
{
    [SerializeField] private AudioClip doorSound = default;
    [SerializeField] private Animator doorAnimator = default;

    private Animator door2Animator = null;
    private PortalComponent other;

    public override void Awake()
    {
        gameObject.layer = 6;

        toolTip = Instantiate(Resources.Load<ToolTip>("Prefabs/ToolTip"), transform);
        toolTip.key = Camera.main.GetComponentInParent<PlayerController>().interactKey;

        other = doorAnimator.GetComponentInParent<PortalComponent>();
    }

    public void UpdateConnection()
    {
        other = doorAnimator.GetComponentInParent<PortalComponent>();
        door2Animator = other.linkedPortal.GetComponentInChildren<Animator>();
    }

    public override void OnInteract(Vector3 pos)
    {

        if (UseAudio)
        {
            AudioSource.PlayClipAtPoint(doorSound, doorAnimator.gameObject.transform.position);
            if (door2Animator)
                AudioSource.PlayClipAtPoint(doorSound, door2Animator.gameObject.transform.position);
        }

        doorAnimator.SetTrigger("ToggleTrigger");
        if (door2Animator)
            door2Animator.SetTrigger("ToggleTrigger");
        
    }
}
