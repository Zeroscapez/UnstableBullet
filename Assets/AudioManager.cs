using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    // One per scene (MainMenu and Level1TheSun each have their own)
    public static AudioManager Instance { get; private set; }

    public AudioMixer mixer;

    // Must match the exposed parameter names on MainAudioMixer
    [SerializeField] private string musicVolumeParam = "BGMVolume";
    [SerializeField] private string sfxVolumeParam = "SFXVolume";

    // Log10(0) is -infinity, so clamp to a tiny value (-80 dB, silent)
    private const float MinVolume = 0.0001f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // AudioMixer.SetFloat doesn't stick when called in Awake, so apply the saved volumes here
        SetVolume(musicVolumeParam, GameSettings.MusicVolume);
        SetVolume(sfxVolumeParam, GameSettings.SfxVolume);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // Sets and saves the music volume (0-1)
    public void SetMusicVolume(float volume)
    {
        GameSettings.MusicVolume = volume;
        SetVolume(musicVolumeParam, volume);
    }

    // Sets and saves the sound effects volume (0-1)
    public void SetSFXVolume(float volume)
    {
        GameSettings.SfxVolume = volume;
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
