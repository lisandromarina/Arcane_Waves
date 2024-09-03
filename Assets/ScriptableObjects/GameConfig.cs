using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Config")]
public class GameConfig : ScriptableObject
{
    public int bestWave;
    public int moneyAmount;

    // Add other game settings here
}
