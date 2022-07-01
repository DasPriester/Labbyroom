using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BGMusic : MonoBehaviour
{
   public AudioClip musicClip;
  [SerializeField] private AudioMixer audioMixer = default;

    public void Start()
    {
        Settings settings = Resources.Load<Settings>("Settings/Current");
        audioMixer.SetFloat("Master Volume", Mathf.Log10(settings.masterVolume) * 20);
        audioMixer.SetFloat("Effects Volume", Mathf.Log10(settings.effectsVolume) * 20);
        audioMixer.SetFloat("Music Volume", Mathf.Log10(settings.musicVolume) * 20);
        transform.GetComponent<AudioSource>().clip = musicClip;
        transform.GetComponent<AudioSource>().Play();
    }
}
