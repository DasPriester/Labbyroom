using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ObjectWithTagExist : Task
{
    [SerializeField] private string tag;

    public ObjectWithTagExist()
    {
        tag = "";
    }
    public ObjectWithTagExist(string tag)
    {
        this.tag = tag;
    }

    override public float Done()
    {
        return GameObject.FindGameObjectsWithTag(tag).Length;
    }

    public override string Progress()
    {
        return tag + ": " + Done();
    }
}
