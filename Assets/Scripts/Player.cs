using System;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{

    public static Player Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs: EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 7.0f;
    [SerializeField] GameInput gameInput;
    [SerializeField] LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private bool isWalking;
    private Vector3 lastInteractDirection;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"There is more than one {this.GetType().Name} instance");
        }
        Instance = this;
    }

    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

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
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                // Hit a BaseCounter
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
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
            canMove = movementDirection.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, movementDirectionX, moveDistance);
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
                canMove = movementDirection.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, movementDirectionZ, moveDistance);
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

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObjectFollowTransform(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) { this.kitchenObject = kitchenObject; }

    public KitchenObject GetKitchenObject() { return kitchenObject; }

    public void ClearKitchenObject() { kitchenObject = null; }

    public bool HasKitchenObject() { return kitchenObject != null; }
}
