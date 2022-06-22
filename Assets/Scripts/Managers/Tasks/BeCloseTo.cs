using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BeCloseTo : Task
{
    [SerializeField] private Vector3 position;

    private GameObject player;

    override public float Done()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        return Vector3.Distance(position, player.transform.position);
    }
}
