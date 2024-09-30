using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Config")]
    public GameConfig gameConfig; // Reference to the ScriptableObject

    [Header("Money Settings")]
    public int moneyAmount = 1000;
    public TextMeshProUGUI moneyText;

    [Header("Upgrade Costs")]
    public int playerUpgradeLevel = 0;
    public int tankUpgradeLevel = 0;
    public int mageUpgradeLevel = 0;
    public int warriorUpgradeLevel = 0;

    private float playerBaseCost = 100f;
    private float tankBaseCost = 150f;
    private float mageBaseCost = 200f;
    private float warriorBaseCost = 175f;

    private float playerScalingFactor = 0.2f;
    private float tankScalingFactor = 0.25f;
    private float mageScalingFactor = 0.3f;
    private float warriorScalingFactor = 0.25f;

    [Header("Buttons")]
    public Button upgradePlayerButton;
    public Transform buttonsParent;
    public GameObject buttonPrefab;

    private Dictionary<Button, GameObject> buttonPrefabMap = new Dictionary<Button, GameObject>();


    [Header("Game Over Settings")]
    public GameObject gameOverPanel;
    private bool isGameOver = false;

    private Player player;
    private List<Ally> allies = new List<Ally>();


    private WaveManager waveManager;

    [Header("Persistent objects")]
    public SaveManager saveManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateMoneyUI();

        waveManager = GetComponent<WaveManager>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        if (player != null)
        {
            player.onDeath += CheckGameOver;
        }

        InstantiateButtonGrid();
        UpdateButtonLabels();
    }

    void UpdateMoneyUI()
    {
        moneyText.text = $"{moneyAmount}";
    }

    private void InstantiateButtonGrid()
    {
        // Clear existing buttons if necessary
        foreach (Transform child in buttonsParent)
        {
            Destroy(child.gameObject);
        }

        List<GameObject> alliesList = PrefabManager.Instance.GetAllies();

        for (int i = 0; i < alliesList.Count; i++)
        {
            // Check if the prefab's tag matches the current filter
            if (alliesList[i].tag != "Ally" && alliesList[i].tag != "Player")
            {
                continue; // Skip prefabs that don't match the filter
            }

            // Instantiate the button
            GameObject newButton = Instantiate(buttonPrefab, buttonsParent);
            Button button = newButton.GetComponent<Button>();

            // Store the button and corresponding prefab in the dictionary
            buttonPrefabMap[button] = alliesList[i];

            // Find the child by name and get the Image component
            Transform childTransform = newButton.transform.Find("upgradePlayerButton");
            if (childTransform != null)
            {
                Image buttonImage = childTransform.GetComponent<Image>();
                if (buttonImage != null)
                {
                    // Access the SpriteRenderer component from the prefab
                    SpriteRenderer prefabSpriteRenderer = alliesList[i].GetComponent<SpriteRenderer>();
                    if (prefabSpriteRenderer != null)
                    {
                        buttonImage.sprite = prefabSpriteRenderer.sprite;
                    }
                }
            }

            string prefabName = alliesList[i].name;
            if (prefabName.Contains("Player"))
            {
                button.onClick.AddListener(() => UpgradePlayer());
            }
            else if (prefabName.Contains("Tank"))
            {
                button.onClick.AddListener(() => BuyUnit(PrefabManager.Instance.tankPrefab, ref tankUpgradeLevel, tankBaseCost, tankScalingFactor));
            }
            else if (prefabName.Contains("Space"))
            {
                button.onClick.AddListener(() => BuyUnit(PrefabManager.Instance.spaceCadet, ref mageUpgradeLevel, mageBaseCost, mageScalingFactor));
            }
            // You can add more unit types here if needed
        }
    }

    void UpdateButtonLabels()
    {
        foreach (var entry in buttonPrefabMap)
        {
            Button button = entry.Key;
            GameObject prefab = entry.Value;

            // Find the child TextMeshProUGUI in the button
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
            {
                string prefabName = prefab.name;

                if (prefabName.Contains("Player"))
                {
                    buttonText.text = GetPlayerUpgradeCost().ToString();
                }
                else if (prefabName.Contains("Tank"))
                {
                    buttonText.text = GetTankCost().ToString();
                }
                else if (prefabName.Contains("Space"))
                {
                    buttonText.text = GetMageCost().ToString();
                }
                // You can add more cases if needed
            }
        }
    }

    bool CanAfford(float cost)
    {
        return moneyAmount >= cost;
    }

    void DeductMoney(float cost)
    {
        moneyAmount -= (int)cost;
        UpdateMoneyUI();
    }

    float GetPlayerUpgradeCost()
    {
        return playerBaseCost * Mathf.Pow(1 + playerScalingFactor, playerUpgradeLevel);
    }

    float GetTankCost()
    {
        return tankBaseCost * Mathf.Pow(1 + tankScalingFactor, tankUpgradeLevel);
    }

    float GetMageCost()
    {
        return mageBaseCost * Mathf.Pow(1 + mageScalingFactor, mageUpgradeLevel);
    }

    float GetWarriorCost()
    {
        return warriorBaseCost * Mathf.Pow(1 + warriorScalingFactor, warriorUpgradeLevel);
    }

    void UpgradePlayer()
    {
        float cost = GetPlayerUpgradeCost();
        if (CanAfford(cost))
        {
            DeductMoney(cost);
            Debug.Log("Player upgraded!");

            playerUpgradeLevel++;
            UpdateButtonLabels();

            // Add logic to upgrade player stats or abilities here
            GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Player player = playerGameObject.GetComponent<Player>();
                if (player != null)
                {
                    player.UpgradeDamage();
                }
                else
                {
                    Debug.LogWarning("PlayerStats component not found on the Player object.");
                }
            }
            else
            {
                Debug.LogWarning("Player object not found.");
            }
        }
        else
        {
            Debug.Log("Not enough money to upgrade player.");
        }
    }

    void BuyUnit(GameObject unitPrefab, ref int upgradeLevel, float baseCost, float scalingFactor)
    {
        float cost = baseCost * Mathf.Pow(1 + scalingFactor, upgradeLevel);

        if (CanAfford(cost))
        {
            DeductMoney(cost);
            upgradeLevel++;
            UpdateButtonLabels();

            GameObject unitObject = PortalManager.CreatePortal(new Vector3(0, 0, 0), unitPrefab);
            Ally unitAlly = unitObject != null ? unitObject.GetComponent<Ally>() : null;

            if (unitAlly != null)
            {
                RegisterAlly(unitAlly);
            }
            else
            {
                Debug.LogWarning("Unit does not have an Ally component.");
            }
        }
        else
        {
            Debug.Log("Not enough money to buy unit.");
        }
    }

    void BuyTank()
    {
        float cost = GetTankCost();
        if (CanAfford(cost))
        {
            DeductMoney(cost);
            Debug.Log("Tank purchased!");

            tankUpgradeLevel++;
            UpdateButtonLabels();


            GameObject tankObject = PortalManager.CreatePortal(new Vector3(0, 0, 0), PrefabManager.Instance.tankPrefab);

            Ally tankHealth = tankObject != null ? tankObject.GetComponent<Ally>() : null;
            if (tankHealth != null)
            {
                RegisterAlly(tankHealth);
            }
           
        }
        else
        {
            Debug.Log("Not enough money to buy a tank.");
        }
    }

    void BuyMage()
    {
        float cost = GetMageCost();
        if (CanAfford(cost))
        {
            DeductMoney(cost);
            Debug.Log("Mage purchased!");

            mageUpgradeLevel++;
            UpdateButtonLabels();

            GameObject tankObject = PortalManager.CreatePortal(new Vector3(0, 0, 0), PrefabManager.Instance.spaceCadet);

            Ally tankHealth = tankObject != null ? tankObject.GetComponent<Ally>() : null;
            if (tankHealth != null)
            {
                RegisterAlly(tankHealth);
            }

            // Add logic to spawn a mage or upgrade its abilities here
        }
        else
        {
            Debug.Log("Not enough money to buy a mage.");
        }
    }

    void BuyWarrior()
    {
        float cost = GetWarriorCost();
        if (CanAfford(cost))
        {
            DeductMoney(cost);
            Debug.Log("Warrior purchased!");

            warriorUpgradeLevel++;
            UpdateButtonLabels();

            // Add logic to spawn a warrior or upgrade its abilities here
        }
        else
        {
            Debug.Log("Not enough money to buy a warrior.");
        }
    }

    public void AddMoney(int amount)
    {
        moneyAmount += amount;
        Debug.Log("Money added: " + amount + ". Total: " + moneyAmount);
        UpdateMoneyUI();
    }

    void CheckGameOver()
    {
        if (!player.IsAlive && allies.Count == 0)
        {
            Debug.Log("Game Over! All allies and player are dead.");
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                isGameOver = true;
                SaveGameData(); // Save data when the game is over
            }
            else
            {
                Debug.LogWarning("GameOver panel is not assigned in the GameManager.");
            }
        }
    }

    public void SaveGameData()
    {
        if (gameConfig != null)
        {
            if (isGameOver)
            {
                gameConfig.lastGameWave = WaveManager.Instance.GetCurrentWave() - 1;
            }
            else
            {
                gameConfig.lastGameWave = WaveManager.Instance.GetCurrentWave();
            }
            // Update the gameConfig with the current game state
           
            gameConfig.hasToMove = true;
            saveManager.SaveGameConfigToJson();
            Debug.Log($"Game data saved. LAST GAME Wave: {gameConfig.lastGameWave}");
        }
        else
        {
            Debug.LogWarning("GameConfig is not assigned in GameManager.");
        }
    }

    public void RegisterAlly(Ally ally)
    {
        allies.Add(ally);
        ally.onDeath += () => OnAllyDeath(ally);
    }

    void OnAllyDeath(Ally ally)
    {
        Debug.Log("allie died");
        allies.Remove(ally);
        CheckGameOver();
    }

    public void OnMainMenuClick()
    {
        Loader.Load(Loader.Scene.MainMenu);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Logic to destroy the GameManager instance if needed
        if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }
}
