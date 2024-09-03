using UnityEngine;

public class GameManagerMainMenu : MonoBehaviour
{
    // Singleton instance
    public static GameManagerMainMenu Instance;

    public GameConfig gameConfig;
    private bool isFirstLoad;

    [SerializeField] private GameObject playerPrefab;
    private PlayerMainMenu player;
    private void Awake()
    {
        // Check if an instance already exists
        GameObject playerGO = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity);
        player = playerGO.GetComponent<PlayerMainMenu>();
        if (Instance == null)
        {
            Instance = this; // Set the instance to the current object
            isFirstLoad = true;
            Debug.Log("First Instance");
            DontDestroyOnLoad(gameObject); // Make this object persistent across scenes
        }
        else
        {
            Debug.Log("Duplicate GameManagerMainMenu instance detected and destroyed.");
            Destroy(gameObject); // Destroy any duplicate instances
            player.StartMovement(gameConfig.lastGameWave);
            isFirstLoad = false;
        }
    }

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        if (gameConfig != null)
        {
            Debug.Log($"bestWave: {gameConfig.bestWave}");
            Debug.Log($"moneyAmount: {gameConfig.moneyAmount}");
            // Initialize other game settings
        }
    }

    public bool IsFirstLoad()
    {
        return isFirstLoad;
    }

}
