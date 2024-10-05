using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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

    [Header("Movement Joystick")]
    public InputActionAsset playerController;

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
        //SoundManager.PlaySound(SoundManager.Sound.BattleMusic, true, 0.16f);

        UpdateMoneyUI();

        PrefabStatsManager.Instance.LoadStatsFromJson();

        Vector3 spawnPosition = new Vector3(-1.4f, 6.1f, 0f);
        GameObject playerGO = Instantiate(PrefabManager.Instance.playerPrefab, spawnPosition, Quaternion.identity);
        player = playerGO.GetComponent<Player>();
        PlayerInput playerInput = playerGO.AddComponent<PlayerInput>();

        // Assign the InputActionAsset and set the default action map
        playerInput.actions = playerController;  // Assign your InputActionAsset here
        playerInput.defaultActionMap = "Player";
        JoystickController joystickController = GetComponent<JoystickController>();
        joystickController.enabled = true;

        playerInput.enabled = false;  // Temporarily disable to make sure the InputActionAsset is applied first
        playerInput.enabled = true;

        if (player != null)
        {
            player.onDeath += CheckGameOver;
        }


        waveManager = GetComponent<WaveManager>();
        waveManager.enabled = true;

        InstantiateButtonGrid();
        UpdateButtonLabels();

        GameObject mainCamera = GameObject.FindWithTag("MainCamera");

        // Check if the main camera exists
        if (mainCamera != null)
        {
            // Get the GameCamera component attached to the main camera
            GameCamera gameCamera = mainCamera.GetComponent<GameCamera>();

            // Enable the GameCamera component
            if (gameCamera != null)
            {
                gameCamera.enabled = true;
                Debug.Log("GameCamera component enabled.");
            }
        }
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

                    PrefabStatsLoader characterLoader = alliesList[i].GetComponent<PrefabStatsLoader>();

                    string[] skins = FindFirstObjectByType<PrefabStatsManager>().GetListOfSkins(characterLoader.prefabName);


                    SpriteRenderer prefabSpriteRenderer = null;
                    if (skins.Length > 0 && alliesList[i].tag == "Player")//FOR THE MOMENT THE ONLY ONE WITH SKIN IS THE PLAYER!
                    {
                        Player player = alliesList[i].GetComponent<Player>();

                        string skinSelected = PrefabStatsManager.Instance.GetSkinSelected(characterLoader.prefabName);

                        buttonImage.sprite = player.GetSpriteRenderer(skinSelected);
                    }
                    else
                    {
                        prefabSpriteRenderer = alliesList[i].GetComponent<SpriteRenderer>();

                        if (prefabSpriteRenderer != null)
                        {
                            buttonImage.sprite = prefabSpriteRenderer.sprite;
                        }
                    }

                    // Access the SpriteRenderer component from the prefab

                }
            }
            else
            {
                Debug.Log("childTransform not found");
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

            GameObject playerGO = GameObject.FindWithTag("Player");

            Vector3 portalPosition = new Vector3(0,0,0);
            if (playerGO != null)
            {
                // Get player's position
                Vector3 playerPosition = playerGO.transform.position;

                // Define the offset where the portal will appear (e.g., 2 units to the right)
                Vector3 portalOffset = new Vector3(2, 0, 0); // You can modify this offset as needed

                // Calculate the portal's position relative to the player
                portalPosition = playerPosition + portalOffset;

                // Create the portal at the calculated position
            }
            
            GameObject unitObject = PortalManager.CreatePortal(portalPosition, unitPrefab);

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
            Debug.LogWarning("Player not found!");
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

    public void onSkillClick()
    {

        Vector3 playerPosition = GameObject.FindWithTag("Player").transform.position;

        // Move the BeamGuy to the left edge of the camera
        Camera mainCamera = Camera.main;
        Vector3 respawnPosition = Vector3.zero;
        if (mainCamera != null)
        {
            // Get the left edge of the camera in world space, keeping the Y position from the player
            Vector3 leftEdgeOfCamera = mainCamera.ViewportToWorldPoint(new Vector3(0, 0.5f, mainCamera.nearClipPlane));
            // Position BeamGuy at the same Y as the player but on the left edge of the camera's X
            respawnPosition = new Vector3(leftEdgeOfCamera.x + 15, playerPosition.y, 0); // Maintain the player's Y position
        }


        GameObject beamGuy = Instantiate(GameAssets.i.BeamGuy, respawnPosition, Quaternion.identity);
        /*beamGuy.GetComponent<BeamGuy>().castSpell();*/
    }
}
