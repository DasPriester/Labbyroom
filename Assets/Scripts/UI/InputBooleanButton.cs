using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputBooleanButton : Button
{
    Settings currentSettings;

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

        currentSettings = Resources.Load<Settings>("Settings/Current");
    }

    private void Update()
    {
        tick.SetHide(!currentSettings.GetValue(sets));
    }

    public void ToggleBoolean()
    {
        tick.ToggleHide();
        currentSettings.SetValue(sets, !tick.isHidden());
    }
}
