using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class PauseMenuControl : MonoBehaviour
{
    private PlayerController playerController; // Reference to the PlayerController

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
