using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;

    public static InputManager Instance {
        get {
            return _instance;
        }
    }

    private PlayerControls playerControls;
    public bool isSprinting;
    public bool playerJumped;

    private void Awake() {
        if( _instance != null && _instance != this){
            Destroy(this.gameObject);
        }
        else{
            _instance = this;
        }
        playerControls = new PlayerControls();
        Cursor.visible = false;
        playerControls.Player.SprintStart.performed += x => PlayerSprinting();
        playerControls.Player.SprintStop.performed += x => PlayerWalking();
    }

    private void OnEnable() {
        playerControls.Enable();
    }
    
    private void OnDisable() {
        playerControls.Disable();
    }

    public Vector2 GetPlayerMovement() {
        return playerControls.Player.Movement.ReadValue<Vector2>();
    }

    public Vector2 GetMouseDelta() {
        return playerControls.Player.Look.ReadValue<Vector2>();
    }

    public bool PlayerJumpedThisFrame() {
        playerJumped = true;
        return playerControls.Player.Jump.triggered;
    }

    public bool PlayerCrouching () {
        return playerControls.Player.Crouch.triggered;
    }

    private void PlayerSprinting () {
        isSprinting = true;
    }

    private void PlayerWalking () {
        isSprinting = false;
    }

    public bool PlayerSprintJumped() {
        return playerJumped && isSprinting;
    }
}
