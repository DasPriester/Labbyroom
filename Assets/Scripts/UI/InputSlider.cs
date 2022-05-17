using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputSlider : MonoBehaviour
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
    private Slider slider;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        description = GetComponentInChildren<Text>();

        currentSettings = Resources.Load<Settings>("Settings/Current");
    }

    private void Update()
    {
        slider.value = currentSettings.GetValue(sets);
    }

    public void ChangeValue()
    {
        currentSettings.SetValue(sets, slider.value);
    }
}
