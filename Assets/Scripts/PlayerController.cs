using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    Vector2 moveInput;
    public float moveSpeed = 5f;

    [Header("Shooting")]

    [Header("Settings")]
    public Transform fireOrigin;
    public GameObject bulletPrefab;
    public Collider2D playerHitbox;
    public Collider2D grazeBox;
    public int playerBulletPoolSize = 20;
    public GameObject pauseMenu;
    public GameObject instantiatedPauseMenu;
    public Sprite defaultPlayer;
    public Sprite distortPlayer;
    public GameObject distortedOverlay;


    [Header("Sounds")]
    public AudioSource audio;
    public AudioClip distortSound;



    public bool isPaused = false; // Flag to check if the game is paused

    public int grazeScore = 1000; // Score for grazing an enemy bullet

    PlayerControls controls;

    // Private pool for player bullets
    private Queue<GameObject> playerBulletPool = new Queue<GameObject>();
    public float fireRate = 0.2f; //Gap between shots firing
    public bool autoFire = true;
    [SerializeField] private bool isDistort = false;
  
    private Coroutine firingCoroutine;

    // Event for player death
    public static event Action OnPlayerDied;
    private void Awake()
    {
        controls = new PlayerControls();
        distortedOverlay.SetActive(false);
        // Bind the movement actions to the Move method
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        autoFire = true;
        if (autoFire)
        {
            // Start firing automatically
            StartFiring();
        }
        else if (!autoFire && !isDistort)
        {
            // Start firing when the fire button is pressed
            controls.Player.Fire.performed += ctx => StartFiring();
            // Stop firing when the fire button is released
            controls.Player.Fire.canceled += ctx => StopFiring();
        }


        controls.Menu.Pause.performed += ctx =>
        {
            if (isPaused)
            {
                // Unpause the game
                Time.timeScale = 1f;
                isPaused = false;
                controls.Player.Enable(); // Re-enable player controls
                // Destroy the instantiated pause menu
                if (instantiatedPauseMenu != null)
                {
                    instantiatedPauseMenu.SetActive(false); // Hide the pause menu
                }
            }
            else
            {
                if (instantiatedPauseMenu == null)
                {
                    // Instantiate the pause menu if it doesn't already exist
                    instantiatedPauseMenu = Instantiate(pauseMenu, Vector3.zero, Quaternion.identity);
                }
                else
                {
                    // Show the existing pause menu
                    instantiatedPauseMenu.SetActive(true);
                }

                // Pause the game
                Time.timeScale = 0f;
                controls.Player.Disable();
                isPaused = true;
               
            }
        };

        controls.Player.Bomb.performed += ctx =>
        {
            if (PlayerScoreManager.Instance.sigils >= 1)
            {
                GigaCrash();
                
            }
           
        };

        controls.Player.Distort.performed += ctx => Distort();

        controls.Player.Distort.canceled += ctx => StopDistort();

        this.gameObject.GetComponent<SpriteRenderer>().sprite = defaultPlayer; // Set the default sprite

    }

    private void Start()
    {
        
        // Initialize the player's bullet pool
        InitializePlayerBulletPool();

       
    }

    private void Distort()
    {

        grazeBox.enabled = false;
        playerHitbox.enabled = false;
        PlayerScoreManager.Instance.AddScore(-1000); // Deduct 1000 points for distorting
        distortedOverlay.SetActive(true); // Activate the distorted overlay
        audio.PlayOneShot(distortSound); // Play the distortion sound
        isDistort = true;
        this.gameObject.GetComponent<SpriteRenderer>().sprite = distortPlayer; // Set the distorted sprite
        controls.Player.Fire.Disable(); //Disable firing
        Debug.Log("Distorting");
    }

    private void StopDistort()
    {
        grazeBox.enabled = true;
        playerHitbox.enabled = true;
        distortedOverlay.SetActive(false);
        isDistort = false;
        this.gameObject.GetComponent<SpriteRenderer>().sprite = defaultPlayer; // Set the default sprite
        controls.Player.Fire.Enable(); // Re-enable firing
        Debug.Log("Stop Distorting");
    }


    private void GigaCrash()
    {
        EnemyBullet[] enemyBullets = FindObjectsOfType<EnemyBullet>();

        int bulletsCleared = 0;
        int pointsgained = 500;

        if(PlayerScoreManager.Instance.sigils >= 1)
        {
            PlayerScoreManager.Instance.RemoveSigil();
            foreach (EnemyBullet bullet in enemyBullets)
            {
                // Destroy or return the bullet to its pool
                if (bullet != null)
                {
                    bullet.ReturnToPool(); // Assuming EnemyBullet inherits from BulletBase
                    bulletsCleared++;
                }
            }
           
            PlayerScoreManager.Instance.AddScore(bulletsCleared * pointsgained);
            
            Debug.Log($"Cleared {bulletsCleared} bullets and awarded {bulletsCleared * pointsgained} points.");

        }





    }
    private void InitializePlayerBulletPool()
    {
        if (bulletPrefab == null)
        {
           // Debug.LogError("Bullet prefab is not assigned in the PlayerController!");
            return;
        }

        for (int i = 0; i < playerBulletPoolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            playerBulletPool.Enqueue(bullet);
        }
    }

    private GameObject GetBulletFromPool()
    {
        if (playerBulletPool.Count > 0)
        {
            GameObject bullet = playerBulletPool.Dequeue();
            bullet.SetActive(true);
            return bullet;
        }
        else
        {
            //Debug.LogWarning("Player bullet pool is empty! Instantiating a new bullet.");
            GameObject bullet = Instantiate(bulletPrefab);
            return bullet;
        }
    }

    public void ReturnBulletToPool(GameObject bullet)
    {
        bullet.SetActive(false);
        playerBulletPool.Enqueue(bullet);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    

    // Update is called once per frame
    void Update()
    {
        if (autoFire && !isDistort)
        {
            // Start firing automatically
            StartFiring();
        }
        else if (!autoFire && !isDistort)
        {
            Debug.Log("Manual Fire");
            // Start firing when the fire button is pressed
            controls.Player.Fire.performed += ctx => StartFiring();
            // Stop firing when the fire button is released
            controls.Player.Fire.canceled += ctx => StopFiring();
        }
        else
        {
            StopFiring();
        }

            Vector3 delta = new Vector3(moveInput.x, moveInput.y, 0) * moveSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + delta;

        // Clamp the position using PlayAreaManager
        newPosition.x = Mathf.Clamp(newPosition.x, PlayAreaManager.Instance.MinX, PlayAreaManager.Instance.MaxX);
        newPosition.y = Mathf.Clamp(newPosition.y, PlayAreaManager.Instance.MinY, PlayAreaManager.Instance.MaxY);

        transform.position = newPosition;

        
    }

    private void Fire()
    {
        if (bulletPrefab != null && fireOrigin != null)
        {
            // Get a bullet from the pool
            GameObject bullet = GetBulletFromPool();

            // Set the bullet's position and rotation to match the fireOrigin
            bullet.transform.position = fireOrigin.position;
            bullet.transform.rotation = fireOrigin.rotation;

            // Set the bullet's pool owner
            BulletBase bulletScript = bullet.GetComponent<BulletBase>();
            if (bulletScript != null)
            {
             
            }

            // Set the bullet's velocity
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = fireOrigin.up * 10f; // Adjust speed as needed
            }
            else
            {
                Debug.LogError("Rigidbody2D not found on bulletPrefab!");
            }

          
        }
    }


    void StartFiring()
    {
        if (firingCoroutine == null)
        {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
    }

    void StopFiring()
    {
        if (firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }
    }

    IEnumerator FireContinuously()
    {
        while (true) // Keep firing until the coroutine is stopped
        {
            Fire();
            yield return new WaitForSeconds(fireRate); // Wait for the next shot
        }
    }

    private void OnDestroy()
    {
        // Notify subscribers that the player has died
        OnPlayerDied?.Invoke();
    }

   
}

