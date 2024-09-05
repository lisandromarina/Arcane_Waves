using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Configuration")]
    public List<GameObject> enemyPrefabs;
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 15f;
    public float spawnRate = 1f;
    public List<List<EnemyWave>> waves = new List<List<EnemyWave>>();

    private int currentWave = 0;
    private int enemiesAlive;
    private bool waveInProgress;
    private float waveTimer;
    private float spawnTimer;

    private Queue<EnemyWave> enemySpawnQueue = new Queue<EnemyWave>();

    [Header("References")]
    public WaveUIManager waveUIManager;
    private Player player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        InitializeWaves();
        ResetWaveTimer();
    }

    void Update()
    {
        if (waveInProgress)
        {
            HandleSpawning();
            CheckWaveCompletion();
        }
        else
        {
            HandleWaveStart();
        }
    }

    private void InitializeWaves()
    {
        waves.Add(new List<EnemyWave> {
            new EnemyWave(enemyPrefabs[0], 1)
        });

        waves.Add(new List<EnemyWave> { 
            new EnemyWave(enemyPrefabs[0], 10),
            new EnemyWave(enemyPrefabs[1], 10),
        });
        
        waves.Add(new List<EnemyWave> { 
            new EnemyWave(enemyPrefabs[2], 1), 
            new EnemyWave(enemyPrefabs[3], 1) 
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
            new EnemyWave(enemyPrefabs[3], 1)
        });
    }

    private void HandleWaveStart()
    {
        waveTimer -= Time.deltaTime;
        if (waveTimer <= 0f)
        {
            StartNextWave();
        }
    }

    private void HandleSpawning()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f && enemySpawnQueue.Count > 0)
        {
            SpawnEnemyFromQueue();
            spawnTimer = spawnRate;
        }
    }

    private void CheckWaveCompletion()
    {
        if (enemySpawnQueue.Count == 0 && enemiesAlive == 0)
        {
            EndCurrentWave();
        }
    }

    private void StartNextWave()
    {
        ClearExistingCharacters("Enemy");
        ClearExistingAllies();

        if (currentWave < waves.Count)
        {
            PrepareWave();
        }
        else
        {
            GameManager.Instance.SaveGameData();
            Debug.Log("All waves completed!");
            Loader.Load(Loader.Scene.MainMenu);
        }
    }

    private void PrepareWave()
    {
        currentWave++;
        foreach (EnemyWave enemyWave in waves[currentWave - 1])
        {
            for (int i = 0; i < enemyWave.quantity; i++)
            {
                enemySpawnQueue.Enqueue(enemyWave);
            }
        }
        ResetWaveState();
        waveUIManager.UpdateWaveScore(currentWave);
        waveUIManager.ShowWavePanel();
    }

    private void ResetWaveState()
    {
        enemiesAlive = 0;
        waveInProgress = true;
        waveTimer = 0f;
        spawnTimer = 0f;
    }

    private void EndCurrentWave()
    {
        waveInProgress = false;
        ResetWaveTimer();
        if (CheckForAliveAllies())
        {
            RevivePlayer();
        }
    }

    private void ResetWaveTimer() => waveTimer = timeBetweenWaves;

    private void SpawnEnemyFromQueue()
    {
        if (enemySpawnQueue.Count > 0)
        {
            EnemyWave enemyWave = enemySpawnQueue.Dequeue();
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemy = Instantiate(enemyWave.enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            enemiesAlive++;
            SubscribeToEnemyDeath(enemy);
        }
    }

    private void SubscribeToEnemyDeath(GameObject enemy)
    {
        BaseCharacter baseCharacter = enemy.GetComponent<BaseCharacter>();
        if (baseCharacter != null)
        {
            baseCharacter.onDeath += OnEnemyDeath;
        }
    }

    void OnEnemyDeath() => enemiesAlive--;

    private void ClearExistingCharacters(string tag)
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject character in characters)
        {
            Destroy(character);
        }
    }

    public void ClearExistingAllies()
    {
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
        foreach (GameObject ally in allies)
        {
            Health allyHealth = ally.GetComponent<Health>();
            if (allyHealth != null && !allyHealth.IsAlive)
            {
                Destroy(ally);
            }
        }
    }

    private bool CheckForAliveAllies()
    {
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
        foreach (GameObject ally in allies)
        {
            Ally allyComponent = ally.GetComponent<Ally>();
            if (allyComponent != null && allyComponent.IsAlive)
            {
                return true;
            }
        }
        return false;
    }

    private void RevivePlayer()
    {
        if (player != null && !player.IsAlive)
        {
            player.Revive();
            Debug.Log("Player revived!");
        }
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }
}
