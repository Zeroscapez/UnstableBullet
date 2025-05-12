using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreenControl : MonoBehaviour
{
  

    public void QuitButton()
    {
      
        // Resume the game time in case it was paused
        Time.timeScale = 1f;
        
        
        // Load the main menu scene
        SceneManager.LoadScene("MainMenu"); // Replace "MainMenu" with the actual name of your main menu scene
    }
}
