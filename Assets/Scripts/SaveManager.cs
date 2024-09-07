using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public GameConfig gameConfig; // Assign this via inspector

    public void SaveGameConfigToJson()
    {
        // Create a wrapper instance from the ScriptableObject
        GameConfigWrapper wrapper = new GameConfigWrapper(gameConfig);

        // Serialize the wrapper to JSON
        string json = JsonUtility.ToJson(wrapper, true);
        Debug.Log(json);
        // Define the path where you want to save the JSON file
        string filePath = Path.Combine(Application.persistentDataPath, "gameConfig.json");
        Debug.Log("GameConfig saved to " + filePath);
        // Write the JSON to the file
        File.WriteAllText(filePath, json);

        Debug.Log("GameConfig saved to " + filePath);
    }
}
