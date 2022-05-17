using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverDoorInteractable : Interactable
{
    [SerializeField] private AudioClip flipSound = default;
    [SerializeField] private Animator leverAnimator = default;

    [SerializeField] private AudioClip doorSound = default;
    [SerializeField] private Animator doorAnimator = default;
    [SerializeField] private Animator door2Animator = default;

    public override void OnInteract(Vector3 pos)
    {

        if (UseAudio)
        {
            AudioSource.PlayClipAtPoint(flipSound, transform.position, Mathf.Min(Settings.masterVolume, Settings.effectsVolume));
            AudioSource.PlayClipAtPoint(doorSound, doorAnimator.gameObject.transform.position, Mathf.Min(Settings.masterVolume, Settings.effectsVolume));
            AudioSource.PlayClipAtPoint(doorSound, door2Animator.gameObject.transform.position, Mathf.Min(Settings.masterVolume, Settings.effectsVolume));
        }

        if (UseParticle)
        {
            GetComponentInParent<ParticleSystem>().Play();
            var shape = GetComponentInParent<ParticleSystem>().shape;
            shape.rotation = new Vector3(-45, shape.rotation.y * -1, 0);
        }

        if(UseAnimation)
            leverAnimator.SetTrigger("ToggleTrigger");
        doorAnimator.SetTrigger("ToggleTrigger");
        door2Animator.SetTrigger("ToggleTrigger");
        
    }
}
