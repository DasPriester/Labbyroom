using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Moveable))]
public class PickUpInteractable : Interactable
{

    [SerializeField] protected AudioClip placeSound = default;
    [SerializeField] protected AudioClip pickUpSound = default;
    [SerializeField] protected ParticleSystem pickUpParticle = default;

    protected string prefabName = "";
    protected GameObject prefab;

    public virtual void Start()
    {
        prefabName = GetComponent<Moveable>().PrefabName;
        prefab = (GameObject)Resources.Load("Prefabs/" + prefabName);
    }

    public void OnPlace(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(placeSound, position, Mathf.Min(pc.settings.masterVolume, pc.settings.effectsVolume));
    }

    public override void OnInteract(Vector3 pos)
    {
        if (UseAudio)
            AudioSource.PlayClipAtPoint(pickUpSound, transform.position, Mathf.Min(pc.settings.masterVolume, pc.settings.effectsVolume));

        if (UseParticle && pc.settings.particlesActivated)
            Instantiate(pickUpParticle, transform.position, Quaternion.identity).Play();

        var inv = GameObject.Find("UI/Inventory").GetComponent<PlayerInventory>();
        inv.AddItem(prefab, prefabName);

        Destroy(gameObject);
    }
}
