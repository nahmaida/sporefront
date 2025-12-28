using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private PlayerInputActions playerInputActions;
    private Vector2 moveInput;

    public Vector2 MoveInput => moveInput;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    private void Update()
    {
        moveInput = playerInputActions.Player.Move.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        playerInputActions?.Enable();
    }

    private void OnDisable()
    {
        playerInputActions?.Disable();
    }
}
