using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mixer;

    // Must match the exposed parameter names on MainAudioMixer
    [SerializeField] private string musicVolumeParam = "BGMVolume";
    [SerializeField] private string sfxVolumeParam = "SFXVolume";

    // Log10(0) is -infinity, so clamp to a tiny value (-80 dB, silent)
    private const float MinVolume = 0.0001f;

    public void SetMusicVolume(float volume)
    {
        SetVolume(musicVolumeParam, volume);
    }

    public void SetSFXVolume(float volume)
    {
        SetVolume(sfxVolumeParam, volume);
    }

    private void SetVolume(string param, float volume)
    {
        float db = Mathf.Log10(Mathf.Clamp(volume, MinVolume, 1f)) * 20;
        if (!mixer.SetFloat(param, db))
        {
            CustomDebug.LogWarning($"Audio mixer has no exposed parameter named '{param}'.");
        }
    }
}
