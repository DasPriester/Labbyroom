using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeInteractable : Interactable
{
    [SerializeField] private AudioClip pickupSound = default;
    [SerializeField] private Recipe recipe = null;

    private void Start()
    {
        GameObject.Find("Canvas/Text").GetComponent<Text>().text = recipe.name;
        string nameOfResult = "";
        foreach (PickUpInteractable p in recipe.Yield.Keys)
        {
            nameOfResult = p.name;
            break;
        }
        GameObject.Find("Canvas/Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + nameOfResult);
    }

    public override void OnInteract(Vector3 pos)
    {
        if (UseAudio)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        recipe.unlocked = true;
        Destroy(gameObject);
        
    }
}
