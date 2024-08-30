using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : BaseCharacter
{
    private Rigidbody2D rb;
    private Vector2 movementInput;
    private bool isMoving = false;

    [SerializeField] public PlayerAttributes savedStats;
    [SerializeField] private float stopDistanceThreshold = 0.1f;

    private PlayerInput playerInput;
    private InputAction moveAction;

    // Screen boundaries
    private float minYPosition = -110.1f;
    private float maxYPosition = 140f;
    private float minXPosition;
    private float maxXPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        characterBase = GetComponent<Character_Base>();

        Debug.Log(savedStats.speed);
        LoadAttributes();

        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];

        // Calculate screen boundaries
        Camera cam = Camera.main;
        minXPosition = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        maxXPosition = cam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
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
    }

    private void LoadAttributes()
    {
        if (savedStats != null)
        {
            speed = savedStats.speed;
            attackRange = savedStats.attackRange;
            baseDamage = savedStats.baseDamage;
            Debug.Log("Loaded");
        }
        else
        {
            Debug.LogWarning("PlayerAttributes is not assigned.");
        }
    }

    public void Revive()
    {
        health = 100;
        IsAlive = true;
        characterBase.PlayReviveAnim();
    }
}
