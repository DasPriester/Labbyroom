using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpInteractable : Interactable
{
    [SerializeField] private string prefabName = "";

    [SerializeField] private AudioClip placeSound = default;
    [SerializeField] private AudioClip pickUpSound = default;
    [SerializeField] private ParticleSystem pickUpParticle = default;

    private GameObject prefab;

    public void Start()
    {
        prefab = (GameObject)Resources.Load("Prefabs/"+prefabName);
    }

    public void OnPlace(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(placeSound, position);
    }

    public override void OnInteract(Vector3 pos)
    {
        if (UseAudio)
            AudioSource.PlayClipAtPoint(pickUpSound, transform.position);

        if (UseParticle)
            Instantiate(pickUpParticle, transform.position, Quaternion.identity).Play();

        var inv = GameObject.Find("UI/Inventory").GetComponent<PlayerInventory>();
        inv.AddItem(prefab, prefabName);

        Destroy(gameObject);
    }

    public override void OnFocus(Vector3 pos)
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
