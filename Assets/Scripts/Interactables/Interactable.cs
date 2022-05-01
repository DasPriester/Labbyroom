using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("Juiciness")]
    [SerializeField] public bool UseAudio = true;
    [SerializeField] public bool UseOutline = true;
    [SerializeField] public bool UseParticle = true;
    [SerializeField] public bool UseAnimation = true;


    public virtual void Awake()
    {
        gameObject.layer = 6;
    }
    public abstract void OnInteract();
    public abstract void OnFocus();
    public abstract void OnLoseFcous();

//    private void OnDrawGizmos()
//    {
//        Gizmos.color = Color.yellow;
//        Gizmos.DrawWireCube(transform.position, transform.localScale);
//    }
}
