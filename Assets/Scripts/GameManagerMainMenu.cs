using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManagerMainMenu : MonoBehaviour
{
    // Singleton instance
    public static GameManagerMainMenu Instance;

    public GameConfig gameConfig;
    public SaveManager saveManager;
    public LoadManager loadManager;

    [SerializeField] private GameObject playerPrefab;
    private PlayerMainMenu player;

    private void Awake()
    {
        Instance = this;

        GameObject playerGO = RespawnPrefab(playerPrefab, Vector2.zero);
        GameObject gridGO = GameObject.Find("AdventurePanel");

        player = playerGO.GetComponent<PlayerMainMenu>();
        if (gridGO != null)
        {
            // Set the player's parent to the grid GameObject's transform
            playerGO.transform.SetParent(gridGO.transform, false);
        }
        else
        {
            Debug.LogWarning("Grid GameObject not found. Make sure it exists in the scene.");
        }
    }

    private void Start()
    {
        InitializeGame();
        SoundManager.PlaySound(SoundManager.Sound.PlayMainMenuMusic, true);
    }

    private void InitializeGame()
    {
        loadManager.LoadGameConfigFromJson();


        if (gameConfig.bestWave < gameConfig.lastGameWave)
        {
            if (!gameConfig.hasToMove)
            {
                gameConfig.bestWave = gameConfig.lastGameWave;
            }
            else
            {
                Debug.Log("gameConfig.lastGameWave " + gameConfig.lastGameWave);
                player.StartMovement(gameConfig.lastGameWave);
            }
        }
        gameConfig.hasToMove = false;

        saveManager.SaveGameConfigToJson();
    }

    public GameObject RespawnPrefab(GameObject prefabs, Vector3 position)
    {
        // Instantiate the prefab at the specified spawn point
        return Instantiate(prefabs, position, Quaternion.identity);
    }

}
