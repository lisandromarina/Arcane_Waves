using UnityEngine;

public class PrefabStatsLoader : MonoBehaviour
{
    public string prefabName; // Set this in the prefab's Inspector
    private PrefabStatsManager statsManager;

    private void Start()
    {
        // Find the PrefabStatsManager in the scene
        statsManager = FindObjectOfType<PrefabStatsManager>();

        if (statsManager != null)
        {
            // Load stats for this prefab
            PrefabStats stats = statsManager.GetPrefabStats(prefabName);

            if (stats != null)
            {
                // Apply the stats to this prefab (assuming the prefab has Health, Attack, Movement components)
                ApplyStats(stats);
            }
            else
            {
                Debug.LogWarning("No stats found for prefab: " + prefabName);
            }
        }
        else
        {
            Debug.LogError("PrefabStatsManager not found in the scene.");
        }
    }

    // Method to apply the stats to the prefab
    private void ApplyStats(PrefabStats stats)
    {
        BaseCharacter character = GetComponent<BaseCharacter>();
        character.SetAttributes(stats);

        Debug.Log($"Stats applied for {prefabName}: Health = {stats.health}, Attack Power = {stats.attackPower}, Speed = {stats.speed}");
    }

    public PrefabStats GetPrefabStat()
    {
        return statsManager.GetPrefabStats(prefabName);
    }
}
