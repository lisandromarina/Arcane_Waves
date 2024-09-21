using UnityEngine;
using System.Collections.Generic;
using System;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("Wave Configuration")]
    public List<List<EnemyWave>> waves = new List<List<EnemyWave>>();
    public Transform[] spawnPoints;
    public float spawnRate = 50f;
    public float timeBetweenWaves = 50f;
    public List<GameObject> enemyPrefabs = new List<GameObject>();

    private int currentWave = 0;
    private Player player;

    private CustomTimer customTimer;

    private WaveUIManager waveUIManager;
    private bool isStarted = false;


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

                            // After all enemies are spawned, start timer for the next wave
                            customTimer.StartTimer(timeBetweenWaves, () =>
                            {
                                currentWave++;
                                StartWave();  // Start the next wave
                            });
                        }
                    });
                }
            }
        }
        else
        {
            Debug.Log("All waves completed!");
        }
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        Debug.Log("Spawn");
        int randomSpawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomSpawnIndex];
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
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

    }

    public int GetCurrentWave()
    {
        return currentWave;
    }
}
