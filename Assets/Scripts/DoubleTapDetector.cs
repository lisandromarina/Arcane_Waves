using UnityEngine;

public class DoubleTapDetector : MonoBehaviour
{
    public float doubleTapTime = 0.3f; // Time interval for double tap
    private float lastTapTime = 0f;     // Time of the last tap
    private bool isCastingSpell = false; // Flag to indicate if the spell is currently being cast
    private bool respawnOnLeft = true;   // Flag to toggle between left and right respawn

    [SerializeField] private RectTransform barPanelRectTransform; // Reference to the Bar panel RectTransform
    private Canvas canvas;

    private Player player; // Reference to the Player script

    private void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas not found. Ensure DoubleTapDetector is a child of a Canvas.");
        }

        // Find the player in the scene
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
        }
    }

    void Update()
    {
        // Check if there is at least one touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Detect the end of the touch
            if (touch.phase == TouchPhase.Ended && !IsTouchOverPanel(touch.position, barPanelRectTransform))
            {
                // Check if the spell is already being cast
                if (isCastingSpell)
                {
                    return; // Ignore the double tap if casting is in progress
                }

                // Check the time since the last tap
                if (Time.time - lastTapTime < doubleTapTime)
                {
                    // Double tap detected
                    OnDoubleTap();
                }

                // Update the last tap time
                lastTapTime = Time.time;
            }
        }
    }

    private void OnDoubleTap()
    {
        // Check if the player has enough mana before casting
        if (player != null && player.HasEnoughMana())
        {
            // Start casting the spell
            isCastingSpell = true;
            onSkillClick();

            // Deduct mana cost
            player.UseMana();
        }
        else
        {
            Debug.Log("Not enough mana to cast the special skill.");
        }
    }

    public void onSkillClick()
    {
        Vector3 playerPosition = GameObject.FindWithTag("Player").transform.position;

        // Determine respawn position based on the current side
        Camera mainCamera = Camera.main;
        Vector3 respawnPosition = Vector3.zero;
        if (mainCamera != null)
        {
            Vector3 edgeOfCamera;
            if (respawnOnLeft)
            {
                // Get the left edge of the camera in world space
                edgeOfCamera = mainCamera.ViewportToWorldPoint(new Vector3(0, 0.5f, mainCamera.nearClipPlane));
                // Position BeamGuy at the same Y as the player but on the left edge of the camera's X
                respawnPosition = new Vector3(edgeOfCamera.x + 15, playerPosition.y, 0); // Maintain the player's Y position
            }
            else
            {
                // Get the right edge of the camera in world space
                edgeOfCamera = mainCamera.ViewportToWorldPoint(new Vector3(1, 0.5f, mainCamera.nearClipPlane));
                // Position BeamGuy at the same Y as the player but on the right edge of the camera's X
                respawnPosition = new Vector3(edgeOfCamera.x - 15, playerPosition.y, 0); // Maintain the player's Y position
            }
        }

        // Create the BeamGuy
        GameObject beamGuy = Instantiate(GameAssets.i.BeamGuy, respawnPosition, Quaternion.identity);
        GameObject soundBeam = SoundManager.PlaySound(SoundManager.Sound.BeamAttack, false, 0.40f);
        SoundManager.FadeOutSound(soundBeam, 2.1f);
        // Flip the BeamGuy based on the respawn side
        Vector3 beamGuyScale = beamGuy.transform.localScale;
        beamGuyScale.x = respawnOnLeft ? beamGuyScale.x * 1 : beamGuyScale.x * -1; // Flip if respawning on the right
        beamGuy.transform.localScale = beamGuyScale;

        // Subscribe to the OnBeamGuyDestroyed event
        BeamGuy beamGuyScript = beamGuy.GetComponent<BeamGuy>();
        if (beamGuyScript != null)
        {
            beamGuyScript.OnBeamGuyDestroyed += HandleBeamGuyDestroyed;
            beamGuyScript.SetLeftRespawn(respawnOnLeft);
        }

        // Toggle the respawn side for the next double tap
        respawnOnLeft = !respawnOnLeft;
    }

    private void HandleBeamGuyDestroyed()
    {
        // Reset the casting state when the BeamGuy is destroyed
        isCastingSpell = false;
    }

    private bool IsTouchOverPanel(Vector2 touchPosition, RectTransform panelRectTransform)
    {
        if (panelRectTransform == null) return false;

        // Convert screen point to local point in panel space
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panelRectTransform,
            touchPosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint
        );

        // Check if the localPoint is within the panel's bounds
        return panelRectTransform.rect.Contains(localPoint);
    }
}
