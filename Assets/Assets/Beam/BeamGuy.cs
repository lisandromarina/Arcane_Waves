using UnityEngine;
using UnityEngine.UI; // Required for UI elements


public class BeamGuy : MonoBehaviour
{
    [SerializeField] private GameObject beamSkill;  // Prefab for the beam
    protected Character_Base characterBase;
    private GameObject beamInstance;

    public Image darkenOverlay;
    // Declare the event for destruction
    public delegate void BeamGuyDestroyedHandler();
    public event BeamGuyDestroyedHandler OnBeamGuyDestroyed;

    public bool isLeftRespawn = true;

    [SerializeField] private RuntimeAnimatorController leftAnimatorController;   // Animator controller for left respawn
    [SerializeField] private RuntimeAnimatorController rightAnimatorController;  // Animator controller for right respawn
    private Animator animator;


    public void Start()
    {
        characterBase = GetComponent<Character_Base>();
        animator = GetComponent<Animator>(); // Get the Animator component
        SetAnimatorController(); // Set the initial animator controller based on position

    }

    public void castSpell()
    {
        float beamHalfWidth = 0;
        Vector3 spawnPosition;

        if (beamSkill != null)
        {
            beamInstance = Instantiate(beamSkill, transform.position, Quaternion.identity);  // Spawn beam at BeamGuy's position

            SpriteRenderer beamSpriteRenderer = beamInstance.GetComponent<SpriteRenderer>();
            if (beamSpriteRenderer != null)
            {
                // Get half of the beam's width
                beamHalfWidth = beamSpriteRenderer.bounds.size.x / 2;
            }

            // Determine spawn position and sprite flipping based on isLeftRespawn
            if (isLeftRespawn)
            {
                spawnPosition = transform.position + new Vector3(beamHalfWidth - 90, -5, 0); // For left respawn
            }
            else
            {
                spawnPosition = transform.position + new Vector3(-beamHalfWidth + 90, -5, 0); // For right respawn
                                                                                              // Flip the beam sprite on the X axis
                if (beamSpriteRenderer != null)
                {
                    beamSpriteRenderer.flipX = true; // Flip the sprite
                }
            }

            beamInstance.transform.position = spawnPosition;

            // Pass this BeamGuy to the Beam script so it can destroy this object later
            Beam beamScript = beamInstance.GetComponent<Beam>();
            if (beamScript != null)
            {
                beamScript.SetBeamGuy(this);
            }
        }
        else
        {
            Debug.LogWarning("BeamSkill prefab not assigned!");
        }
    }


    public void SetLeftRespawn(bool isLeftRespawn)
    {
        this.isLeftRespawn = isLeftRespawn;
        SetAnimatorController(); // Update animator controller when position changes
    }

    private void SetAnimatorController()
    {
        if (animator != null)
        {
            animator.runtimeAnimatorController = isLeftRespawn ? leftAnimatorController : rightAnimatorController;
        }
    }

    // This function will be called by the beam when it's destroyed
    public void DestroyBeamGuy()
    {
        OnBeamGuyDestroyed?.Invoke();
        Destroy(gameObject);  // Destroy the BeamGuy
    }
}
