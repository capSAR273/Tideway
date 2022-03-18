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
    private bool groundedPlayer;
    
    private InputManager inputManager;
    private Transform cameraTransform;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        cameraTransform = Camera.main.transform;
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
        else if(!controller.isGrounded) //not grounded
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
    }
}