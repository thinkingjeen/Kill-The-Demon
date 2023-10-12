using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSoundControll : MonoBehaviour
{
    Slider slider;
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.value = InfoManager.instance.optionInfo.musicVolume;
        slider.value = SoundManager.GetVolumeMusic();
        slider.onValueChanged.AddListener((value) =>
        {
            SoundManager.SetVolumeMusic(value);
            InfoManager.instance.optionInfo.musicVolume = value;
            InfoManager.instance.SaveInfos();
        });
    }
}
