using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputSetterButton : Button
{
    Settings currentSettings;

    public string Sets
    {
        get { return sets; }
        set { sets = value; text.text = currentSettings.GetKey(sets).ToString(); description.text = sets; }
    }
    private string sets;

    private Text description;
    private Text text;

    private void Awake()
    {
        foreach (Text t in GetComponentsInChildren<Text>())
        {
            if (t.name == "Text")
                text = t;
            if (t.name == "Description")
                description = t;
        }
        currentSettings = Resources.Load<Settings>("Settings/Current");
        text.text = currentSettings.GetKey(sets).ToString();
    }

    public void SetKey()
    {
        StopAllCoroutines();
        StartCoroutine(SetAfterInput());
    }

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
