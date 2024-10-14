using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class PrefabStatsManager : MonoBehaviour
{
    public static PrefabStatsManager Instance { get; private set; }

    public PrefabStatsCollection prefabStatsCollection; // Holds the main prefab stats

    private string filePath;

    // The single list of default prefabs
    private List<PrefabStats> defaultPrefabs = new List<PrefabStats>
    {
        new PrefabStats {
            prefabName = "Player",
            health = 150,
            attackPower = 10,
            attackRange = 25,
            speed = 45,
            levelUpgrade = 0,
            baseCost = 100,            // Set the base cost for Player_1 upgrades
            costMultiplier = 1.2f,     // Set the cost multiplier for Player_1 upgrades
            skinSelected = "male",
            listOfSkins = new string[] { "male", "female" }

        },
        new PrefabStats {
            prefabName = "Ally_Tank",
            health = 700,
            attackPower = 10,
            attackRange = 15,
            speed = 25,
            levelUpgrade = 0,
            baseCost = 150,            // Set the base cost for Ally_Tank upgrades
            costMultiplier = 1.3f,
            skinSelected = null,
            listOfSkins =  new string[]{ }
        },
        new PrefabStats {
            prefabName = "Ally_Space",
            health = 75,
            attackPower = 20,
            attackRange = 100,
            speed = 30,
            levelUpgrade = 0,
            baseCost = 120,            // Set the base cost for Ally_Space upgrades
            costMultiplier = 1.4f,      // Set the cost multiplier for Ally_Space upgrades
            skinSelected = null,
            listOfSkins =  new string[]{ }
        }
        // Add more default prefabs here as needed
    };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy this instance if it is not the singleton instance
            return;
        }
        Instance = this;
        // Define the path where the JSON file will be saved
        filePath = Path.Combine(Application.persistentDataPath, "prefabs_stats.json");

        // Load the stats at the start of the game
        //LoadStatsFromJson();

        // Check for missing prefabs and add them if necessary
        //CheckAndAddMissingPrefabs();
    }

    // Load stats from JSON or create default if not found
    public void LoadStatsFromJson()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            prefabStatsCollection = JsonUtility.FromJson<PrefabStatsCollection>(json);
            Debug.Log("Stats loaded from " + filePath);
        }
        else
        {
            Debug.LogWarning("No stats file found at " + filePath + ". Creating default stats...");

            // If no file is found, initialize with the default list
            prefabStatsCollection = new PrefabStatsCollection { prefabsStats = new List<PrefabStats>(defaultPrefabs) };

            // Save the default stats to the file
            SaveStatsToJson();
        }
    }

    // Check for missing prefabs and add them if necessary
    public void CheckAndAddMissingPrefabs()
    {
        bool statsUpdated = false;

        foreach (var defaultPrefab in defaultPrefabs)
        {
            // Check if the current prefab exists in the loaded collection
            var existingPrefab = prefabStatsCollection.prefabsStats.Find(p => p.prefabName == defaultPrefab.prefabName);
            if (existingPrefab == null)
            {
                // Add the missing prefab to the collection
                prefabStatsCollection.prefabsStats.Add(defaultPrefab);
                statsUpdated = true;
                Debug.Log($"Added missing prefab: {defaultPrefab.prefabName}");
            }
        }

        // If any prefabs were added, save the updated stats to the JSON file
        if (statsUpdated)
        {
            SaveStatsToJson();
        }
    }

    public void SaveStatsToJson()
    {
        string json = JsonUtility.ToJson(prefabStatsCollection, true); // Pretty print for readability
        File.WriteAllText(filePath, json);
        Debug.Log("Stats saved to " + filePath);
    }

    // Get stats for a specific prefab by name
    public PrefabStats GetPrefabStats(string prefabName)
    {
        // Remove "(clone)" from the prefabName if it exists
        string cleanedName = prefabName.Replace("(Clone)", "").Trim();

        return prefabStatsCollection.prefabsStats.Find(p => p.prefabName == cleanedName);
    }
    // Method to calculate upgrade cost based on prefab's baseCost and costMultiplier
    public int CalculateUpgradeCost(PrefabStats prefabStats)
    {
        return Mathf.RoundToInt(prefabStats.baseCost * Mathf.Pow(prefabStats.costMultiplier, prefabStats.levelUpgrade));
    }

    public bool TryUpgradePrefab(string prefabName)
    {
        PrefabStats prefabStats = GetPrefabStats(prefabName);

        int upgradeCost = CalculateUpgradeCost(prefabStats);
        Debug.Log("Cost of update for " + prefabName + " is " + upgradeCost);
        // Check if player has enough resources to upgrade
        if (GameManagerMainMenu.Instance.GetAmountOfMoney() >= upgradeCost)
        {
            // Deduct the cost
            GameManagerMainMenu.Instance.DeductMoney(upgradeCost);

            // Apply the upgrade
            UpgradePrefab(prefabStats);

            return true; // Upgrade successful
        }
        else
        {
            Debug.Log("Not enough coins to upgrade " + prefabStats.prefabName);
            return false; // Not enough resources
        }
    }

    // Update stats for a specific prefab by name
    public void UpgradePrefab(PrefabStats prefabStats)
    {

        switch (prefabStats.prefabName)
        {
            case "Ally_Tank":
                UpgradeTank(prefabStats);
                break;

            case "Player":
                UpgradePlayer(prefabStats);
                break;

            case "Ally_Space":
                UpgradeAllySpace(prefabStats);
                break;

            // Add other prefabs as needed
            default:
                Debug.LogWarning("Unknown prefab name for upgrade.");
                break;
        }

        // After upgrade, save the updated stats
        SaveStatsToJson();
    }

    public string GetSkinSelected(string prefabName)
    {
        return GetPrefabStats(prefabName).skinSelected;
    }

    public void SetSkinSelected(string prefabName, string newSkin)
    {
        PrefabStats stats = GetPrefabStats(prefabName);
        stats.skinSelected = newSkin;

        SaveStatsToJson();
    }

    public string[] GetListOfSkins(string prefabName)
    {
        PrefabStats stats = GetPrefabStats(prefabName);

        return stats.listOfSkins;
    }

    private void UpgradeTank(PrefabStats prefabStats)
    {
        prefabStats.health += 100;  // More health on upgrade
        prefabStats.attackPower += 5;
        prefabStats.speed += 2;     // Slight increase in speed
        prefabStats.levelUpgrade++;

        Debug.Log("Upgraded Tank: " + prefabStats.prefabName);
    }

    // Logic to upgrade Player_1: balanced upgrade
    private void UpgradePlayer(PrefabStats prefabStats)
    {
        prefabStats.health += 50;   // Balanced upgrades
        prefabStats.attackPower += 10;
        prefabStats.attackRange += 5;
        prefabStats.speed += 5;
        prefabStats.levelUpgrade++;

        Debug.Log("Upgraded Player: " + prefabStats.prefabName);
    }

    // Logic to upgrade Ally_Space: more attack focused
    private void UpgradeAllySpace(PrefabStats prefabStats)
    {
        prefabStats.health += 20;   // Minimal health increase
        prefabStats.attackPower += 15; // Major attack power boost
        prefabStats.attackRange += 10;
        prefabStats.speed += 3;
        prefabStats.levelUpgrade++;

        Debug.Log("Upgraded Ally_Space: " + prefabStats.prefabName);
    }
}
