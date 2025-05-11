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
    public int playerBulletPoolSize = 20;
    public GameObject pauseMenu;
    public GameObject instantiatedPauseMenu;

    public bool isPaused = false; // Flag to check if the game is paused

    public int grazeScore = 1000; // Score for grazing an enemy bullet

    PlayerControls controls;

    // Private pool for player bullets
    private Queue<GameObject> playerBulletPool = new Queue<GameObject>();
    public float fireRate = 0.2f; //Gap between shots firing
    public bool isFiring;
  
    private Coroutine firingCoroutine;

    // Event for player death
    public static event Action OnPlayerDied;
    private void Awake()
    {
        controls = new PlayerControls();

        // Bind the movement actions to the Move method
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // Start firing when the fire button is pressed
        controls.Player.Fire.performed += ctx => StartFiring();
        // Stop firing when the fire button is released
        controls.Player.Fire.canceled += ctx => StopFiring();

        controls.Player.Pause.performed += ctx =>
        {
            if (isPaused)
            {
                // Unpause the game
                Time.timeScale = 1f;
                isPaused = false;

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
                isPaused = true;
            }
        };
    }

    private void Start()
    {
        // Initialize the player's bullet pool
        InitializePlayerBulletPool();
    }

    private void InitializePlayerBulletPool()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet prefab is not assigned in the PlayerController!");
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
            Debug.LogWarning("Player bullet pool is empty! Instantiating a new bullet.");
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

