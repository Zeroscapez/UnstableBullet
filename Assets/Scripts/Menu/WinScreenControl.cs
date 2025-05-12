using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreenControl : MonoBehaviour
{
    public int highScore { get; private set; }
    public TextMeshProUGUI highScoreText;
    public int finalScore { get; private set; }
    public TextMeshProUGUI finalScoreText;
    // Start is called before the first frame update
    void Start()
    {
        
       
        CallScore();

    }

    public void CallScore()
    {
        PlayerScoreManager.Instance.AddScore(1000); // Add 1000 points to the score
        highScore = PlayerScoreManager.Instance.highScore; // Get the high score
        highScoreText.text = highScore.ToString(); // Update the high score text
        finalScore = PlayerScoreManager.Instance.score; // Get the final score
        finalScoreText.text = finalScore.ToString(); // Update the final score text
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
