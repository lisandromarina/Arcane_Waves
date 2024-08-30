using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour
{
    [SerializeField] private RectTransform joystickRectTransform; // Reference to the joystick RectTransform
    [SerializeField] private RectTransform barPanelRectTransform; // Reference to the Bar panel RectTransform
    private Canvas canvas;

    private bool isTouched = false; // Flag to check if the joystick has been moved once

    private void Start()
    {
        // Initialize Canvas reference
        canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas not found. Ensure JoystickController is a child of a Canvas.");
        }

        if (joystickRectTransform == null)
        {
            Debug.LogError("Joystick RectTransform is not assigned.");
        }

        if (barPanelRectTransform == null)
        {
            Debug.LogError("Bar Panel RectTransform is not assigned.");
        }

        // Hide the joystick initially
        joystickRectTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Only handle touch input if the game is not over
        if (!GameManager.Instance.IsGameOver())
        {
            HandleTouchInput();
        }
        else
        {
            // Hide the joystick if the game is over
            joystickRectTransform.gameObject.SetActive(false);
        }
    }

    private void HandleTouchInput()
    {
        if (joystickRectTransform == null || canvas == null || barPanelRectTransform == null) return;

        // Check if there are any touches
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // Get the first touch

            // Convert screen point to local point in canvas space
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(), // Convert relative to canvas
                touch.position,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera, // Use camera for ScreenSpace - Camera or World Space
                out localPoint
            );

            // Check if the touch is over the Bar panel only for TouchPhase.Began
            if (touch.phase == TouchPhase.Began && IsTouchOverPanel(touch.position, barPanelRectTransform))
            {
                return; // Do nothing if touch is over the Bar panel at the beginning
            }

            // Check for touch phase
            if (touch.phase == TouchPhase.Began && !isTouched)
            {
                // Update joystick position directly to the converted local point
                joystickRectTransform.anchoredPosition = localPoint;

                // Show the joystick
                joystickRectTransform.gameObject.SetActive(true);

                // Simulate joystick press
                SimulatePointerDown(touch.position);

                // Mark the joystick as moved
                isTouched = true;
            }
            else if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && isTouched)
            {
                // Simulate joystick release
                SimulatePointerUp();

                // Hide the joystick when the touch is released
                joystickRectTransform.gameObject.SetActive(false);

                // Reset the flag when the touch is released
                isTouched = false;
            }
            else if (touch.phase == TouchPhase.Moved && isTouched)
            {
                // Simulate dragging the joystick
                SimulatePointerDrag(touch.position);
            }
        }
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

    private void SimulatePointerDown(Vector2 touchPosition)
    {
        // Create PointerEventData and simulate pointer down
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = touchPosition
        };

        // Execute pointer down event
        ExecuteEvents.Execute(joystickRectTransform.gameObject, pointerData, ExecuteEvents.pointerDownHandler);

        // Optionally log or debug here to verify if the event is being executed correctly
        Debug.Log($"Simulated Pointer Down at {touchPosition}");
    }

    private void SimulatePointerUp()
    {
        // Create PointerEventData and simulate pointer up
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(joystickRectTransform.gameObject, pointerData, ExecuteEvents.pointerUpHandler);
    }

    private void SimulatePointerDrag(Vector2 touchPosition)
    {
        // Create PointerEventData and simulate pointer drag
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = touchPosition
        };
        ExecuteEvents.Execute(joystickRectTransform.gameObject, pointerData, ExecuteEvents.dragHandler);
    }
}
