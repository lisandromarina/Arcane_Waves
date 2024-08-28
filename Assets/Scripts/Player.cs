using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : BaseCharacter
{
    private Rigidbody2D rb;
    private Vector3 touchPosition;
    private bool isMoving = false;

    [SerializeField] public PlayerAttributes savedStats;

    [SerializeField]
    private float stopDistanceThreshold = 0.1f; // Threshold to stop the soldier when close enough to the target

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        characterBase = GetComponent<Character_Base>();

        Debug.Log(savedStats.speed);
        LoadAttributes();
    }

    void Update()
    {
        base.Update();
        if (Input.touchCount > 0 && IsAlive)
        {
            Touch touch = Input.GetTouch(0);

            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return; // If the touch is over a UI element, don't process movement
            }

            Vector3 screenTouchPosition = touch.position;
            screenTouchPosition.z = Camera.main.transform.position.z;

            touchPosition = Camera.main.ScreenToWorldPoint(screenTouchPosition);
            touchPosition.z = 0;

            if (touch.phase == TouchPhase.Began)
            {
                // Start moving if the touch position is far enough from the current position
                if (Vector3.Distance(transform.position, touchPosition) > stopDistanceThreshold)
                {
                    isMoving = true;
                    characterBase.PlayMoveAnim((touchPosition - transform.position).normalized);
                }
            }
        }

        if (isMoving && IsAlive)
        {
            // Move the soldier frame-by-frame towards the target position
            transform.position = Vector3.MoveTowards(transform.position, touchPosition, speed * Time.deltaTime);

            // Check if the soldier is close enough to stop
            if (Vector3.Distance(transform.position, touchPosition) <= stopDistanceThreshold)
            {
                isMoving = false;
                characterBase.PlayMoveAnim(Vector3.zero); // Stop the animation when stopping
            }
        }

        // Stop moving if the soldier dies
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
}