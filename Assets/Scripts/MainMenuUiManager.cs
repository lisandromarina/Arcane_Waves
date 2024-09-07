using UnityEngine;
using UnityEngine.UI;

public class MainMenuUiManager : MonoBehaviour
{
    [SerializeField] private Transform AdventurePanel; // Reference for Grid component
    [SerializeField] private Transform TeamPanel; // Reference for Grid component
    [SerializeField] private Button playButton; // Reference for Play button

    [SerializeField] private Transform upgradeDetailsPanel; // Reference for upgradeDetailsPanel
    [SerializeField] private Transform cardsPanel; // Reference for Grid component

    [SerializeField] private GameObject playerPrefab; // Reference for Play button\
    [SerializeField] private GameObject allyPrefab; // Reference for Play button

    private GameObject selectedPrefab;

    private bool isShowingAdventure = true;
    private Vector3 adventureCameraPosition;
    // Start is called before the first frame update
    void Awake()
    {

        playButton.onClick.AddListener(onPlayClick);


        adventureCameraPosition = Camera.main.transform.position;

    }


    // Update is called once per frame
    void Update()
    {
        if (isShowingAdventure)
        {
            adventureCameraPosition = Camera.main.transform.position;
        }

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

    public void onUpgradeCardClick()
    {
        selectedPrefab = GameManagerMainMenu.Instance.RespawnPrefab(playerPrefab, new Vector2(-23, 122));

        cardsPanel.gameObject.SetActive(false);
        upgradeDetailsPanel.gameObject.SetActive(true);
        //prefab.transform.localScale = new Vector3();
    }

    public void onBackClick()
    {
        Destroy(selectedPrefab);

        upgradeDetailsPanel.gameObject.SetActive(false);
        cardsPanel.gameObject.SetActive(true);
        //prefab.transform.localScale = new Vector3();
    }
}