using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public string levelName;
    public int levelID;

    public static LevelManager Instance;

    public float timer;
    public TextMeshProUGUI timerText;
    public PlayerController playerController;
    private GameObject manager;

    public void Awake()
    {
       if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        levelName = SceneManager.GetActiveScene().name;
        levelID = SceneManager.GetActiveScene().buildIndex;
    }
    // Update is called once per frame
    void Update()
    {
        timer = Time.timeSinceLevelLoad;
        timerText.text = timer.ToString("F0");
    }
}
