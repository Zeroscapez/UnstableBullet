using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class PlayerScoreManager : MonoBehaviour
{
    public static PlayerScoreManager Instance { get; private set; }
    public int score { get; private set; }
    public TextMeshProUGUI scoreText;

   

    [Header("Sigil UI")]
    public int sigils = 0;
    private int maxSigils = 3;
    [SerializeField] private int disruptToBomb;
    public GameObject sigilIconPrefab;
    public Transform sigilIconContainer; // Parent transform in UI to hold instantiated sigils
    public List<GameObject> sigilIcon = new();

    public GameObject winScreenPrefab;
    private GameObject winScreenInstance;

    [Header("Life UI")]
    public int lives = 12;
    private int maxLives = 12;
    public GameObject lifeIconPrefab;
    public Transform lifeIconContainer; // Parent transform in UI to hold instantiated lives
    public List<GameObject> lifeIcon = new();
    
    public int highScore { get; private set; }
    public TextMeshProUGUI highScoreText;
    public int disrupt;
    public TextMeshProUGUI disruptText;
    private int maxDisrupt = 999;
    private bool isPlayerAlive = true; // Track if the player is alive
    public bool DebugMode;

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

        IntitializeScores();
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
        DebugMode = false;
        
        InitializeSigils();
        InitializeLives();
        IntitializeScores();
    }

    private void FixedUpdate()
    {
        if (DebugMode)
        {
            Debug.LogWarning("Debug mode is enabled. Dev Actions are enabled");
        }


    }

    public void AddScore(int amount)
    {
        score += amount;

        score = Mathf.Clamp(score, 0, 9999999); 

        if (score > highScore)
        {
            highScore = score;
        }

        UpdateScoreText();
    }

    public void AddDisrupt(int amount)
    {
        
        disrupt += amount;
       
        disruptToBomb += amount;

        disruptToBomb = Mathf.Clamp(disruptToBomb, 0, 100); 
        disrupt = Mathf.Clamp(disrupt, 0, maxDisrupt);

        if(disruptToBomb >= 100)
        {
            Debug.Log("Added Sigil");
            AddSigil();
            disruptToBomb = 0;
        }

        UpdateScoreText();
    }

    public void ResetDisrupt()
    {
        disrupt = 0;
        disruptToBomb = 0;
        UpdateScoreText();
    }

    // Update is called once per frame
    void Update()
    {

        if (DebugMode && Input.GetKeyDown(KeyCode.UpArrow))
        {
            AddScore(1000);
        }

        if (DebugMode && Input.GetKeyDown(KeyCode.I))
        {
            AddDisrupt(10);
        }

        if (DebugMode && Input.GetKeyDown(KeyCode.O))
        {
            AddSigil();
        }

        if (DebugMode && Input.GetKeyDown(KeyCode.P))
        {
            RemoveSigil();
        }

        if(DebugMode && Input.GetKeyDown(KeyCode.L))
        {
            KillPlayer();
        }

        if (DebugMode && Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(1); // Take 1 hit
        }
    }

    private void IntitializeScores()
    {
        score = 0;
        highScore = 0;
        disrupt = 0;
        sigils = 0;
        
        UpdateScoreText();
    }

    private void InitializeSigils()
    {
        // Clear existing sigils
        foreach (var icon in sigilIcon)
        {
            Destroy(icon);
        }
        sigilIcon.Clear();

        // Create up to 3 sigil icons
        for (int i = 0; i < maxSigils; i++)
        {
            GameObject newIcon = Instantiate(sigilIconPrefab, sigilIconContainer);
            sigilIcon.Add(newIcon);
        }

        UpdateSigilUI();
    }

    public void AddSigil()
    {
        if (sigils < 3)
        {
            sigils++;

            sigils = Mathf.Clamp(sigils, 0, maxSigils);
            UpdateSigilUI();
        }
        else
        {
            AddScore(100 * sigils);
        }
    }

    public void InitializeLives()
    {
        lives = maxLives;
        foreach (var icon in lifeIcon)
        {
            Destroy(icon);
        }
        lifeIcon.Clear();
        for (int i = 0; i < maxLives/2; i++)
        {
            GameObject newIcon = Instantiate(lifeIconPrefab, lifeIconContainer);
            lifeIcon.Add(newIcon);
        }
    }

    public void TakeDamage(float amount)
    {
        lives -= 1;
        lives = Mathf.Clamp(lives, 0, maxLives);
        ResetDisrupt();
        UpdateLivesUI();

        if (lives <= 0)
        {
            HandlePlayerDeath();
        }
    }

    private void UpdateLivesUI()
    {
        float remainingLives = lives;

        for (int i = 0; i < lifeIcon.Count; i++)
        {
            Image iconImage = lifeIcon[i].transform.GetChild(0).GetComponent<Image>();

            // Each icon represents 2 HP
            float fill = Mathf.Clamp01(remainingLives / 2f);
            iconImage.fillAmount = fill;

            remainingLives -= 2;
        }
    }


    public void RemoveSigil()
    {
        if (sigils > 0)
        {
            sigils--;
            sigils = Mathf.Clamp(sigils, 0, maxSigils);
            UpdateSigilUI();
        }
    }

    private void UpdateSigilUI()
    {
        for (int i = 0; i < sigilIcon.Count; i++)
        {
            sigilIcon[i].SetActive(i < sigils);
        }
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
            disruptText.text = disrupt.ToString("D3");
        }
    }

    private void HandlePlayerDeath()
    {
        // Stop the score increment when the player dies
        isPlayerAlive = false;
        highScore = score;
        SceneManager.LoadScene("MainMenu");
        IntitializeScores();
    }

    public void CalculateScore()
    {
        if (winScreenInstance == null && winScreenPrefab != null)
        {
            winScreenInstance = Instantiate(winScreenPrefab, Vector3.zero, Quaternion.identity);
        }
    }


    private void KillPlayer()
    {
        TakeDamage(12);
    }
}
