using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerScoreManager : MonoBehaviour
{
    public static PlayerScoreManager Instance { get; private set; }
    public int score { get; private set; }
    public TextMeshProUGUI scoreText;

    public int highScore { get; private set; }
    public TextMeshProUGUI highScoreText;
    public int disrupt { get; private set; }
    public TextMeshProUGUI disruptText;
    public int maxDisrupt = 999;
    private bool isPlayerAlive = true; // Track if the player is alive

    // Start is called before the first frame update

    private void Awake()
    {
        // Ensure only one instance of PlayAreaManager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void OnEnable()
    {
        PlayerController.OnPlayerDied += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        // Unsubscribe from the player death event
        PlayerController.OnPlayerDied -= HandlePlayerDeath;
    }

    void Start()
    {
       
    }

    public void AddScore(int amount)
    {
        score += amount;

        if (score > highScore)
        {
            highScore = score;
        }

        UpdateScoreText();
    }

    public void AddDisrupt(int amount)
    {
        disrupt += amount;
       
        disrupt = Mathf.Clamp(disrupt, 0, maxDisrupt);


    }

    public void ResetDisrupt()
    {
        disrupt = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString("D7");
        }

        if(highScoreText != null)
        {
            highScoreText.text = highScore.ToString("D7");
        }

        if(disruptText != null)
        {
            disruptText.text = disrupt.ToString("D4");
        }
    }

    private void HandlePlayerDeath()
    {
        // Stop the score increment when the player dies
        isPlayerAlive = false;
    }
}
