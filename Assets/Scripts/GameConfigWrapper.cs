[System.Serializable]
public class GameConfigWrapper
{
    public int bestWave;
    public int moneyAmount;
    public int lastGameWave;
    public bool hasToMove;

    // Constructor to initialize the wrapper from a ScriptableObject
    public GameConfigWrapper(GameConfig gameConfig)
    {
        bestWave = gameConfig.bestWave;
        moneyAmount = gameConfig.moneyAmount;
        lastGameWave = gameConfig.lastGameWave;
        hasToMove = gameConfig.hasToMove;
    }
}