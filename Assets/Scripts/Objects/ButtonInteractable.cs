using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteractable : Interactable
{

    [Header("Juiciness")]
    [SerializeField] private bool UseAudio = true;
    [SerializeField] private bool UseOutline = true;
    //[SerializeField] private bool UseParticle = true;

    
    [SerializeField] private AudioClip buttonSound = default;
    [SerializeField] private GameObject prefab = default;
    [SerializeField] private Animator buttonAnimator = default;
    public override void OnInteract()
    {
        if (UseAudio)
            AudioSource.PlayClipAtPoint(buttonSound, transform.position);


        buttonAnimator.SetTrigger("PressTrigger");

        Instantiate(prefab, new Vector3(-8, 15, -60), Quaternion.identity);

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

