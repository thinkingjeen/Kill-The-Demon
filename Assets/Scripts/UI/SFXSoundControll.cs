using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXSoundControll : MonoBehaviour
{
    Slider slider;    
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.value = InfoManager.instance.optionInfo.SFXVolume;
        slider.value = SoundManager.GetVolumeSFX();
        slider.onValueChanged.AddListener((value) =>
        {
            SoundManager.SetVolumeSFX(value);
            InfoManager.instance.optionInfo.SFXVolume = value;
            InfoManager.instance.SaveInfos();
        });
    }

    public void OnVolumeChanged()
    {
        App.instance.YesAudio();
    }
}
