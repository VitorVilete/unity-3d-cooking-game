using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7.0f;
    [SerializeField] GameInput gameInput;
    [SerializeField] LayerMask countersLayerMask;

    private bool isWalking;
    private Vector3 lastInteractDirection;
   
    void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 movementDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        if (movementDirection != Vector3.zero)
        {
            lastInteractDirection = movementDirection;
        }
        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDirection, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter))
            {
                // Hit a ClearCounter
                clearCounter.Interact();
            }
            Debug.Log(raycastHit.transform);
        }
        else
        {
            Debug.Log("-");
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 movementDirection = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        float playerHeight = 2f;
        #region Collision Detection
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, movementDirection, moveDistance);
        if (!canMove)
        {
            // Cannot move towards movementDirection
            // Attempt to move only on X axis
            Vector3 movementDirectionX = new Vector3(movementDirection.x, 0, 0).normalized;
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, movementDirectionX, moveDistance);
            if (canMove)
            {
                // Can move towards movementDirection
                movementDirection = movementDirectionX;
            }
            else
            {
                // Cannot move towards movementDirectionX
                // Attempt to move only on Z axis
                Vector3 movementDirectionZ = new Vector3(movementDirection.x, 0, 0).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, movementDirectionZ, moveDistance);
                if (canMove)
                {
                    // Can move towards movementDirectionZ
                    movementDirection = movementDirectionZ;
                }
                else
                {
                    // Cannot move in any direction
                }
            }
        }
        if (canMove)
        {
            // Can move on the desired direction
            transform.position += movementDirection * moveDistance;
        }
        #endregion
        isWalking = movementDirection != Vector3.zero;

        float rotateSpeed = 10f;
        //Use Slerp for lerping rotation
        transform.forward = Vector3.Slerp(transform.forward, movementDirection, Time.deltaTime * rotateSpeed);
    }

}
