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


    void Start()
    {
        Instance = this;
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        waveUIManager = GetComponent<WaveUIManager>();

        this.customTimer = GetComponent<CustomTimer>();
        InitializeWaves();

        // Start the wave spawning process
        StartWave();
    }

    void Update()
    {
       
    }

    void StartWave()
    {
        
        if (currentWave < waves.Count)
        {
            waveUIManager.UpdateWaveScore(currentWave);
            waveUIManager.ShowWavePanel();
            Debug.Log($"Starting Wave {currentWave + 1}");

            // Spawn all enemies for the current wave
            List<EnemyWave> wave = waves[currentWave];
            foreach (EnemyWave enemyWave in wave)
            {
                for (int i = 0; i <= enemyWave.quantity; i++)
                {
                    float delay = i * spawnRate;

                    // Spawn enemies with a delay between each
                    customTimer.StartTimer(delay, () =>
                    {
                        SpawnEnemy(enemyWave.enemyPrefab);
                    });
                }
            }

            customTimer.StartTimer(timeBetweenWaves, () =>
            {
                currentWave++;

                StartWave();  // Call the next wave after the timer
            });

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
            new EnemyWave(enemyPrefabs[0], 5)
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
