using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputBooleanButton : Button
{
    public string Sets
    {
        get { return sets; }
        set {
            sets = value;

            description.text = sets;
        }
    }
    private string sets;

    private Text description;
    private Hideable tick;

    private void Awake()
    {
        tick = GetComponentInChildren<Hideable>();
        description = GetComponentInChildren<Text>();
    }

    private void Update()
    {
        tick.SetHide(!Settings.GetBool(sets));
    }

    public void ToggleBoolean()
    {
        tick.ToggleHide();
        Settings.SetBool(sets, !tick.isHidden());
    }
}
