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
        Item item = new Item();
        foreach (PickUpInteractable p in recipe.Yield.Keys)
        {
            item.name = p.name;
            item.prefab = p.gameObject;
            break;
        }
        GameObject.Find("Canvas/Image").GetComponent<Image>().sprite = Utility.GetIconFor(item);
    }

    public override void OnInteract(Vector3 pos)
    {
        if (UseAudio)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, Mathf.Min(pc.settings.masterVolume, pc.settings.effectsVolume));
        }

        recipe.unlocked = true;
        Destroy(gameObject);
        
    }
}
