using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mixer;

    public void SetMusicVolume(float volume)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }
}
