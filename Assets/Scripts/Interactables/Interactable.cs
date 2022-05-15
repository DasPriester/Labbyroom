using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Interactable : MonoBehaviour
{
    [Header("Juiciness")]
    [SerializeField] public bool UseAudio = true;
    [SerializeField] public bool UseOutline = true;
    [SerializeField] public bool UseParticle = true;
    [SerializeField] public bool UseAnimation = true;
    [SerializeField] public bool UseToolTip = true;
    protected ToolTip toolTip;

    public virtual void Awake()
    {
        gameObject.layer = 6;

        if (UseToolTip)
        {
            toolTip = GetComponentInChildren<ToolTip>();
        }
    }

    private void Update()
    {
        if (toolTip)
        {
            UpdateToolTip();
        }
    }

    public void UpdateToolTip()
    {
        toolTip.Key = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerController>().settings.interactKey;
    }

    public abstract void OnInteract(Vector3 pos);

    public virtual void OnFocus(Vector3 pos)
    {
        if (UseOutline)
            gameObject.GetComponent<Outline>().enabled = true;

        if (toolTip && UseToolTip)
            toolTip.Unhide();
    }
    public virtual void OnLoseFcous()
    {
        if (UseOutline)
            gameObject.GetComponent<Outline>().enabled = false;

        if (toolTip && UseToolTip)
            toolTip.Hide();
    }
}
