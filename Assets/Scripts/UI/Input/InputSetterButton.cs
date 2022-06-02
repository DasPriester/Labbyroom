using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for all UI key inputs
/// </summary>
public class InputSetterButton : Button
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
    private Text text;

    protected override void Awake()
    {
        foreach (Text t in GetComponentsInChildren<Text>())
        {
            if (t.name == "Text")
                text = t;
            if (t.name == "Description")
                description = t;
        }
        currentSettings = Resources.Load<Settings>("Settings/Current");
    }

    private void Update()
    {
        text.text = currentSettings.GetKey(sets).ToString();
    }

    /// <summary>
    /// Stop all corutines and just listen to next input
    /// </summary>
    public void SetKey()
    {
        StopAllCoroutines();
        StartCoroutine(SetAfterInput());
    }

    /// <summary>
    /// Wait until any input is received and change setting
    /// </summary>
    IEnumerator SetAfterInput()
    {
        yield return new WaitUntil(() => { return Input.anyKeyDown; });

        KeyCode key = new KeyCode();

        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(kcode))
            {
                key = kcode;
                text.text = kcode.ToString();
            }
        }

        currentSettings.SetKey(sets, key);
    }
}
