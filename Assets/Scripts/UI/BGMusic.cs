using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMusic : MonoBehaviour
{
   public AudioClip musicClip;

    public void Awake()
    {
        transform.GetComponent<AudioSource>().clip = musicClip;
        transform.GetComponent<AudioSource>().Play();
    }
}
