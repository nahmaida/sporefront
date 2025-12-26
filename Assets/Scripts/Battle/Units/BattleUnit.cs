// Assets/Scripts/Units/BattleUnit.cs
using UnityEngine;
using System.Collections.Generic;

public class BattleUnit : MonoBehaviour
{
    [System.Serializable]
    public class ActiveStatusEffect
    {
        public EffectType effectType;
        public float duration;
        public int value;
        public BattleUnit source;
    }

    [Header("Unit Data")]
    [SerializeField] private UnitDataSO unitData;
    [SerializeField] private int currentStarLevel = 1;

    [Header("Current Stats")]
    [SerializeField] private int currentHealth;
    [SerializeField] private int currentAttack;
    [SerializeField] private int currentDefense;
    [SerializeField] private int currentSpeed;
    [SerializeField] private int currentMana = 0;
    [SerializeField] private int maxMana = 100;

    [Header("Battle State")]
    [SerializeField] private bool isAlive = true;
    [SerializeField] private bool isSelected = false;
    [SerializeField] private bool hasActedThisTurn = false;
    [SerializeField] private Vector2Int currentGridPosition;
    [SerializeField] private int teamId;

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject selectionIndicator;

    [Header("Status Effects")]
    private List<ActiveStatusEffect> activeEffects = new List<ActiveStatusEffect>();

    public System.Action<BattleUnit> OnUnitDied;

    void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();

        if (selectionIndicator != null)
            selectionIndicator.SetActive(false);
    }

    public void Initialize(UnitDataSO data, int starLevel, int team)
    {
        unitData = data;
        currentStarLevel = starLevel;
        teamId = team;

        CalculateStats();
        currentHealth = GetMaxHealth();

        if (spriteRenderer != null && data.battleSprite != null)
            spriteRenderer.sprite = data.battleSprite;

        if (animator != null && data.animatorController != null)
            animator.runtimeAnimatorController = data.animatorController;

        SetTeamColor();
    }

    void CalculateStats()
    {
        float starMultiplier = 1f + (currentStarLevel - 1) * 0.5f;

        currentAttack = Mathf.RoundToInt(unitData.attackPower * starMultiplier);
        currentDefense = Mathf.RoundToInt(unitData.defense * starMultiplier);
        currentSpeed = Mathf.RoundToInt(unitData.speed * starMultiplier);
    }

    void SetTeamColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = teamId == 0 ?
                new Color(0.2f, 0.4f, 1f, 1f) :
                new Color(1f, 0.2f, 0.2f, 1f);
        }
    }

    public void StartTurn()
    {
        hasActedThisTurn = false;
    }

    public void EndTurn()
    {
        isSelected = false;
        if (selectionIndicator != null)
            selectionIndicator.SetActive(false);
    }

    public void TakeDamage(int damage, DamageType damageType = DamageType.Physical)
    {
        if (!isAlive) return;

        int finalDamage = CalculateDamage(damage, damageType);
        currentHealth = Mathf.Max(0, currentHealth - finalDamage);

        PlayDamageAnimation();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    int CalculateDamage(int incomingDamage, DamageType damageType)
    {
        float damageMultiplier = 1f;

        switch (damageType)
        {
            case DamageType.Physical:
                damageMultiplier -= currentDefense / 100f;
                break;
            case DamageType.Magic:
                damageMultiplier -= currentDefense / 200f;
                break;
        }

        return Mathf.RoundToInt(incomingDamage * Mathf.Max(0.1f, damageMultiplier));
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(GetMaxHealth(), currentHealth + amount);
    }

    void Die()
    {
        isAlive = false;
        PlayDeathAnimation();

        if (BattleGridManager.Instance != null)
        {
            BattleGridManager.Instance.RemoveUnit(currentGridPosition);
        }

        OnUnitDied?.Invoke(this);
        Destroy(gameObject, 2f);
    }

    public void MoveTo(Vector2Int targetPos)
    {
        if (hasActedThisTurn || BattleGridManager.Instance == null) return;

        if (BattleGridManager.Instance.MoveUnit(currentGridPosition, targetPos))
        {
            hasActedThisTurn = true;
            PlayMoveAnimation();
        }
    }

    public void Attack(BattleUnit target)
    {
        if (hasActedThisTurn || target == null || !target.isAlive) return;

        float distance = Vector2Int.Distance(currentGridPosition, target.currentGridPosition);
        if (distance > unitData.attackRange) return;

        PlayAttackAnimation();
        target.TakeDamage(currentAttack);
        hasActedThisTurn = true;
    }

    public void ApplyStatusEffect(StatusEffect effect)
    {
        // Реализация эффектов
    }

    void PlayAttackAnimation() { if (animator != null) animator.SetTrigger("Attack"); }
    void PlayDamageAnimation() { if (animator != null) animator.SetTrigger("Damage"); }
    void PlayDeathAnimation() { if (animator != null) animator.SetTrigger("Die"); }
    void PlayMoveAnimation() { if (animator != null) animator.SetTrigger("Move"); }
    void PlayHealAnimation() { if (animator != null) animator.SetTrigger("Heal"); }

    // Геттеры
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => Mathf.RoundToInt(unitData.maxHealth * (1f + (currentStarLevel - 1) * 0.5f));
    public int GetAttack() => currentAttack;
    public int GetDefense() => currentDefense;
    public int GetSpeed() => currentSpeed;
    public bool HasActed() => hasActedThisTurn;
    public UnitDataSO GetUnitData() => unitData;
    public Vector2Int CurrentGridPosition
    {
        get => currentGridPosition;
        set => currentGridPosition = value;
    }
    public int GetTeamId() => teamId;
    public bool IsAlive() => isAlive;
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (selectionIndicator != null)
            selectionIndicator.SetActive(selected);
    }
}