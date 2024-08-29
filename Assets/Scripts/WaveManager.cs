using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public List<GameObject> enemyPrefabs; // List of enemy prefabs
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 15f;
    public float spawnRate = 1f;

    public List<List<int>> waves = new List<List<int>>(); // 2D list to define each wave's enemy sequence

    private int currentWave = 0;
    private int enemiesToSpawn;
    private int enemiesAlive;
    private bool waveInProgress;

    private float waveTimer;
    private float spawnTimer;
    private int spawnIndex;

    public WaveUIManager waveUIManager; // Reference to the WaveUIManager
    private Player player; // Reference to the player object

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        // Define the waves with the indices of the enemies to spawn
        waves.Add(new List<int> { 0, 0, 0, 0 }); //0 == enemy1, 1 == enemy2
        waves.Add(new List<int> { 0, 0, 0, 1 });
        waves.Add(new List<int> { 0, 0, 0, 1, 1 });
        waves.Add(new List<int> { 0, 0, 0, 1, 1, 1 });
        waves.Add(new List<int> { 0, 0, 0, 1, 1, 0, 0, 0 });

        waveTimer = timeBetweenWaves;
        waveInProgress = false;
    }

    void Update()
    {
        if (!waveInProgress)
        {
            waveTimer -= Time.deltaTime;
            if (waveTimer <= 0f)
            {
                StartNextWave();
            }
        }
        else
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f && enemiesToSpawn > 0)
            {
                SpawnEnemy();
                spawnTimer = spawnRate;
            }

            // Check if the wave is completed
            if (enemiesToSpawn == 0 && enemiesAlive == 0)
            {
                waveInProgress = false;
                waveTimer = timeBetweenWaves; // Reset the wave timer for the next wave

                // Check for alive allies and revive the player if necessary
                if (CheckForAliveAllies())
                {
                    RevivePlayer();
                }
            }
        }

        UpdateWaveScore();
    }

    void StartNextWave()
    {
        ClearExistingEnemies();
        ClearExistingAllies();

        if (currentWave < waves.Count)
        {
            currentWave++;
            spawnIndex = 0;
            enemiesToSpawn = waves[currentWave - 1].Count; // Set the number of enemies to spawn based on the current wave
            enemiesAlive = 0;
            waveInProgress = true;
            waveTimer = 0f;
            spawnTimer = 0f; // Start spawning immediately

            waveUIManager.UpdateWaveScore(currentWave); // Update the wave score display
            waveUIManager.ShowWavePanel(); // Show the wave panel
        }
        else
        {
            Debug.Log("All waves completed!");
        }
    }

    void SpawnEnemy()
    {
        if (spawnIndex < waves[currentWave - 1].Count)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            int enemyIndex = waves[currentWave - 1][spawnIndex]; // Get the enemy index for this spawn
            GameObject enemyPrefab = enemyPrefabs[enemyIndex];

            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            enemiesToSpawn--;
            enemiesAlive++;
            spawnIndex++;

            BaseCharacter baseCharacter = enemy.GetComponent<BaseCharacter>();
            if (baseCharacter != null)
            {
                baseCharacter.onDeath += OnEnemyDeath;
            }
        }
    }

    void OnEnemyDeath()
    {
        enemiesAlive--;
    }

    void ClearExistingEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
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

    bool CheckForAliveAllies()
    {
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
        foreach (GameObject ally in allies)
        {
            Ally allyHealth = ally.GetComponent<Ally>();
            if (allyHealth != null && allyHealth.IsAlive)
            {
                return true; // Found an alive ally
            }
        }
        return false; // No alive allies found
    }

    void RevivePlayer()
    {
        Debug.Log("trying to revive revived!");
        if (player != null && !player.IsAlive)
        {
            player.Revive();
            Debug.Log("Player revived!");
        }
    }

    void UpdateWaveScore()
    {
        // This method can be removed or adjusted if using WaveUIManager for score updates
    }
}
