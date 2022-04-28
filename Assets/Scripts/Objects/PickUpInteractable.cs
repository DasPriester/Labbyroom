using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpInteractable : Interactable
{
    [Header("Juiciness")]
    [SerializeField] private bool UseAudio = true;
    //[SerializeField] private bool UseParticle = true;
    [SerializeField] private bool UseOutline = true;

    [SerializeField] private AudioClip pickUpSound = default;
    public override void OnInteract()
    {
        if (UseAudio)
            AudioSource.PlayClipAtPoint(pickUpSound, transform.position);
        Destroy(gameObject);
    }
    public override void OnFocus()
    {
        if(UseOutline)
            gameObject.GetComponent<Outline>().enabled = true;
    }
    public override void OnLoseFcous()
    {
        if(UseOutline)
            gameObject.GetComponent<Outline>().enabled = false;
    }
}
