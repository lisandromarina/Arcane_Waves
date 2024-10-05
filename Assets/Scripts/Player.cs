using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : BaseCharacter
{
    private Rigidbody2D rb;
    private Vector2 movementInput;
    private bool isMoving = false;
    private Vector2 lastDirection = Vector2.right; // Keeps track of the last direction the player moved

    [SerializeField] public PlayerAttributes savedStats;
    [SerializeField] private float stopDistanceThreshold = 0.1f;

    [SerializeField] private List<CharacterAnimator> characterAnimators; // List of character types and their animators

    [SerializeField] private GameObject beamSkill;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private Animator animator;

    // Screen boundaries
    private float minYPosition = -402f;
    private float maxYPosition = 150f;
    private float minXPosition = -349;
    private float maxXPosition = 315;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        characterBase = GetComponent<Character_Base>();

        playerInput = GetComponent<PlayerInput>();
        if (playerInput)
        {
            moveAction = playerInput.actions["Move"];
        }

        animator = GetComponent<Animator>();

        LoadAnimator();
    }

    void Update()
    {
        base.Update();

        // Get the movement input from joystick
        movementInput = moveAction.ReadValue<Vector2>();

        Vector3 direction = new Vector3(movementInput.x, movementInput.y, 0).normalized;

        if (movementInput.magnitude > stopDistanceThreshold)
        {
            isMoving = true;
            lastDirection = direction; // Store the last direction the player moved
            characterBase.PlayMoveAnim(direction);
        }
        else
        {
            isMoving = false;
            characterBase.PlayMoveAnim(Vector3.zero);
        }

        if (isMoving && IsAlive)
        {
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }

        // Clamp Y position
        float clampedY = Mathf.Clamp(transform.position.y, minYPosition, maxYPosition);
        // Clamp X position to screen boundaries
        float clampedX = Mathf.Clamp(transform.position.x, minXPosition, maxXPosition);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);

        if (!IsAlive)
        {
            isMoving = false;
            characterBase.PlayMoveAnim(Vector3.zero);
        }
        Debug.Log(lastDirection);
    }

    public List<CharacterAnimator> GetCharacterAnimators()
    {
        return characterAnimators;
    }

    public void Revive()
    {
        health = 100;
        IsAlive = true;
        characterBase.PlayReviveAnim();
    }

    private void LoadAnimator()
    {
        PrefabStatsLoader characterLoader = GetComponent<PrefabStatsLoader>();

        string currentSkin = PrefabStatsManager.Instance.GetSkinSelected(characterLoader.prefabName);

        // Find the corresponding AnimatorController from the list of characterAnimators
        foreach (var characterAnimator in characterAnimators)
        {
            if (characterAnimator.characterType == currentSkin)
            {
                animator.runtimeAnimatorController = characterAnimator.animatorController;
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = characterAnimator.spriteRenderer;
                return;
            }
        }

        // Fallback if no matching AnimatorController is found
        Debug.LogWarning($"No AnimatorController found for skin: {currentSkin}, using default.");
    }

    public Sprite GetSpriteRenderer(string characterType)
    {
        foreach (var characterAnimator in characterAnimators)
        {
            if (characterAnimator.characterType == characterType)
            {
                return characterAnimator.spriteRenderer;
            }
        }

        return null;
    }

    public void castSpell()
    {
        // Play the special skill animation
        characterBase.PlaySpecialSkillAnim("beam");

        Vector3 spawnPosition;
        float beamHalfWidth = 0;
        bool flipBeam = false;

        // Instantiate the beam skill at the player's position
        GameObject beamInstance = Instantiate(beamSkill, transform.position, Quaternion.identity);

        // Get the size of the beam's sprite renderer (if it has one)
        SpriteRenderer beamSpriteRenderer = beamInstance.GetComponent<SpriteRenderer>();
        if (beamSpriteRenderer != null)
        {
            // Get half of the beam's width
            beamHalfWidth = beamSpriteRenderer.bounds.size.x / 2;
        }

        // Check the player's last movement direction
        if (lastDirection.x > 0) // Moving right
        {
            // Set the spawn position 3 units to the right of the player
            spawnPosition = transform.position + new Vector3(1 + beamHalfWidth, -5, 0);
        }
        else if (lastDirection.x < 0) // Moving left
        {
            // Set the spawn position 3 units to the left of the player
            spawnPosition = transform.position + new Vector3(-1 - beamHalfWidth, -5, 0);

            // Flip the beam to face left
            flipBeam = true;
        }
        else
        {
            // If the player isn't moving horizontally, default to right
            spawnPosition = transform.position + new Vector3(3 + beamHalfWidth, 0, 0);
        }

        // Set the beam's position
        beamInstance.transform.position = spawnPosition;

        // Flip the beam horizontally if the player is moving left
        if (flipBeam)
        {
            beamSpriteRenderer.flipX = true;
        }
    }

    [System.Serializable]
    public class CharacterAnimator
    {
        public string characterType; // e.g., "male", "female", "warrior", etc.
        public RuntimeAnimatorController animatorController;
        public Sprite spriteRenderer;
    }
}
