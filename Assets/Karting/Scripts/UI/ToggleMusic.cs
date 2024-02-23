using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]

public class ToggleMusic : MonoBehaviour
{
    public AudioMixerGroup AMG;
    Toggle musicToggle;

    private void Start()
    {
        musicToggle = GetComponent<Toggle>();
        if(AudioListener.volume == 0)
        {
            musicToggle.isOn = false;
        }
    }

    public void ToggleAudio(bool AudioIn)
    {
        AudioListener.volume = AudioIn ? 1 : 0;
    }
}