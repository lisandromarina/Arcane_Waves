using UnityEngine;

public class GameManagerMainMenu : MonoBehaviour
{

    public GameConfig gameConfig;

    void Start()
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
}