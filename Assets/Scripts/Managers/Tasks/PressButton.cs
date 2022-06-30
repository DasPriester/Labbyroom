using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PressButton : Task
{
    [SerializeField] private List<KeyCode> buttons;

    public PressButton() { 
        buttons = new List<KeyCode>(); 
    }

    public PressButton(List<KeyCode> buttons)
    {
        this.buttons = buttons;
    }

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
    public override string Progress()
    {
        return "Pressed: " + Done();
    }
}
