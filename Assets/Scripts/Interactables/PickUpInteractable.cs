using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// object that can be picked up
/// </summary>
[RequireComponent(typeof(Moveable))]
[RequireComponent(typeof(AudioSource))]
public class PickUpInteractable : Interactable
{

    [SerializeField] protected AudioClip placeSound = default;
    [SerializeField] protected AudioClip pickUpSound = default;
    [SerializeField] protected ParticleSystem pickUpParticle = default;


    protected GameObject prefab;
    protected string prefabName = "";

    public virtual void Start()
    {
        prefabName = GetComponent<Moveable>().PrefabName;
        prefab = (GameObject)Resources.Load("Prefabs/" + prefabName);
    }

    /// <summary>
    /// Play placing sound
    /// </summary>
    public void OnPlace()
    {
        if (UseAudio)
        {
            GetComponent<AudioSource>().clip = placeSound;
            GetComponent<AudioSource>().Play();
        }
            
    }

    protected void PlayPickUpAudio()
    {
        GameObject go = new GameObject("AudioSource");
        go.AddComponent<AudioSource>();
        go.GetComponent<AudioSource>().playOnAwake = false;
        go.GetComponent<AudioSource>().outputAudioMixerGroup = GetComponent<AudioSource>().outputAudioMixerGroup;
        go.GetComponent<AudioSource>().clip = pickUpSound;
        go.GetComponent<AudioSource>().Play();
        Destroy(go, pickUpSound.length + 1);
    }

    /// <summary>
    /// Play pickup sound and add item to inventory
    /// </summary>
    public override void OnInteract(Vector3 hit)
    {
        if (UseAudio)
        {
            PlayPickUpAudio();
        }

        if (UseParticle)
            Instantiate(pickUpParticle, transform.position, Quaternion.identity).Play();

        var inv = GameObject.Find("UI/Inventory").GetComponent<Inventory>();
        inv.AddItem(prefab, prefabName);

        Destroy(gameObject);
    }
}
