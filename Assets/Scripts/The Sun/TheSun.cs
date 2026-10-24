using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum TheSunBossState
{
    Phase1 = 0, Phase2P1 = 1, Phase2P2 = 2, Phase3 = 3
}
public class TheSun : Enemy
{
    [Header("Spawner Settings")]
    public GameObject spawnerPrefab;
    public float spawnerMoveDistance = 5f;
    public Transform leftHand;
    public Transform rightHand;
    public Transform centerOfSun;
    public Transform centerOfBounds;

    [Header("Boss Settings")]
    public GameObject idolHPBar;
    private Image idolFill;


    [Header("Movement Settings")]
    public int bossMoveSpeed = 1;
    public float moveInterval = 3f;
    private float moveTimer = 0f;
    private Vector3 originalPosition;
    private Vector3 targetMovePosition;


    [Header("State and Timer")]
    public TheSunBossState currentState = TheSunBossState.Phase1; 
    public float fightTimer = 0f;
    [Tooltip("Fraction of max HP at which each phase starts")]
    [SerializeField, Range(0f, 1f)] private float phase2P1Threshold = 0.8f;
    [SerializeField, Range(0f, 1f)] private float phase2P2Threshold = 0.5f;
    [SerializeField, Range(0f, 1f)] private float phase3Threshold = 0.3f;
    public float phase2P1HP;
    public float phase2P2HP;
    public float phase3HP;


    [Header("Phase Transitions")]
    [SerializeField] private float transitionInvulnerability = 2f; // Seconds the boss ignores damage after a phase change
    [SerializeField] private int bulletClearPoints = 100; // Score per enemy bullet cleared on a phase change or death
    [SerializeField] private float returnToStartSpeed = 4f; // How fast the boss glides back to its start position for Phase 3
    [SerializeField] private float blinkInterval = 0.1f;
    private bool isInvulnerable = false;
    private Coroutine invulnerabilityRoutine;
    private Coroutine returnToStartRoutine;
    private SpriteRenderer[] bossRenderers;
    private float[] bossRendererAlphas; // Each sprite's own alpha, so the blink restores it exactly

    public bool DebugMode;

    private List<GameObject> phase1Spawners = new();
    private List<GameObject> phase2Spawners = new();
    private List<GameObject> phase3Spawners = new();

    public override void Start()
    {
        this.currentHealth = maxHealth;
        phase2P1HP = maxHealth * phase2P1Threshold;
        phase2P2HP = maxHealth * phase2P2Threshold;
        phase3HP = maxHealth * phase3Threshold;
        originalPosition = transform.position;

        // Look up the HP bar's fill image once instead of every frame
        if (idolHPBar != null)
        {
            idolFill = idolHPBar.transform.GetChild(0).GetComponent<Image>();
        }
        else
        {
            CustomDebug.LogWarning("Idol HP Bar is not assigned in the inspector.");
        }

        // Cache the boss's own sprites (before any spawners are parented to it) for the invulnerability blink
        bossRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        bossRendererAlphas = new float[bossRenderers.Length];
        for (int i = 0; i < bossRenderers.Length; i++)
        {
            bossRendererAlphas[i] = bossRenderers[i].color.a;
        }

        StartPhase1();
    }

    private void Update()
    {
        if (idolFill != null)
        {
            idolFill.fillAmount = currentHealth / maxHealth;
        }

        // Only log when debug mode switches on, not every frame
        bool debugModeNow = PlayerScoreManager.Instance.DebugMode;
        if (debugModeNow && !DebugMode)
        {
            CustomDebug.LogWarning("Debug mode is enabled. Dev Actions are enabled");
        }
        DebugMode = debugModeNow;



        fightTimer = LevelManager.Instance.timer;

        // Check for phase transitions based on health
        if (currentState == TheSunBossState.Phase1 && currentHealth <= phase2P1HP)
        {
            StartPhase2P1();
        }


        if(currentState == TheSunBossState.Phase2P1 && currentHealth <= phase2P2HP)
        {
            StartPhase2P2();
        }

        if(currentState == TheSunBossState.Phase2P2)
        {
            MoveBoss();
        }

        if (currentState == TheSunBossState.Phase2P2 && currentHealth <= phase3HP)
        {
           
            StartPhase3();
        }

        // Dev keys (Input System); Keyboard.current is null when no keyboard is connected
        var kb = UnityEngine.InputSystem.Keyboard.current;

        // Only skip forward, so pressing a key twice can't stack a phase's spawners
        if(kb != null && kb.f1Key.wasPressedThisFrame && DebugMode && currentState < TheSunBossState.Phase2P1)
        {
            StartPhase2P1();
        }

        if (kb != null && kb.f2Key.wasPressedThisFrame && DebugMode && currentState < TheSunBossState.Phase2P2)
        {
            StartPhase2P2();
        }

        if(kb != null && kb.f3Key.wasPressedThisFrame && DebugMode && currentState < TheSunBossState.Phase3)
        {
            StartPhase3();
        }
    }

    private void StartPhase1()
    {
        
        
        currentState = TheSunBossState.Phase1;
        CustomDebug.Log("The Sun Phase 1");

        // Aimed 3-way so the player can't just sit in a gap in the explosion ring
        AddAimedSpawner(phase1Spawners, 2f, 3, 20f, 5f, 4f);


        GameObject spawner5;


        spawner5 = Instantiate(spawnerPrefab, centerOfSun.position, Quaternion.Euler(0, 0, -90));

        ConfigureSpawner(
              spawner5, //Spawner Ref
              SpawnerType.Explosion, //Spawner Type
              1f, //Firing Rate
              20, // Bullet Count
              0f, // Spread Angle
              0f, // Wave Frequency
              0f, // Wave Amplitude
              0f, // Spiral Rotate Speed
              true, // Should Rotate
              30f, // Rotation Speed
              4f, // Speed
              3f, // Spawn Delay
              20f, // Bullet Life
              false //shouldHome
               );
        phase1Spawners.Add(spawner5);
        //ConfigureSpawner(spawner5, SpawnerType.)
    }

    private void StartPhase2P1()
    {
        currentState = TheSunBossState.Phase2P1;
        DestroySpawners(phase1Spawners);
        BeginPhaseTransition();
        CustomDebug.Log("The Sun Phase 2 Part 1");

        // Single aimed shots from the Sun; keeps firing into Part 2
        AddAimedSpawner(phase2Spawners, 1.5f, 1, 0f, 6f, 2f);

        GameObject spawner1;
        GameObject spawner2;
        // Spawn spawners for Phase 1
        float middleY = (PlayAreaManager.Instance.MinY + PlayAreaManager.Instance.MaxY) / 2;
        Vector3 leftMiddlePosition = new Vector3(PlayAreaManager.Instance.MinX, middleY, 0);
        Vector3 rightMiddlePosition = new Vector3(PlayAreaManager.Instance.MaxX, middleY, 0);

        // Spawn spawners at the calculated positions
         spawner1 =  Instantiate(spawnerPrefab, leftMiddlePosition, Quaternion.Euler(0, 0, 0));
         spawner2 = Instantiate(spawnerPrefab, rightMiddlePosition, Quaternion.Euler(0, 0, 180));
        phase2Spawners.Add(spawner1);
        phase2Spawners.Add(spawner2);



        ConfigureSpawner(
            spawner1, 
            SpawnerType.Explosion, 
            4f, 
            20, 
            0,
            0f,
            0f,
            0f,
            true,
            20,
            4f, 
            2f, 
            20f, 
            false);
        ConfigureSpawner(
            spawner2,
            SpawnerType.Explosion,
            4f,
            20,
            0,
            0f,
            0f,
            0f,
            true,
            20,
            4f,
            2f,
            20f,
            false);






    }

    private void StartPhase2P2()
    {
        currentState = TheSunBossState.Phase2P2;
        DestroySpawners(phase1Spawners); // Only has anything in it when F2 skips straight from Phase 1
        BeginPhaseTransition();

        // Pick a real first target; otherwise the boss heads for (0, 0) first, which is mid-screen
        PickNewTopHalfTarget();
        moveTimer = 0f;

       CustomDebug.Log("The Sun Phase 2 Part 2");

        // Wide aimed 5-way while the Sun moves around
        AddAimedSpawner(phase2Spawners, 2.5f, 5, 40f, 5f, 2f);
       
            
            GameObject spawner3;
            GameObject spawner4;

            spawner3 = Instantiate(spawnerPrefab, leftHand.position, Quaternion.Euler(0, 0, -90), this.transform);
            spawner4 = Instantiate(spawnerPrefab, rightHand.position, Quaternion.Euler(0, 0, -90), this.transform);
            phase2Spawners.Add(spawner3);
            phase2Spawners.Add(spawner4);

        



        ConfigureSpawner(
               spawner3, //Spawner Ref
               SpawnerType.Fan, //Spawner Type
               1.5f, //Firing Rate
               7, // Bullet Count
               90f, // Spread Angle
               0f, // Wave Frequency
               0f, // Wave Amplitude
               0f, // Spiral Rotate Speed
               false, // Should Rotate
               45, // Rotation Speed
               4f, // Speed
               2f, // Spawn Delay
               20f, // Bullet Life
               false //shouldHome
                );
            ConfigureSpawner(
                spawner4,
                SpawnerType.Fan,
                1.5f,
                7,
                90f,
                0f,
                0f,
                0f,
                false,
                45,
                4f,
                2f,
                20f,
                false);
        
    }

 

   

   

    private void StartPhase3()
    {
        DestroySpawners(phase1Spawners); // Only has anything in it when F3 skips straight from Phase 1
        DestroySpawners(phase2Spawners);
        BeginPhaseTransition();

        // Glide back to the start position instead of teleporting
        if (returnToStartRoutine != null) StopCoroutine(returnToStartRoutine);
        returnToStartRoutine = StartCoroutine(ReturnToStart());
        currentState = TheSunBossState.Phase3;
        CustomDebug.Log("The Sun Phase 3");

        // Fast aimed 3-way on top of the rotating explosions
        AddAimedSpawner(phase3Spawners, 2f, 3, 24f, 6f, 1f);

        GameObject spawner6;
        GameObject spawner7;

        spawner6 = Instantiate(spawnerPrefab, centerOfBounds.position, Quaternion.Euler(0, 0, -90));
        spawner7 = Instantiate(spawnerPrefab, centerOfBounds.position, Quaternion.Euler(0, 0, 0));
        phase3Spawners.Add(spawner6);
        phase3Spawners.Add(spawner7);

        ConfigureSpawner(
              spawner6, //Spawner Ref
              SpawnerType.Explosion, //Spawner Type
              0.3f, //Firing Rate
              8, // Bullet Count
              0f, // Spread Angle
              0f, // Wave Frequency
              0f, // Wave Amplitude
              0f, // Spiral Rotate Speed
              true, // Should Rotate
              10f, // Rotation Speed
              4f, // Speed
              0f, // Spawn Delay
              20f, // Bullet Life
              false //shouldHome
               );


        ConfigureSpawner(
              spawner7, //Spawner Ref
              SpawnerType.Explosion, //Spawner Type
              0.3f, //Firing Rate
              8, // Bullet Count
              0f, // Spread Angle
              0f, // Wave Frequency
              0f, // Wave Amplitude
              0f, // Spiral Rotate Speed
              true, // Should Rotate
              8f, // Rotation Speed
              4f, // Speed
              0f, // Spawn Delay
              20f, // Bullet Life
              false //shouldHome
               );
    }

    private void ConfigureSpawner(
     GameObject spawner,
     SpawnerType spawnerType,
     float firingRate,
     int bulletCount,
     float spreadAngle,
     float waveFreq,
       float waveAmp,
       float spiralRotateSpeed,
       bool shouldRotate,
       float rotationSpeed,
     float speed,
     float spawnDelay,
     float bulletLife,
     bool homingPlayer
 )
    {
        BulletSpawner spawnerScript = spawner.GetComponent<BulletSpawner>();
        if (spawnerScript == null) return;

        // Clear previous patterns and set new one
        spawnerScript.activePatterns.Clear();
        spawnerScript.activePatterns.Add(spawnerType);

        // Bullet properties
        spawnerScript.speed = speed;
        spawnerScript.bulletLife = bulletLife;
        spawnerScript.bulletCount = bulletCount;
        spawnerScript.spreadAngle = spreadAngle;

        // Firing behavior
        spawnerScript.firingRate = firingRate;
        spawnerScript.enableHoming = homingPlayer;

        // Movement/rotation modifiers
        spawnerScript.waveFrequency = waveFreq;
        spawnerScript.waveAmplitude = waveAmp;
        spawnerScript.spiralRotationSpeed = spiralRotateSpeed;
        spawnerScript.rotateSpawner = shouldRotate;
        spawnerScript.rotationSpeed = rotationSpeed;

        // Hold fire until the slide-in and delay finish (the prefab's spawner starts enabled)
        spawnerScript.enabled = false;

        // Start movement and attack coroutine
        StartCoroutine(SpawnerAttackSequence(spawner, Vector3.down, spawnDelay));
    }
    // Adds a spawner at the centre of the Sun that fires a spread aimed at the player.
    // It's parented to the boss so it follows the Sun when it moves.
    private void AddAimedSpawner(List<GameObject> phaseSpawners, float firingRate, int bulletCount, float spreadAngle, float speed, float spawnDelay)
    {
        GameObject spawner = Instantiate(spawnerPrefab, centerOfSun.position, Quaternion.identity, transform);
        phaseSpawners.Add(spawner);

        ConfigureSpawner(
            spawner,
            SpawnerType.Aimed,
            firingRate,
            bulletCount,
            spreadAngle,
            0f, // Wave Frequency
            0f, // Wave Amplitude
            0f, // Spiral Rotate Speed
            false, // Should Rotate
            0f, // Rotation Speed
            speed,
            spawnDelay,
            20f, // Bullet Life
            false // shouldHome
            );
    }

    private IEnumerator SpawnerAttackSequence(GameObject spawner, Vector3 moveDirection, float spawnerDelay)
    {
        Vector3 targetPosition = spawner.transform.position + moveDirection;
        // The spawner can be destroyed mid-slide by a phase change, so check it every frame
        while (spawner != null && Vector3.Distance(spawner.transform.position, targetPosition) > 0.1f)
        {
            spawner.transform.position = Vector3.MoveTowards(spawner.transform.position, targetPosition, Time.deltaTime * spawnerMoveDistance);
            yield return null;
        }

        if (spawner == null)
            yield break;

        // Wait for the delay
        yield return new WaitForSeconds(spawnerDelay);

        if (spawner == null)
            yield break;


        // Enable the spawner's firing behavior
        if (spawner != null)
        {
            BulletSpawner spawnerScript = spawner.GetComponent<BulletSpawner>();
            if (spawnerScript != null)
            {
                spawnerScript.enabled = true;
            }
        }
    }




    public override void TakeDamage(float damage)
    {
        // Ignore hits while the boss is changing phase
        if (isInvulnerable) return;

        base.TakeDamage(damage);
    }

    protected override void Die()
    {
        
        DestroySpawners(phase1Spawners);
        DestroySpawners(phase2Spawners);
        DestroySpawners(phase3Spawners);
        ClearEnemyBullets();
        PlayerScoreManager.Instance.AddScore(10000);
        PlayerScoreManager.Instance.CalculateScore();
        // Additional logic for boss death
        CustomDebug.Log("Boss defeated! Triggering level end...");
        base.Die();
    }

    // Clears the screen and gives the player a short window where the boss can't be damaged
    private void BeginPhaseTransition()
    {
        ClearEnemyBullets();

        if (invulnerabilityRoutine != null) StopCoroutine(invulnerabilityRoutine);
        invulnerabilityRoutine = StartCoroutine(InvulnerabilityWindow());
    }

    // Turns every enemy bullet on screen into points
    private void ClearEnemyBullets()
    {
        // Only runs on phase changes, so a scene search is fine here
        EnemyBullet[] bullets = FindObjectsByType<EnemyBullet>();
        foreach (EnemyBullet bullet in bullets)
        {
            bullet.ReturnToPool();
        }

        if (bullets.Length > 0)
        {
            PlayerScoreManager.Instance.AddScore(bullets.Length * bulletClearPoints);
        }
    }

    private IEnumerator InvulnerabilityWindow()
    {
        isInvulnerable = true;

        // Blink the boss so the player can see it isn't taking damage
        float elapsed = 0f;
        bool dimmed = false;
        while (elapsed < transitionInvulnerability)
        {
            dimmed = !dimmed;
            SetBossAlpha(dimmed ? 0.4f : 1f);
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        SetBossAlpha(1f);
        isInvulnerable = false;
        invulnerabilityRoutine = null;
    }

    // Sets each boss sprite to its own alpha times the multiplier (1 = normal)
    private void SetBossAlpha(float multiplier)
    {
        if (bossRenderers == null) return;
        for (int i = 0; i < bossRenderers.Length; i++)
        {
            SpriteRenderer sr = bossRenderers[i];
            if (sr == null) continue;
            Color c = sr.color;
            c.a = bossRendererAlphas[i] * multiplier;
            sr.color = c;
        }
    }

    private IEnumerator ReturnToStart()
    {
        while (Vector3.Distance(transform.position, originalPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, returnToStartSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = originalPosition;
        returnToStartRoutine = null;
    }

    private void DestroySpawners(List<GameObject> spawnerList)
    {
        foreach (var spawner in spawnerList)
        {
            if (spawner != null)
            {
                Destroy(spawner);
            }
        }
        spawnerList.Clear();
    }

    private void MoveBoss()
    {
        if (currentState == TheSunBossState.Phase2P2)
        {
            moveTimer += Time.deltaTime;

            // Move towards target
            transform.position = Vector3.MoveTowards(transform.position, targetMovePosition, bossMoveSpeed * Time.deltaTime);

            // If reached or time to pick new target
            if (moveTimer >= moveInterval || Vector3.Distance(transform.position, targetMovePosition) < 0.1f)
            {
                PickNewTopHalfTarget();
                moveTimer = 0f;
            }
        }
    }

    private void PickNewTopHalfTarget()
    {
        float midY = (PlayAreaManager.Instance.MinY + PlayAreaManager.Instance.MaxY) / 2;
        float x = Random.Range(PlayAreaManager.Instance.MinX, PlayAreaManager.Instance.MaxX);
        float y = Random.Range(midY, PlayAreaManager.Instance.MaxY);
        targetMovePosition = new Vector3(x, y, transform.position.z);
    }

}
