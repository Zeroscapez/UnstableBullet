using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreenControl : MonoBehaviour
{
    void Start()
    {


        deadScreen();

    }
    public void deadScreen()
    {
        // Pause the game
        Time.timeScale = 0f; // Pause the game
    }

    public void QuitButton()
    {
        
        // Resume the game time in case it was paused
        Time.timeScale = 1f;
        
        
        // Load the main menu scene
        SceneManager.LoadScene("MainMenu"); // Replace "MainMenu" with the actual name of your main menu scene
    }
}
