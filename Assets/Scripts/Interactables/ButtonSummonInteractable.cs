using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSummonInteractable : Interactable
{   
    [SerializeField] private AudioClip buttonSound = default;
    [SerializeField] private Animator buttonAnimator = default;

    [SerializeField] private ParticleSystem summonParticle = default;
    [SerializeField] private GameObject summonPrefab = default;
    [SerializeField] private GameObject summonLocation = default;
    public override void OnInteract()
    {
        if (UseAudio)
            AudioSource.PlayClipAtPoint(buttonSound, transform.position);

        if (UseAnimation)
            buttonAnimator.SetTrigger("PressTrigger");

        Vector3 pos = summonLocation.transform.position;
        pos.y += 15;

        if (UseParticle)
            Instantiate(summonParticle, pos, Quaternion.identity).Play();

        Instantiate(summonPrefab, pos, Quaternion.identity);


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

