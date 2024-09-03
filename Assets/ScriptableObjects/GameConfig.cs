using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Config")]
public class GameConfig : ScriptableObject
{
    public int bestWave;
    public int moneyAmount;

    //LAST GAME DATA
    public int lastGameWave;//updated from the previous game, so player moves
}
