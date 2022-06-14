using System;
using UnityEngine;

public struct Item
{
    public GameObject prefab;
    public string name;
    public int amount;

    public Item(GameObject prefab, string name, int amount)
    {
        this.prefab = prefab;
        this.name = name;
        this.amount = amount;
    }
}
