using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Config")]
public class GameConfig : ScriptableObject
{
    public bool deleteFiles;

    public int bestWave;
    public int moneyAmount;

    //LAST GAME DATA
    public int lastGameWave;//updated from the previous game, so player moves
    public bool hasToMove; //indicates if hast to move after a game or it should show up in the wave
}
