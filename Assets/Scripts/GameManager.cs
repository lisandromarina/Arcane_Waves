using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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

    private Player player;
    private List<Ally> allies = new List<Ally>();

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

        // Assign button listeners
        upgradePlayerButton.onClick.AddListener(UpgradePlayer);
        buyTankButton.onClick.AddListener(BuyTank);
        //buyMageButton.onClick.AddListener(BuyMage);
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
        //buyMageButton.GetComponentInChildren<TextMeshProUGUI>().text = GetMageCost().ToString();
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
        /*float cost = GetPlayerUpgradeCost();
        if (CanAfford(cost))
        {
            DeductMoney(cost);
            Debug.Log("Player upgraded!");

            playerUpgradeLevel++;
            UpdateButtonLabels();

            // Add logic to upgrade player stats or abilities here
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                SoldierAttack playerStats = player.GetComponent<SoldierAttack>();
                if (playerStats != null)
                {
                    int increment = Mathf.RoundToInt(playerStats.GetDamage() * 0.1f);
                    playerStats.SetDamage(playerStats.GetDamage() + increment);
                    Debug.Log("Player's damage upgraded to: " + playerStats.GetDamage());
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
        }*/
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

            GameObject tankPrefab = PrefabManager.Instance.tankPrefab;
            Vector3 spawnPosition = new Vector3(0, 0, 0); // Set the desired position
            Quaternion spawnRotation = Quaternion.identity; // Set the desired rotation
            //Instantiate(tankPrefab, spawnPosition, spawnRotation);

            GameObject tankObject = Instantiate(tankPrefab, spawnPosition, spawnRotation);
            Ally tankHealth = tankObject.GetComponent<Ally>();
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
            }
            else
            {
                Debug.LogWarning("GameOver panel is not assigned in the GameManager.");
            }
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
}
