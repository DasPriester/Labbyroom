using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputSlider : MonoBehaviour
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
    private Slider slider;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        description = GetComponentInChildren<Text>();
    }

    private void Update()
    {
        slider.value = Settings.GetValue(sets);
    }

    public void ChangeValue()
    {
        Settings.SetValue(sets, slider.value);
    }
}
