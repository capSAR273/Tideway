using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float playerSpeed = 6.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = 20f;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private Vector3 groundedVelocity;
    private Vector3 originalCenter;
    private bool groundedPlayer;
    private bool isCrouching;
    private float originalHeight;
    private float playerHeightCrouching;

    private InputManager inputManager;
    private Transform cameraTransform;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        cameraTransform = Camera.main.transform;
        originalHeight = controller.height;
        originalCenter = controller.center;
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        Vector2 movement = inputManager.GetPlayerMovement();
        Vector3 move = new Vector3(movement.x, 0f, movement.y);
        
        if (controller.isGrounded)
        {
            //Camera Movement
            move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
            move.y = 0f;
            move = Vector3.ClampMagnitude(move, 1f);
            
            //Sprinting on ground
            if (inputManager.isSprinting)
            {
                Debug.Log("Sprinting on ground");
                playerSpeed = 12.0f;
                groundedVelocity = move;
                Debug.Log(groundedVelocity);
                controller.Move(move * Time.deltaTime * playerSpeed);
            }
            else if (!inputManager.isSprinting)
            {
                Debug.Log("Walking on ground");
                playerSpeed = 6.0f;
                groundedVelocity = move;
                Debug.Log(groundedVelocity);
                controller.Move(move * Time.deltaTime * playerSpeed);
            }
        }
        //Not grounded
        else if(!controller.isGrounded)
        {
            move = groundedVelocity;
            move *= Time.deltaTime;
            controller.Move(groundedVelocity * Time.deltaTime * playerSpeed);
        }

        // Handles jumping behavior
        if (inputManager.PlayerJumpedThisFrame() && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        //Handles crouching behavior
        if(inputManager.PlayerCrouching())
        {
            Debug.Log("Crouching");
            isCrouching = true;
            playerSpeed = 3.0f;
            controller.height = originalHeight*0.5f;
            controller.center = controller.height /2 ;
        }
        else if(!inputManager.PlayerCrouching() && isCrouching)
        {
            Debug.Log("Not Crouching");
            Vector3 point0 = transform.position + originalCenter - new Vector3(0.0f, originalHeight, 0.0f);
            Vector3 point1 = transform.position + originalCenter + new Vector3(0.0f, originalHeight, 0.0f);
            if(Physics.OverlapCapsule(point0, point1, controller.radius).Length == 0)
            {
                isCrouching = false;
                controller.height = originalHeight;
                controller.center = originalCenter;
                playerSpeed = 6.0f;
            }
        }
        var lastHeight = controller.height;
        controller.height = Mathf.Lerp(controller.height, originalHeight, 2 * Time.deltaTime);
        controller.center = Vector3.Lerp(controller.center, new Vector3(0, originalCenter, 0), 2 * Time.deltaTime);
        
    }
}