using System.IO;
using UnityEngine;

public class LoadManager : MonoBehaviour
{
    public GameConfig gameConfig; // Assign this via inspector

    public void LoadGameConfigFromJson()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "gameConfig.json");

        if (File.Exists(filePath))
        {
            // Read the JSON from the file
            string json = File.ReadAllText(filePath);

            // Deserialize the JSON into the wrapper
            GameConfigWrapper wrapper = JsonUtility.FromJson<GameConfigWrapper>(json);

            // Update the ScriptableObject
            gameConfig.bestWave = wrapper.bestWave;
            gameConfig.moneyAmount = wrapper.moneyAmount;
            gameConfig.lastGameWave = wrapper.lastGameWave;
            gameConfig.hasToMove = wrapper.hasToMove;

            Debug.Log("GameConfig loaded from " + filePath);
        }
        else
        {
            Debug.LogWarning("No data file found at " + filePath);
        }
    }
}
