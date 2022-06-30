using UnityEngine;
using System;

[Serializable]
public abstract class Task
{
    public abstract float Done();
    public abstract string Progress();
}