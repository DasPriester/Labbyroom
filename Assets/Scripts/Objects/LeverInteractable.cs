using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverInteractable : Interactable
{
    [Header("Juiciness")]
    [SerializeField] private bool UseAudio = true;
    [SerializeField] private bool UseOutline = true;
    [SerializeField] private bool UseParticle = true;

    [SerializeField] private AudioClip flipSound = default;
    [SerializeField] private Animator leverAnimator = default;

    [SerializeField] private AudioClip doorSound = default;
    [SerializeField] private Animator doorAnimator = default;
    [SerializeField] private Animator door2Animator = default;

    public override void OnInteract()
    {
        
        if (UseAudio)
            AudioSource.PlayClipAtPoint(flipSound, transform.position);
            AudioSource.PlayClipAtPoint(doorSound, doorAnimator.gameObject.transform.position);
            AudioSource.PlayClipAtPoint(doorSound, door2Animator.gameObject.transform.position);

        if (UseParticle)
        {
            GetComponentInParent<ParticleSystem>().Play();
            var shape = GetComponentInParent<ParticleSystem>().shape;
            shape.rotation = new Vector3(-45, shape.rotation.y * -1, 0);
        }


        leverAnimator.SetTrigger("ToggleTrigger");
        doorAnimator.SetTrigger("ToggleTrigger");
        door2Animator.SetTrigger("ToggleTrigger");
        
    }
    public override void OnFocus()
    {
        if (UseOutline)
                gameObject.GetComponent<Outline>().enabled = true;
    }
    public override void OnLoseFcous()
    {
        if (UseOutline)
               gameObject.GetComponent<Outline>().enabled = false;
    }
}
