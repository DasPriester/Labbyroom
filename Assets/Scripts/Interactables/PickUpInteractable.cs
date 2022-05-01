using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpInteractable : Interactable
{
    [Header("Juiciness")]
    [SerializeField] private bool UseAudio = true;
    [SerializeField] private bool UseParticle = true;
    [SerializeField] private bool UseOutline = true;

    [SerializeField] private string displayName = "";

    [SerializeField] private AudioClip pickUpSound = default;
    [SerializeField] private ParticleSystem pickUpParticle = default;

    public override void OnInteract()
    {
        if (UseAudio)
            AudioSource.PlayClipAtPoint(pickUpSound, transform.position);

        if (UseParticle)
            Instantiate(pickUpParticle, transform.position, Quaternion.identity).Play();

        PlayerInventory inv = Camera.main.GetComponentInParent<PlayerInventory>();
        inv.AddItem(displayName);

        Destroy(gameObject);
    }

    public override void OnFocus()
    {
        if (UseOutline) 
            gameObject.GetComponent<Outline>().enabled = true;
                
    }
    public override void OnLoseFcous()
    {
        if(UseOutline)
            gameObject.GetComponent<Outline>().enabled = false;
    }
}
