using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for all UI button inputs 
/// </summary>
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

    protected override void Awake()
    {
        tick = GetComponentInChildren<Hideable>();
        description = GetComponentInChildren<Text>();

        currentSettings = Resources.Load<Settings>("Settings/Current");
    }

    private void Update()
    {
        tick.Hidden = !currentSettings.GetBool(sets);
    }

    public void ToggleBoolean()
    {
        tick.Hidden = !tick.Hidden;
        currentSettings.SetBool(sets, !tick.Hidden);
    }
}
