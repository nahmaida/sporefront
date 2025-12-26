using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] public Text turnText;
    [SerializeField] public Text goldText;
    [SerializeField] public Text roundText;
    [SerializeField] public GameObject preparationPanel;
    [SerializeField] public GameObject battlePanel;
    [SerializeField] public GameObject resultsPanel;
    [SerializeField] public Text resultsText;
    [SerializeField] public Text resultsDescription;

    [Header("Unit Info")]
    [SerializeField] public GameObject unitInfoPanel;
    [SerializeField] public Text unitNameText;
    [SerializeField] public Text unitClassText;
    [SerializeField] public Text unitRaceText;
    [SerializeField] public Text unitHealthText;
    [SerializeField] public Slider healthSlider;
    [SerializeField] public Slider manaSlider;
    [SerializeField] public Text manaText;
    [SerializeField] public Text attackText;
    [SerializeField] public Text defenseText;
    [SerializeField] public Text speedText;

    [Header("Action Buttons")]
    [SerializeField] public Button moveButton;
    [SerializeField] public Button attackButton;
    [SerializeField] public Button waitButton;
    [SerializeField] public Button ultimateButton;

    [Header("End Turn")]
    [SerializeField] public Button endTurnButton;
    [SerializeField] public Button endPreparationButton;

    private BattleUnit currentSelectedUnit;

    void Start()
    {
        // Настройка кнопок
        if (moveButton != null) moveButton.onClick.AddListener(OnMoveClicked);
        if (attackButton != null) attackButton.onClick.AddListener(OnAttackClicked);
        if (waitButton != null) waitButton.onClick.AddListener(OnWaitClicked);
        if (ultimateButton != null) ultimateButton.onClick.AddListener(OnUltimateClicked);
        if (endTurnButton != null) endTurnButton.onClick.AddListener(OnEndTurnClicked);
        if (endPreparationButton != null) endPreparationButton.onClick.AddListener(OnEndPreparationClicked);
    }

    public void ShowPreparationUI()
    {
        if (preparationPanel != null) preparationPanel.SetActive(true);
        if (battlePanel != null) battlePanel.SetActive(false);
        if (resultsPanel != null) resultsPanel.SetActive(false);

        if (unitInfoPanel != null) unitInfoPanel.SetActive(false);
    }

    public void ShowBattleUI()
    {
        if (preparationPanel != null) preparationPanel.SetActive(false);
        if (battlePanel != null) battlePanel.SetActive(true);
        if (resultsPanel != null) resultsPanel.SetActive(false);
    }

    public void ShowResults(bool playerWon, int round)
    {
        if (resultsPanel != null) resultsPanel.SetActive(true);
        if (preparationPanel != null) preparationPanel.SetActive(false);
        if (battlePanel != null) battlePanel.SetActive(false);

        if (resultsText != null)
        {
            resultsText.text = playerWon ? "ПОБЕДА!" : "ПОРАЖЕНИЕ!";
            resultsText.color = playerWon ? Color.green : Color.red;
        }

        if (resultsDescription != null)
        {
            if (playerWon)
            {
                resultsDescription.text = $"Вы выиграли раунд {round}!\nНажмите для продолжения...";
            }
            else
            {
                resultsDescription.text = "Ваши герои пали в бою.\nИгра окончена.";
            }
        }
    }

    public void SelectUnit(BattleUnit unit)
    {
        currentSelectedUnit = unit;

        if (unit == null)
        {
            if (unitInfoPanel != null) unitInfoPanel.SetActive(false);
            return;
        }

        if (unitInfoPanel != null) unitInfoPanel.SetActive(true);

        var unitData = unit.GetUnitData();
        if (unitData == null) return;

        if (unitNameText != null) unitNameText.text = unitData.unitName;
        if (unitClassText != null) unitClassText.text = $"Класс: {unitData.characterClass}";
        if (unitRaceText != null) unitRaceText.text = $"Раса: {unitData.race}";
        if (attackText != null) attackText.text = $"Атака: {unit.GetAttack()}";
        if (defenseText != null) defenseText.text = $"Защита: {unit.GetDefense()}";
        if (speedText != null) speedText.text = $"Скорость: {unit.GetSpeed()}";

        UpdateUnitInfo();
    }

    public void UpdateUnitInfo()
    {
        if (currentSelectedUnit == null) return;

        if (unitHealthText != null)
        {
            unitHealthText.text = $"{currentSelectedUnit.GetCurrentHealth()}/{currentSelectedUnit.GetMaxHealth()}";
        }

        if (healthSlider != null)
        {
            healthSlider.maxValue = currentSelectedUnit.GetMaxHealth();
            healthSlider.value = currentSelectedUnit.GetCurrentHealth();
        }

        // Обновление кнопок действий
        if (attackButton != null) attackButton.interactable = !currentSelectedUnit.HasActed();
        if (moveButton != null) moveButton.interactable = !currentSelectedUnit.HasActed();
        if (ultimateButton != null) ultimateButton.interactable = !currentSelectedUnit.HasActed();
    }

    public void UpdateGold(int gold)
    {
        if (goldText != null) goldText.text = $"Золото: {gold}";
    }

    public void UpdateRound(int round)
    {
        if (roundText != null) roundText.text = $"Раунд: {round}";
    }

    public void UpdateTurn(string turnInfo)
    {
        if (turnText != null) turnText.text = turnInfo;
    }

    // Обработчики кнопок
    void OnMoveClicked()
    {
        if (currentSelectedUnit != null)
        {
            BattleManager.Instance?.SetActionMode(ActionMode.Move);
        }
    }

    void OnAttackClicked()
    {
        if (currentSelectedUnit != null)
        {
            BattleManager.Instance?.SetActionMode(ActionMode.Attack);
        }
    }

    void OnWaitClicked()
    {
        if (currentSelectedUnit != null)
        {
            BattleManager.Instance?.SkipTurn();
        }
    }

    void OnUltimateClicked()
    {
        if (currentSelectedUnit != null)
        {
            BattleManager.Instance?.SetActionMode(ActionMode.Ultimate);
        }
    }

    void OnEndTurnClicked()
    {
        BattleManager.Instance?.SkipTurn();
    }

    void OnEndPreparationClicked()
    {
        BattleManager.Instance?.EndPreparationPhase();
    }
}