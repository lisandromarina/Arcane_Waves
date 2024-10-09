using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;
using static Player;

public class PlayerMainMenu : MonoBehaviour
{
    public List<Vector3> waypoints;

    public float speed = 5f;      // Speed of the player movement
    private int currentWaypointIndex = 0;
    private bool isAttacking = false;  // State to check if the player is attacking
    private bool isMoving = false;      // State to check if the player is moving

    private Character_Base characterBase;   // Reference to the animation handler
    private Animator animator;
    public GameConfig gameConfig;       // Reference to the GameConfig ScriptableObject

    private Action stopMovementAction;

    private List<CharacterAnimator> characterAnimators; // List of character types and their animators
    void Awake()
    {
        waypoints = new List<Vector3>
        {
            new Vector3(-2f, -130.6f, 0f),
            new Vector3(-2f, 87f, 0f),
            new Vector3(-2f, 295f, 0f),
            new Vector3(-2f, 480f, 0f)
        };
    }

    void Start()
    {
        Debug.Log("GetComponent<ManaPlayer>() == null " + (GetComponent<ManaPlayer>() == null));
        GetComponent<ManaPlayer>().SetIsVisible(false);
        characterBase = GetComponent<Character_Base>();
        animator = GetComponent<Animator>();
        characterBase.PlayMoveAnim(Vector3.zero); // Start default movement animation
        characterAnimators = GetComponent<Player>().GetCharacterAnimators();
        LoadAnimator();
        LoadPlayerState();
    }

    void Update()
    {
        if (!isAttacking && isMoving) // Only move if not attacking and movement is allowed
        {
            MoveToWaypoint();
        }
    }

    public void StartMovement(int maxWave, Action onStopMovement)
    {
        Debug.Log("maxWave " + maxWave);
        currentWaypointIndex = CalculateWaypointIndex(maxWave);
        Debug.Log("currentWaypointIndex:" + currentWaypointIndex);
        if (currentWaypointIndex > 0)
        {
            Debug.Log("Move");
            isMoving = true;

            stopMovementAction = onStopMovement;
        }
        else
        {
            onStopMovement.Invoke();
        }
    }

    private void MoveToWaypoint()
    {
        if (currentWaypointIndex >= waypoints.Count || currentWaypointIndex < 0) return; // No valid waypoint to move to

        Vector3 targetWaypoint = waypoints[currentWaypointIndex];
        Debug.Log(currentWaypointIndex);
        Debug.Log(waypoints[3]);
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);

        // Play the move animation in the direction of the next waypoint
        characterBase.PlayMoveAnim((targetWaypoint - transform.position).normalized);

        // Check if the player has reached the waypoint
        if (Vector3.Distance(transform.position, targetWaypoint) < 0.1f)
        {
            StopMovement();

        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !isAttacking)
        {
            isAttacking = true; // Set attacking state to true
            characterBase.PlayMoveAnim(Vector3.zero);
            characterBase.PlayAttackAnim(isAttacking); // Play the attack animation

            // "Kill" the enemy and wait for the death animation to complete
            other.GetComponent<EnemyMainMenu>().Kill(() =>
            {
                ResumeMovementAfterAttack();
            });
        }
    }

    private void ResumeMovementAfterAttack()
    {
        isAttacking = false; // Reset attacking state
        characterBase.PlayAttackAnim(false); // Reset attack animation
    }

    private int CalculateWaypointIndex(int wave)
    {
        // Calculate the closest waypoint index that is less than or equal to the wave value
        int maxIndex = Mathf.FloorToInt((float)wave / 5);
        // Ensure index is within bounds
        Debug.Log("Calculated Index: " + maxIndex);
        return maxIndex; // Adjust for zero-based index
    }

    private void StopMovement()
    {
        isMoving = false; // Set moving state to false
        characterBase.PlayMoveAnim(Vector3.zero); // Play idle animation

        stopMovementAction?.Invoke();

    }

    public void LoadPlayerState()
    {

        int currentWaypointIndex = CalculateWaypointIndex(gameConfig.bestWave);
        GameObject mainCamera = GameObject.FindWithTag("MainCamera");

        if (currentWaypointIndex >= 0 && currentWaypointIndex < waypoints.Count)
        {
            Debug.Log("Here");
            transform.position = waypoints[currentWaypointIndex]; // Set player position to the saved waypoint

            //mainCamera.GetComponent<CameraFollow>().hasToFollow = true;

        }
        else
        {
            Debug.LogWarning("Invalid waypoint index when loading player state: " + currentWaypointIndex);
        }

        // Remove enemies between player and waypoint
        RemoveEnemiesBelowMaxWave(gameConfig.bestWave);
    }

    private void RemoveEnemiesBelowMaxWave(int maxWave)
    {
        // Get all enemies tagged as "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            EnemyMainMenu enemyScript = enemy.GetComponent<EnemyMainMenu>();

            if (enemyScript != null)
            {
                int enemyIndexPosition = enemyScript.GetWavePosition();

                // Check if the enemy's indexPosition is less than the maxWave
                if (enemyIndexPosition <= maxWave)
                {
                    Destroy(enemy);
                }
            }
        }
    }

    public void LoadAnimator()
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
}
