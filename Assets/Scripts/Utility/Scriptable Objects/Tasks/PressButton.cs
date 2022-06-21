using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PressButton : Task
{
    [SerializeField] private List<KeyCode> buttons;

    override public float Done()
    {
        int k = 0;
        foreach(KeyCode button in buttons)
        {
            if (Input.GetKey(button))
                k += 1;
        }
        return k;
    }
}
