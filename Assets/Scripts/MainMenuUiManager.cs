using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUiManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    [SerializeField] private Transform AdventurePanel; // Reference for Grid component
    [SerializeField] private Transform TeamPanel; // Reference for Grid component
    [SerializeField] private Button playButton; // Reference for Play button

    [SerializeField] private Transform upgradeDetailsPanel; // Reference for upgradeDetailsPanel
    [SerializeField] private Transform cardsPanel; // Reference for Grid component

    private string currentFilterTag = "Player"; // Can be set to "Player" or "Hero"
    [SerializeField] private Button heroFilterButton; // Reference for Play button
    [SerializeField] private Button allyFilterButton; // Reference for Play button

    [SerializeField] private GameObject[] playerPrefabs; // Reference for Play button\
    [SerializeField] private GameObject buttonPrefab; // Reference to your button prefab
    public Transform buttonParent;  // Parent object to hold the buttons (Canvas or Panel)

    [SerializeField] private Button upgradeCharacterButton; // Reference for upgrade character button

    [SerializeField] private Button changeSkinButton; // Reference for upgrade character button

    private Dictionary<Button, GameObject> buttonPrefabMap = new Dictionary<Button, GameObject>();


    private GameObject selectedPrefab;
    private Button buttonSelected;

    private bool isShowingAdventure = true;
    private Vector3 adventureCameraPosition;
    // Start is called before the first frame update
    void Awake()
    {

        playButton.onClick.AddListener(onPlayClick);
        
        upgradeCharacterButton.onClick.AddListener(onUpgradeCharacterClick);


        adventureCameraPosition = Camera.main.transform.position;

        heroFilterButton.onClick.AddListener(() => SetFilter("Player"));
        allyFilterButton.onClick.AddListener(() => SetFilter("Ally"));

        moneyText.text = GameManagerMainMenu.Instance.GetAmountOfMoney().ToString();
    }


    void Update()
    {
        if (isShowingAdventure)
        {
            adventureCameraPosition = Camera.main.transform.position;
        }

        //JUST TO MAKE SURE THAT THE PLAYER/ALLY DOES NOT MOVE FROM THAT POSITION
        if (selectedPrefab != null)
        {
            selectedPrefab.transform.position = new Vector3(-23, 122, 0);

            TextMeshProUGUI[] texts = upgradeCharacterButton.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI text in texts)
            {
                if (text.name == "Cost") // Match the text name
                {
                    PrefabStatsLoader characterLoader = selectedPrefab.GetComponent<PrefabStatsLoader>();
                    PrefabStatsManager statsManager = FindObjectOfType<PrefabStatsManager>();
                    PrefabStats stats = statsManager.GetPrefabStats(characterLoader.prefabName);
                    Debug.Log(statsManager.CalculateUpgradeCost(stats));
                    text.text = statsManager.CalculateUpgradeCost(stats).ToString();
                    break;
                }
            }
        }

    }

    public void UpdateUI()
    {
        moneyText.text = GameManagerMainMenu.Instance.GetAmountOfMoney().ToString();
    }

    public void onPlayClick()
    {
        Loader.Load(Loader.Scene.SampleScene);
    }

    public void onTeamClick()
    {
        InstantiateButtonGrid();
        playButton.gameObject.SetActive(false);
        AdventurePanel.gameObject.SetActive(false);
        isShowingAdventure = false;

        TeamPanel.gameObject.SetActive(true);
        changeCameraSetting(new Vector3(0,0,-10), false);
    }

    public void onAdventureClick()
    {
        playButton.gameObject.SetActive(true);
        AdventurePanel.gameObject.SetActive(true);
        isShowingAdventure = true;

        TeamPanel.gameObject.SetActive(false);
        changeCameraSetting(adventureCameraPosition, true);

        if(selectedPrefab != null)
        {
            Destroy(selectedPrefab);

            upgradeDetailsPanel.gameObject.SetActive(false);
            cardsPanel.gameObject.SetActive(true);
        }

        //destroy possible popup when changing the scene
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("DamagePopup"))
            {
                Destroy(obj);
            }
        }
    }

    public void SetFilter(string filterTag)
    {
        currentFilterTag = filterTag;
        InstantiateButtonGrid(); // Re-instantiate buttons based on the new filter
    }

    private void changeCameraSetting(Vector3 position, bool isOffsetEnabled)
    {
        GameObject cameraObject = Camera.main.gameObject;

        // Get the CameraFollow component
        CameraFollow cameraFollow = cameraObject.GetComponent<CameraFollow>();

        // Check if the component was found
        if (cameraFollow != null )
        {
            cameraFollow.enabled = isOffsetEnabled;
            Camera.main.transform.position = position;
        }
        else
        {
            Debug.Log("CameraFollow script not found on the camera GameObject.");
        }
    }

    public void onUpgradeCardClick(Button clickedButton)
    {
        if (buttonPrefabMap.TryGetValue(clickedButton, out GameObject associatedPrefab))
        {
            buttonSelected = clickedButton;
            selectedPrefab = GameManagerMainMenu.Instance.RespawnPrefab(associatedPrefab, new Vector2(-23, 122));
            ManaPlayer manaPlayer = selectedPrefab.GetComponent<ManaPlayer>();
            if(manaPlayer != null)
            {
                manaPlayer.SetIsVisible(false);
            }
            selectedPrefab.transform.localScale = new Vector3(150, 150, 0);
            BaseCharacter character = selectedPrefab.GetComponent<BaseCharacter>();
            character.SetAttackRange(100);

            cardsPanel.gameObject.SetActive(false);
            upgradeDetailsPanel.gameObject.SetActive(true);


            //VERIFY IF THE PREFABS HAS SOME SKIN
            PrefabStatsLoader characterLoader = selectedPrefab.GetComponent<PrefabStatsLoader>();
            string currentSkin = PrefabStatsManager.Instance.GetSkinSelected(characterLoader.prefabName);
            string[] skins = PrefabStatsManager.Instance.GetListOfSkins(characterLoader.prefabName);

            if(skins.Length <= 0)
            {
                changeSkinButton.gameObject.SetActive(false);
            }
            else
            {
                changeSkinButton.gameObject.SetActive(true);
            }
        }
    }

    public void onBackClick()
    {
        Destroy(selectedPrefab);
        selectedPrefab = null;
        buttonSelected = null;
        upgradeDetailsPanel.gameObject.SetActive(false);
        cardsPanel.gameObject.SetActive(true);
    }

    private void InstantiateButtonGrid()
    {
        foreach (Transform child in buttonParent)
        {
            // Clear existing buttons (if necessary)
            Destroy(child.gameObject);
        }

        for (int i = 0; i < playerPrefabs.Length; i++)
        {
            // Check if the prefab's tag matches the current filter
            if (playerPrefabs[i].tag != currentFilterTag)
            {
                continue; // Skip prefabs that don't match the filter
            }

            // Instantiate the button
            GameObject newButton = Instantiate(buttonPrefab, buttonParent);
            Button button = newButton.GetComponent<Button>();

            // Store prefab in dictionary with button as key
            buttonPrefabMap[button] = playerPrefabs[i];

            // Find the child by name and get the Image component
            Transform childTransform = newButton.transform.Find("ChildImageName"); // Replace "ChildImageName" with the actual name of your child object
            if (childTransform != null)
            {
                Image buttonImage = childTransform.GetComponent<Image>();
                if (buttonImage != null)
                {

                    PrefabStatsLoader characterLoader = playerPrefabs[i].GetComponent<PrefabStatsLoader>();

                    Debug.Log("characterLoader.prefabName " + (characterLoader.prefabName));
                    string[] skins = FindFirstObjectByType<PrefabStatsManager>().GetListOfSkins(characterLoader.prefabName);


                    SpriteRenderer prefabSpriteRenderer = null;
                    if (skins.Length > 0 && playerPrefabs[i].tag == "Player")//FOR THE MOMENT THE ONLY ONE WITH SKIN IS THE PLAYER!
                    {
                        Player player = playerPrefabs[i].GetComponent<Player>();

                        string skinSelected = PrefabStatsManager.Instance.GetSkinSelected(characterLoader.prefabName);

                        buttonImage.sprite = player.GetSpriteRenderer(skinSelected);
                    }
                    else
                    {
                        prefabSpriteRenderer = playerPrefabs[i].GetComponent<SpriteRenderer>();

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

            // Add onClick listener
            button.onClick.AddListener(() => onUpgradeCardClick(button));
        }
    }

    private void onUpgradeCharacterClick()
    {
        PrefabStatsLoader characterLoader = selectedPrefab.GetComponent<PrefabStatsLoader>();

        PrefabStatsManager.Instance.TryUpgradePrefab(characterLoader.prefabName);
        UpdateUI();
    }

    public void onSkinChange()
    {
        PrefabStatsLoader characterLoader = selectedPrefab.GetComponent<PrefabStatsLoader>();
        // Get current skin selected
        string currentSkin = PrefabStatsManager.Instance.GetSkinSelected(characterLoader.prefabName);

        // Get the list of skins
        string[] skins = PrefabStatsManager.Instance.GetListOfSkins(characterLoader.prefabName);

        // Find the index of the current skin
        int currentIndex = Array.IndexOf(skins, currentSkin);

        // Calculate the index for the next skin (looping back to 0 if we are at the end of the list)
        int nextIndex = (currentIndex + 1) % skins.Length;

        // Set the new skin
        PrefabStatsManager.Instance.SetSkinSelected(characterLoader.prefabName, skins[nextIndex]);

        // Debug log to show the new skin selected
        Debug.Log("New skin selected: " + skins[nextIndex]);
        
        Destroy(selectedPrefab);
        selectedPrefab = null;
        
        if (buttonPrefabMap.TryGetValue(buttonSelected, out GameObject associatedPrefab))
        {
            selectedPrefab = GameManagerMainMenu.Instance.RespawnPrefab(associatedPrefab, new Vector2(-23, 122));
            selectedPrefab.transform.localScale = new Vector3(150, 150, 0);
            BaseCharacter character = selectedPrefab.GetComponent<BaseCharacter>();
            character.SetAttackRange(100);

            ManaPlayer manaPlayer = selectedPrefab.GetComponent<ManaPlayer>();
            if (manaPlayer != null)
            {
                manaPlayer.SetIsVisible(false);
            }

            cardsPanel.gameObject.SetActive(false);
            upgradeDetailsPanel.gameObject.SetActive(true);

            InstantiateButtonGrid();
        }

        if(selectedPrefab.tag == "Player")
        {
            Transform player = FindPlayerInAdventurePanel(AdventurePanel);

            if (player != null)
            {
                Debug.Log("Player found");
                player.GetComponent<PlayerMainMenu>().LoadAnimator();
                // Player found, do something
            }
            else
            {
                Debug.LogWarning("Player not found in AdventurePanel!");
            }
        }

    }

    private Transform FindPlayerInAdventurePanel(Transform parent)
    {
        // Loop through each child of the parent
        foreach (Transform child in parent)
        {
            // Check if the child has the tag "Player"
            if (child.CompareTag("Player"))
            {
                Debug.Log("Player found: " + child.name);
                return child;  // Return the player if found
            }

            // Recursive search in case the child has more children
            Transform found = FindPlayerInAdventurePanel(child);
            if (found != null)
            {
                return found;  // Return the found player
            }
        }

        return null;  // Return null if no player is found
    }
}