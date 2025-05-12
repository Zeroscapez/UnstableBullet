using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum TheSunBossState
{
    Phase1 = 0, Phase2P1 = 1, Phase2P2 = 2, Phase3 = 3
}
public class TheSun : Enemy
{
    [Header("Spawner Settings")]
    public GameObject spawnerPrefab;
    public float spawnerMoveDistance = 5f;
    public float spawnerDelay = 2f;
    public Transform leftHand;
    public Transform rightHand;
    public Transform centerOfSun;
    public Transform centerOfBounds;

    [Header("Boss Settings")]
    public Transform player;


    [Header("Movement Settings")]
    public int bossMoveSpeed = 1;
    public float moveInterval = 3f;
    private float moveTimer = 0f;
    private Vector3 originalPosition;
    private Vector3 targetMovePosition;


    [Header("State and Timer")]
    public TheSunBossState currentState = TheSunBossState.Phase1; 
    public float fightTimer = 0f;
    public float phase2P1HP;
    public float phase2P2HP;
    public float phase3HP;

    private GameObject leftSpawner;
    private GameObject rightSpawner;

    public bool DebugMode;

    private List<GameObject> phase1Spawners = new();
    private List<GameObject> phase2Spawners = new();
    private List<GameObject> phase3Spawners = new();

    public override void Start()
    {
        this.currentHealth = maxHealth;
        phase2P1HP = maxHealth * 0.8f;
        phase2P2HP = maxHealth * 0.5f;
        phase3HP = maxHealth * 0.3f;
        originalPosition = transform.position;
       
        StartPhase1();
    }

    private void Update()
    {
        DebugMode = PlayerScoreManager.Instance.DebugMode;
        if (DebugMode)
        {
            
            Debug.LogWarning("Debug mode is enabled. Dev Actions are enabled");
        }

        fightTimer = LevelManager.Instance.timer;

        // Check for phase transitions based on health
        if (currentState == TheSunBossState.Phase1 && currentHealth <= (maxHealth * 0.8))
        {
            StartPhase2P1();
        }


        if(currentState == TheSunBossState.Phase2P1 && currentHealth <= (maxHealth * 0.5))
        {
            StartPhase2P2();
        }

        if(currentState == TheSunBossState.Phase2P2)
        {
            MoveBoss();
        }

        if (currentState == TheSunBossState.Phase2P2 && currentHealth <= (maxHealth * 0.3))
        {
           
            StartPhase3();
        }

        if(Input.GetKeyDown(KeyCode.F1) && DebugMode)
        {
            StartPhase2P1();
        }

        if (Input.GetKeyDown(KeyCode.F2) && DebugMode)
        {
            StartPhase2P2();
        }

        if(Input.GetKeyDown(KeyCode.F3) && DebugMode)
        {
            StartPhase3();
        }
    }

    private void StartPhase1()
    {
        
        
        currentState = TheSunBossState.Phase1;
        Debug.Log("The Sun Phase 1");


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
              1f, // Spawn Delay
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
        Debug.Log("The Sun Phase 2 Part 1");

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
        
        Debug.Log("The Sun Phase 2 Part 2");
       
            
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
        DestroySpawners(phase2Spawners);
        transform.position = originalPosition;
        currentState = TheSunBossState.Phase3;
        Debug.Log("The Sun Phase 3");

        GameObject spawner6;
        GameObject spawner7;

        spawner6 = Instantiate(spawnerPrefab, centerOfBounds.position, Quaternion.Euler(0, 0, -90));
        spawner7 = Instantiate(spawnerPrefab, centerOfBounds.position, Quaternion.Euler(0, 0, 0));

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

        // Start movement and attack coroutine
        StartCoroutine(SpawnerAttackSequence(spawner, Vector3.down, spawnDelay));
    }
    private IEnumerator SpawnerAttackSequence(GameObject spawner, Vector3 moveDirection, float spawnerDelay)
    {
        Vector3 targetPosition = spawner.transform.position + moveDirection;
        while (Vector3.Distance(spawner.transform.position, targetPosition) > 0.1f)
        {
            spawner.transform.position = Vector3.MoveTowards(spawner.transform.position, targetPosition, Time.deltaTime * spawnerMoveDistance);
            yield return null;
        }

        // Wait for the delay
        yield return new WaitForSeconds(spawnerDelay);

        // Enable the spawner's firing behavior
        BulletSpawner spawnerScript = spawner.GetComponent<BulletSpawner>();
        if (spawnerScript != null)
        {
            spawnerScript.enabled = true;
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        // Additional logic for taking damage (e.g., visual effects)
       
    }

    protected override void Die()
    {
        base.Die();

        // Additional logic for boss death
        Debug.Log("Boss defeated! Triggering level end...");
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
