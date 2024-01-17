using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Vector2 movementInput;
    [SerializeField] private float moveSpeed = 7.0f;
   
    void Start()
    {
        
    }

    void Update()
    {
        Vector3 movementDirection = new Vector3(movementInput.x, 0, movementInput.y);
        transform.position += movementDirection * moveSpeed * Time.deltaTime;

        float rotateSpeed = 10f;
        //Use Slerp for lerping rotation
        transform.forward = Vector3.Slerp(transform.forward, movementDirection, Time.deltaTime * rotateSpeed);
    }

    public bool IsWalking()
    {
        return movementInput != Vector2.zero;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        if (context.performed)
        {
            Debug.Log(context.ToString());
        }
    }
}
