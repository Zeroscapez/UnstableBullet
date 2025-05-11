using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class PauseMenuControl : MonoBehaviour
{
    private PlayerController playerController; // Reference to the PlayerController

    private void Start()
    {
        // Find the PlayerController in the scene
        playerController = FindObjectOfType<PlayerController>();

        if (playerController == null)
        {
            Debug.LogError("PlayerController not found in the scene!");
        }
    }

    public void ResumeButton()
    {
        // Resume the game
        Time.timeScale = 1f;

        // Hide the pause menu
        gameObject.SetActive(false);

        // Update the isPaused variable in PlayerController
        if (playerController != null)
        {
            playerController.isPaused = false;
        }
    }

    public void QuitButton()
    {
        // Resume the game time in case it was paused
        Time.timeScale = 1f;

        // Load the main menu scene
        SceneManager.LoadScene("MainMenu"); // Replace "MainMenu" with the actual name of your main menu scene
    }
}
