using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float moveThreshold = 0.1f;

    [Header("Animation Settings")]
    public Animator playerAnimator;
    public SpriteRenderer playerSpriteRenderer;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private string currentAnimState;

    // Константы должны совпадать с названиями анимационных состояний в окне Animator
    const string IDLE = "Idle";
    const string RUN = "Running";
    const string RUN_UP = "RunningUp";
    const string RUN_DOWN = "RunningDown";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (playerAnimator == null) playerAnimator = GetComponentInChildren<Animator>();
        if (playerSpriteRenderer == null) playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        GetInput();
        HandleAnimation();
    }

    void FixedUpdate()
    {
        if (moveInput.magnitude > moveThreshold)
        {
            rb.MovePosition(rb.position + moveInput.normalized * speed * Time.fixedDeltaTime);
        }
    }

    void GetInput()
    {
        if (GameInput.Instance != null)
            moveInput = GameInput.Instance.MoveInput;
        else
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    void HandleAnimation()
    {
        if (playerAnimator == null) return;

        bool isMoving = moveInput.magnitude > moveThreshold;

        if (!isMoving)
        {
            ChangeAnimationState(IDLE);
            return;
        }

        float absX = Mathf.Abs(moveInput.x);
        float absY = Mathf.Abs(moveInput.y);

        if (absX > absY)
        {
            // Горизонтальный бег (вправо/влево через flipX)
            ChangeAnimationState(RUN);
            if (playerSpriteRenderer != null) playerSpriteRenderer.flipX = moveInput.x < 0;
        }
        else
        {
            // Вертикальный бег
            if (moveInput.y > 0)
                ChangeAnimationState(RUN_UP);
            else
                ChangeAnimationState(RUN_DOWN);

            if (playerSpriteRenderer != null) playerSpriteRenderer.flipX = false;
        }
    }

    private void ChangeAnimationState(string newState)
    {
        // ЭТА СТРОЧКА ИСПРАВЛЯЕТ ЗАСТЫВАНИЕ: 
        // Если анимация уже играет, мы не вызываем Play() повторно.
        if (currentAnimState == newState) return;

        playerAnimator.Play(newState);
        currentAnimState = newState;
    }
}
