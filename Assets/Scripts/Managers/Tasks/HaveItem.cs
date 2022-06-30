using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class HaveItem : Task
{
    [SerializeField] private string name;

    private GameObject player;
    public HaveItem()
    {
        name = "";
    }
    public HaveItem(string name)
    {
        this.name = name;
    }

    override public float Done()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        return player.GetComponent<Inventory>().GetAmount(name);
    }
    public override string Progress()
    {
        return name + ": " + Done();
    }
}
