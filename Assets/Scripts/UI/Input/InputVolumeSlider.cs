using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class InputVolumeSlider : InputSlider
{
    [SerializeField] private AudioMixer audioMixer = default;
    [SerializeField] private string audioChannel;
    
    public string AudioChannel {
         get { return audioChannel; }
         set { audioChannel = value; }
    }

    public override void ChangeValue()
    {
        currentSettings.SetValue(sets, slider.value);
        audioMixer.SetFloat(audioChannel, Mathf.Log10(slider.value) * 20);
    }

}
