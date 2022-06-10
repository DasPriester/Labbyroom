using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Recipe that can be picked up
/// </summary>
public class RecipeInteractable : PickUpInteractable
{
    [SerializeField] private Recipe recipe = null;

    private Text text;
    private Image image;

    public Recipe Recipe {
        get { return recipe; }
        set { recipe = value; }
    }

    public override void Start()
    {
        prefab = (GameObject)Resources.Load("Prefabs/" + prefabName);
        text = GameObject.Find("Canvas/Text").GetComponent<Text>();
        image = GameObject.Find("Canvas/Image").GetComponent<Image>();

        
        text.text = recipe.name;
        Item item = new Item();
        foreach (PickUpInteractable p in recipe.Yield.Keys)
        {
            item.name = p.name;
            item.prefab = p.gameObject;
            break;
        }
        image.sprite = Utility.GetIconFor(item);
    }
   
    /// <summary>
    /// Play sound and unlock recipe
    /// </summary>
    public override void OnInteract(Vector3 hit)
    {
        if(UseAudio)
            AudioSource.PlayClipAtPoint(pickUpSound, transform.position, Mathf.Min(pc.settings.masterVolume, pc.settings.effectsVolume));
        
        recipe.unlocked = true;
        Destroy(gameObject);
        
    }
}
