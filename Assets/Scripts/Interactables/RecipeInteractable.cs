using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeInteractable : Interactable
{
    [SerializeField] private AudioClip pickupSound = default;
    [SerializeField] private Recipe recipe = null;

    public override void OnInteract(Vector3 pos)
    {
        if (UseAudio)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        recipe.unlocked = true;
        Destroy(gameObject);
        
    }
    public override void OnFocus(Vector3 pos)
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
