using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput playerInput;


    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        Debug.Log("befor stick");
        Vector2 dir = playerInput.actions["Move"].ReadValue<Vector2>();
        Debug.Log(dir);
       
    }
}