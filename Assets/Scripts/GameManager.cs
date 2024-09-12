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
    public Button buyTankButton;
    public Button buyMageButton;
    public Button buyWarriorButton;

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
        UpdateButtonLabels();

        waveManager = GetComponent<WaveManager>();

        // Assign button listeners
        upgradePlayerButton.onClick.AddListener(UpgradePlayer);
        buyTankButton.onClick.AddListener(BuyTank);
        buyMageButton.onClick.AddListener(BuyMage);
        //buyWarriorButton.onClick.AddListener(BuyWarrior);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        if (player != null)
        {
            player.onDeath += CheckGameOver;
        }
    }

    void UpdateMoneyUI()
    {
        moneyText.text = $"{moneyAmount}";
    }

    void UpdateButtonLabels()
    {
        // Update the button labels with the current prices
        upgradePlayerButton.GetComponentInChildren<TextMeshProUGUI>().text = GetPlayerUpgradeCost().ToString();
        buyTankButton.GetComponentInChildren<TextMeshProUGUI>().text = GetTankCost().ToString();
        buyMageButton.GetComponentInChildren<TextMeshProUGUI>().text = GetMageCost().ToString();
        //buyWarriorButton.GetComponentInChildren<TextMeshProUGUI>().text = GetWarriorCost().ToString();
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
