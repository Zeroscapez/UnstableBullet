using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Settings screen used by both the main menu and the pause menu.
// Open it with Open(); the Back button closes it and runs the callback so the caller can show its own menu again.
public class SettingsMenuControl : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Display")]
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle vsyncToggle;

    [Header("Gameplay")]
    [SerializeField] private Toggle autoFireToggle;

    [Header("Navigation")]
    [SerializeField] private Button backButton;

    private Action onClosed;

    private void Awake()
    {
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        vsyncToggle.onValueChanged.AddListener(OnVSyncChanged);
        autoFireToggle.onValueChanged.AddListener(OnAutoFireChanged);
        backButton.onClick.AddListener(Close);
    }

    private void OnEnable()
    {
        // Show the saved values without firing the change callbacks
        musicSlider.SetValueWithoutNotify(GameSettings.MusicVolume);
        sfxSlider.SetValueWithoutNotify(GameSettings.SfxVolume);
        fullscreenToggle.SetIsOnWithoutNotify(GameSettings.Fullscreen);
        vsyncToggle.SetIsOnWithoutNotify(GameSettings.VSync);
        autoFireToggle.SetIsOnWithoutNotify(GameSettings.AutoFire);

        // Select the first control so keyboard and gamepad navigation work straight away
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(musicSlider.gameObject);
        }
    }

    public void Open(Action whenClosed)
    {
        onClosed = whenClosed;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        GameSettings.Save();
        gameObject.SetActive(false);

        Action callback = onClosed;
        onClosed = null;
        callback?.Invoke();
    }

    private void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }
        else
        {
            GameSettings.MusicVolume = value; // Still saved; applied by the next scene's AudioManager
        }
    }

    private void OnSfxVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
        }
        else
        {
            GameSettings.SfxVolume = value;
        }
    }

    private void OnFullscreenChanged(bool isOn)
    {
        GameSettings.Fullscreen = isOn;
        GameSettings.ApplyDisplay();
    }

    private void OnVSyncChanged(bool isOn)
    {
        GameSettings.VSync = isOn;
        GameSettings.ApplyDisplay();
    }

    private void OnAutoFireChanged(bool isOn)
    {
        GameSettings.AutoFire = isOn;

        // Apply straight away if we're in a level (opened from the pause menu)
        if (LevelManager.Instance != null && LevelManager.Instance.playerController != null)
        {
            LevelManager.Instance.playerController.SetAutoFire(isOn);
        }
    }
}
