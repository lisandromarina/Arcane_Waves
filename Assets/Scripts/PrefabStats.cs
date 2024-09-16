[System.Serializable]
public class PrefabStats
{
    public string prefabName;
    public int health;
    public int attackPower;
    public int attackRange;
    public float speed;

    public int levelUpgrade;
    public int baseCost;            // Base cost for the first upgrade
    public float costMultiplier;    // Multiplier that increases cost with each upgrade level

    public bool isSelected;
    // Add any other stats as needed
}