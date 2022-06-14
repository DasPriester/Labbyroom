using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class InputVolumeSlider : InputSlider
{
    [SerializeField] private AudioMixer audioMixer = default;

    public override void ChangeValue()
    {
        currentSettings.SetValue(sets, slider.value);
        audioMixer.SetFloat(sets, Mathf.Log10(slider.value) * 20);
    }

}
