using UnityEngine;

// Если хотите отделить логику анимаций от движения
public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 lastMovementDirection = Vector2.down; // По умолчанию смотрит вниз

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Этот метод вызывается из Player.cs
    public void UpdateAnimations(Vector2 movementInput, bool isMoving)
    {
        // Обновляем направление взгляда
        if (movementInput.magnitude > 0.1f)
        {
            lastMovementDirection = movementInput.normalized;
        }

        // Устанавливаем параметры анимации
        animator.SetBool("IsRunning", isMoving);

        if (isMoving)
        {
            // Для движения используем текущее направление
            animator.SetFloat("MoveX", movementInput.x);
            animator.SetFloat("MoveY", movementInput.y);
        }
        else
        {
            // Для idle используем последнее направление
            animator.SetFloat("MoveX", lastMovementDirection.x);
            animator.SetFloat("MoveY", lastMovementDirection.y);
        }

        // Разворот спрайта (если движение только по горизонтали)
        if (Mathf.Abs(movementInput.x) > 0.1f)
        {
            spriteRenderer.flipX = movementInput.x < 0;
        }
    }

    // Метод для внешнего управления анимацией (например, при получении урона)
    public void PlayAnimation(string animationName)
    {
        animator.Play(animationName);
    }

    // Метод для установки триггера
    public void SetTrigger(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }
}