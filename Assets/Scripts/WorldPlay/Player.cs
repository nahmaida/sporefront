using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public Animator playerAnimator;
    public SpriteRenderer playerSpriteRenderer;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private string currentAnimation;

    // Ќазвани€ должны точно совпадать с блоками в Animator
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
        if (GameInput.Instance != null)
        {
            moveInput = GameInput.Instance.MoveInput;
        }

        UpdateAnimation();
    }

    void FixedUpdate()
    {
        if (moveInput.magnitude > 0.1f)
        {
            rb.MovePosition(rb.position + moveInput.normalized * speed * Time.fixedDeltaTime);
        }
    }

    void UpdateAnimation()
    {
        if (playerAnimator == null) return;

        bool isMoving = moveInput.magnitude > 0.1f;

        if (!isMoving)
        {
            ChangeAnimationState(IDLE);
            return;
        }

        float absX = Mathf.Abs(moveInput.x);
        float absY = Mathf.Abs(moveInput.y);

        if (absX > absY)
        {
            ChangeAnimationState(RUN);
            playerSpriteRenderer.flipX = moveInput.x < 0;
        }
        else
        {
            if (moveInput.y > 0)
                ChangeAnimationState(RUN_UP);
            else
                ChangeAnimationState(RUN_DOWN);

            playerSpriteRenderer.flipX = false;
        }
    }

    void ChangeAnimationState(string newAnimation)
    {
        if (currentAnimation == newAnimation) return;

        playerAnimator.Play(newAnimation);
        currentAnimation = newAnimation;
    }
}
