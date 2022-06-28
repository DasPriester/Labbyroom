using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SubMenu : MonoBehaviour
{

    protected Inventory inv = null;

    public virtual void Awake()
    {
        inv = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().GetComponent<Inventory>();
    }

    public abstract void OpenMenu();
    public abstract void CloseMenu();


}
