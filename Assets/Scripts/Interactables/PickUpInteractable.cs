using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// object that can be picked up
/// </summary>
public class PickUpInteractable : Interactable
{

    [SerializeField] protected AudioClip placeSound = default;
    [SerializeField] protected AudioClip pickUpSound = default;
    [SerializeField] protected ParticleSystem pickUpParticle = default;


    protected AudioSource audioSource;
    public string prefabName = "";

    /// <summary>
    /// Loading the prefab by its name
    /// </summary>
    public virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
        prefab = (GameObject)Resources.Load("Prefabs/" + prefabName);
    }

    protected GameObject prefab;
    /// <summary>
    /// Play placing sound
    /// </summary>
    /// <param name="position">Potition to play the sound at</param>
    public void OnPlace(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(placeSound, position);
    }

    /// <summary>
    /// Play pickup sound and add item to inventory
    /// </summary>
    public override void OnInteract(Vector3 hit)
    {
        AudioSource.PlayClipAtPoint(pickUpSound, transform.position);

        if (pc.settings.particlesActivated)
            Instantiate(pickUpParticle, transform.position, Quaternion.identity).Play();

        var inv = GameObject.Find("UI/Inventory").GetComponent<Inventory>();
        inv.AddItem(prefab, prefabName);

        Destroy(gameObject);
    }
}
