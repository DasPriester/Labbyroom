using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BeCloseTo : Task
{
    [SerializeField] private Vector3 position;

    private GameObject player;
    public BeCloseTo()
    {
        this.position = Vector3.zero;
    }
    public BeCloseTo(Vector3 position)
    {
        this.position = position;
    }

    override public float Done()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        return Vector3.Distance(position, player.transform.position);
    }
    public override string Progress()
    {
        return "Distance: " + Done();
    }
}
