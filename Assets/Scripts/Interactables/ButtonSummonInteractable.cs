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

    public override void OnInteract(Vector3 pos)
    {
        if (UseAudio)
            AudioSource.PlayClipAtPoint(buttonSound, transform.position);

        if (UseAnimation)
            buttonAnimator.SetTrigger("PressTrigger");

        Vector3 position = summonLocation.transform.position;
        position.y += 15;

        if (UseParticle)
            Instantiate(summonParticle, position, Quaternion.identity).Play();

        Instantiate(summonPrefab, position, Quaternion.identity);


    }
}

