using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("Wave Configuration")]
    public List<List<EnemyWave>> waves = new List<List<EnemyWave>>();
    public Transform[] spawnPoints;
    public float spawnRate = 50f;
    public float timeBetweenWaves = 500f;
    public List<GameObject> enemyPrefabs = new List<GameObject>();

    private int currentWave = 0;
    private Player player;

    private CustomTimer customTimer;

    private WaveUIManager waveUIManager;
    private bool isStarted = false;

    private List<GameObject> activeEnemies = new List<GameObject>();

    private bool allWavesCompleted = false;


    void Start()
    {
        Instance = this;
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        waveUIManager = GetComponent<WaveUIManager>();

        this.customTimer = GetComponent<CustomTimer>();
        InitializeWaves();
    }

    void Update()
    {
        if (!isStarted)
        {
            // Start the wave spawning process
            StartWave();
            isStarted = true;
        }
        if (allWavesCompleted && activeEnemies.Count == 0)
        {
            Debug.Log("All waves and enemies are completed!");
            // Perform any final actions when all waves and enemies are defeated
            GameCompleted();
        }
    }

    void StartWave()
    {
        if (currentWave < waves.Count)
        {
            waveUIManager.UpdateWaveScore(currentWave + 1);
            waveUIManager.ShowWavePanel();
            Debug.Log($"Starting Wave {currentWave + 1}");

            // Get the current wave's enemy list
            List<EnemyWave> wave = waves[currentWave];
            int totalEnemies = 0;  // Counter to track total enemies to be spawned
            int spawnedEnemies = 0;  // Counter to track how many enemies have been spawned
            bool isBossWave = (currentWave + 1) % 5 == 0; // Check if it's a boss wave

            foreach (EnemyWave enemyWave in wave)
            {
                totalEnemies += enemyWave.quantity;  // Count total enemies in this wave

                for (int i = 0; i < enemyWave.quantity; i++)
                {
                    float delay = i * spawnRate;

                    // Spawn enemies with a delay between each
                    customTimer.StartTimer(delay, () =>
                    {
                        SpawnEnemy(enemyWave.enemyPrefab);
                        spawnedEnemies++;

                        // Check if all enemies have been spawned
                        if (spawnedEnemies == totalEnemies)
                        {
                            Debug.Log("All enemies spawned, preparing next wave.");

                            // If it's a boss wave, wait until all enemies are dead before proceeding
                            if (isBossWave)
                            {
                                StartCoroutine(WaitForAllEnemiesDefeated(() =>
                                {
                                    Debug.Log("Boss and all enemies defeated. Starting the next wave.");
                                    currentWave++;
                                    StartWave();
                                }));
                            }
                            else
                            {
                                // After all enemies are spawned, start timer for the next wave
                                customTimer.StartTimer(timeBetweenWaves, () =>
                                {
                                    Debug.Log("Next wave started");
                                    currentWave++;
                                    StartWave();  // Start the next wave
                                });
                            }
                        }
                    });
                }
            }
        }
        else
        {
            Debug.Log("All waves completed!");
            allWavesCompleted = true;
        }
    }

    // Coroutine to wait until all enemies are defeated before proceeding
    IEnumerator WaitForAllEnemiesDefeated(Action onAllEnemiesDefeated)
    {
        // Wait until the activeEnemies list is empty
        while (activeEnemies.Count > 0)
        {
            yield return null; // Wait for the next frame
        }

        // All enemies are defeated, invoke the next step
        onAllEnemiesDefeated.Invoke();
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        Debug.Log("Spawn");
        int randomSpawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomSpawnIndex];

        // Instantiate the enemy and add it to the activeEnemies list
        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        activeEnemies.Add(spawnedEnemy);

        // Add a listener to the enemy's death event to remove it from the list when it dies
        spawnedEnemy.GetComponent<Enemy>().onDeath += () =>
        {
            activeEnemies.Remove(spawnedEnemy);  // Remove enemy from list when it dies
        };
    }

    private void InitializeWaves()
    {
       waves.Add(new List<EnemyWave> {
            new EnemyWave(enemyPrefabs[0], 15)
        });

      waves.Add(new List<EnemyWave> {
            new EnemyWave(enemyPrefabs[0], 5),
            new EnemyWave(enemyPrefabs[1], 3),
            new EnemyWave(enemyPrefabs[0], 5),
        });

        waves.Add(new List<EnemyWave> {
            new EnemyWave(enemyPrefabs[2], 2),
            new EnemyWave(enemyPrefabs[3], 2),
            new EnemyWave(enemyPrefabs[2], 2),
            new EnemyWave(enemyPrefabs[3], 2),
            new EnemyWave(enemyPrefabs[2], 2),
            new EnemyWave(enemyPrefabs[3], 2)
        });

        waves.Add(new List<EnemyWave> {
            new EnemyWave(enemyPrefabs[0], 1),
            new EnemyWave(enemyPrefabs[2], 1),
            new EnemyWave(enemyPrefabs[3], 1),
            new EnemyWave(enemyPrefabs[0], 1),
            new EnemyWave(enemyPrefabs[2], 1),
            new EnemyWave(enemyPrefabs[3], 1),
            new EnemyWave(enemyPrefabs[0], 1),
            new EnemyWave(enemyPrefabs[2], 1),
            new EnemyWave(enemyPrefabs[3], 1),
            new EnemyWave(enemyPrefabs[0], 1),
            new EnemyWave(enemyPrefabs[2], 1),
            new EnemyWave(enemyPrefabs[0], 1),
            new EnemyWave(enemyPrefabs[2], 1),
            new EnemyWave(enemyPrefabs[0], 1),
            new EnemyWave(enemyPrefabs[2], 1)
        });

        waves.Add(new List<EnemyWave> {
            new EnemyWave(enemyPrefabs[4], 1) //boss
        });

        waves.Add(new List<EnemyWave> {
            new EnemyWave(enemyPrefabs[5], 5)
        });

        waves.Add(new List<EnemyWave> {
            new EnemyWave(enemyPrefabs[5], 5),
            new EnemyWave(enemyPrefabs[6], 15)
        });

        waves.Add(new List<EnemyWave> {
            new EnemyWave(enemyPrefabs[5], 5),
            new EnemyWave(enemyPrefabs[6], 10),
            new EnemyWave(enemyPrefabs[3], 8),
             new EnemyWave(enemyPrefabs[2], 12)

        });

        waves.Add(new List<EnemyWave> {
            new EnemyWave(enemyPrefabs[5], 5),
            new EnemyWave(enemyPrefabs[6], 15),
            new EnemyWave(enemyPrefabs[2], 10)
        });

        waves.Add(new List<EnemyWave> {
            new EnemyWave(enemyPrefabs[8], 1)
        });

    }

    public int GetCurrentWave()
    {
        return currentWave;
    }

    public void GameCompleted()
    {
        customTimer.StartTimer(5f, () =>
        {
            GameManager.Instance.SaveGameData();
            Debug.Log("All waves completed!");
            Loader.Load(Loader.Scene.MainMenu);

        });
    }
}
