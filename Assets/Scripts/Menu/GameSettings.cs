using UnityEngine;

// Player-facing settings, saved with PlayerPrefs so they persist between launches.
// Audio volumes are applied by AudioManager (it owns the mixer); display settings are applied here.
public static class GameSettings
{
    private const string MusicVolumeKey = "Settings.MusicVolume";
    private const string SfxVolumeKey = "Settings.SfxVolume";
    private const string FullscreenKey = "Settings.Fullscreen";
    private const string VSyncKey = "Settings.VSync";
    private const string AutoFireKey = "Settings.AutoFire";

    // Volumes are 0-1 (slider values); AudioManager converts them to decibels
    public static float MusicVolume
    {
        get => PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        set => PlayerPrefs.SetFloat(MusicVolumeKey, Mathf.Clamp01(value));
    }

    public static float SfxVolume
    {
        get => PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
        set => PlayerPrefs.SetFloat(SfxVolumeKey, Mathf.Clamp01(value));
    }

    public static bool Fullscreen
    {
        get => PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;
        set => PlayerPrefs.SetInt(FullscreenKey, value ? 1 : 0);
    }

    public static bool VSync
    {
        get => PlayerPrefs.GetInt(VSyncKey, 1) == 1;
        set => PlayerPrefs.SetInt(VSyncKey, value ? 1 : 0);
    }

    public static bool AutoFire
    {
        get => PlayerPrefs.GetInt(AutoFireKey, 1) == 1;
        set => PlayerPrefs.SetInt(AutoFireKey, value ? 1 : 0);
    }

    public static void Save()
    {
        PlayerPrefs.Save();
    }

    public static void ApplyDisplay()
    {
        Screen.fullScreen = Fullscreen;
        QualitySettings.vSyncCount = VSync ? 1 : 0;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void ApplyOnStartup()
    {
        QualitySettings.vSyncCount = VSync ? 1 : 0;

        // Only override the window mode once the player has chosen one in the settings menu
        if (PlayerPrefs.HasKey(FullscreenKey))
        {
            Screen.fullScreen = Fullscreen;
        }
    }
}
