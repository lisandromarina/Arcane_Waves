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

    [SerializeField] private GameObject[] playerPrefabs; // Reference for Play button\
    [SerializeField] private GameObject buttonPrefab; // Reference to your button prefab
    public Transform buttonParent;  // Parent object to hold the buttons (Canvas or Panel)

    [SerializeField] private Button upgradeCharacterButton; // Reference for upgrade character button

    private Dictionary<Button, GameObject> buttonPrefabMap = new Dictionary<Button, GameObject>();


    private GameObject selectedPrefab;

    private bool isShowingAdventure = true;
    private Vector3 adventureCameraPosition;
    // Start is called before the first frame update
    void Awake()
    {

        playButton.onClick.AddListener(onPlayClick);
        
        upgradeCharacterButton.onClick.AddListener(onUpgradeCharacterClick);


        adventureCameraPosition = Camera.main.transform.position;

        InstantiateButtonGrid();

        moneyText.text = GameManagerMainMenu.Instance.GetAmountOfMoney().ToString();
    }


    // Update is called once per frame
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
            selectedPrefab = GameManagerMainMenu.Instance.RespawnPrefab(associatedPrefab, new Vector2(-23, 122));
            selectedPrefab.transform.localScale = new Vector3(150, 150, 0);
            BaseCharacter character = selectedPrefab.GetComponent<BaseCharacter>();
            character.SetAttackRange(100);

            cardsPanel.gameObject.SetActive(false);
            upgradeDetailsPanel.gameObject.SetActive(true);
        }
    }

    public void onBackClick()
    {
        Destroy(selectedPrefab);
        selectedPrefab = null;
        upgradeDetailsPanel.gameObject.SetActive(false);
        cardsPanel.gameObject.SetActive(true);
        //prefab.transform.localScale = new Vector3();
    }

    private void InstantiateButtonGrid()
    {
        for (int i = 0; i < playerPrefabs.Length; i++)
        {
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
                    // Access the SpriteRenderer component from the prefab
                    SpriteRenderer prefabSpriteRenderer = playerPrefabs[i].GetComponent<SpriteRenderer>();

                    if (prefabSpriteRenderer != null)
                    {
                        buttonImage.sprite = prefabSpriteRenderer.sprite;
                    }
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
        Debug.Log("Ea");
    }
}