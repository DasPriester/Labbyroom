using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for all UI slider inputs
/// </summary>
public class InputSlider : MonoBehaviour
{
    protected Settings currentSettings;

    public string Sets
    {
        get { return sets; }
        set {
            sets = value;
            description.text = sets;
        }
    }
    protected string sets;

    protected Text description;
    protected Slider slider;

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

    public virtual void ChangeValue()
    {
        currentSettings.SetValue(sets, slider.value);
    }
}
