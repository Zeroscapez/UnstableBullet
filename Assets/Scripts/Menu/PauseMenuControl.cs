using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class PauseMenuControl : MonoBehaviour
{
    private PlayerController playerController; // Reference to the PlayerController

    [Header("Settings")]
    [SerializeField] private SettingsMenuControl settingsMenu;
    [SerializeField] private GameObject[] pauseContent; // The pause title and buttons, hidden while settings are open

    // Called by PlayerController.Resume before it hides the pause menu, so unpausing with the
    // pause key while settings are open doesn't leave them open for the next pause
    public void CloseSettings()
    {
        if (settingsMenu != null && settingsMenu.gameObject.activeSelf)
        {
            settingsMenu.Close();
        }
    }

    public void SettingsButton()
    {
        if (settingsMenu == null) return;

        SetPauseContentActive(false);
        settingsMenu.Open(() => SetPauseContentActive(true));
    }

    private void SetPauseContentActive(bool active)
    {
        foreach (GameObject item in pauseContent)
        {
            if (item != null) item.SetActive(active);
        }
    }

    private void Start()
    {
        playerController = LevelManager.Instance.playerController;

        if (playerController == null)
        {
            CustomDebug.LogError("PlayerController not found in the scene!");
        }
    }

    public void ResumeButton()
    {
        // Let the player resume so its controls are re-enabled too
        if (playerController != null)
        {
            playerController.Resume();
        }
        else
        {
            Time.timeScale = 1f;
        }

        // Hide the pause menu
        gameObject.SetActive(false);
    }

    public void QuitButton()
    {
        // Resume the game time in case it was paused
        Time.timeScale = 1f;

        // Load the main menu scene
        SceneManager.LoadScene("MainMenu"); // Replace "MainMenu" with the actual name of your main menu scene
    }
}
