// BattleUI.cs - полная версия
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
    [Header("Текстовые элементы")]
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI unitNameText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI resultsText;

    [Header("Панели")]
    public GameObject topPanel;
    public GameObject battlePanel;
    public GameObject unitInfoPanel;
    public GameObject preparationPanel;
    public GameObject resultsPanel;
    public GameObject unitInfoPanelObject;

    [Header("Кнопки")]
    public Button moveButton;
    public Button attackButton;
    public Button waitButton;
    public Button endTurnButton;
    public Button startBattleButton;
    public Button continueButton;

    [Header("Slider")]
    public Slider healthSlider;

    [Header("Дополнительные тексты")]
    public TextMeshProUGUI unitHealthText;

    private BattleUnit currentSelectedUnit;

    void Start()
    {
        // Инициализация кнопок
        if (startBattleButton != null)
        {
            startBattleButton.onClick.AddListener(OnStartBattleClicked);
        }

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClicked);
        }

        if (moveButton != null)
        {
            moveButton.onClick.AddListener(OnMoveClicked);
        }

        if (attackButton != null)
        {
            attackButton.onClick.AddListener(OnAttackClicked);
        }

        if (waitButton != null)
        {
            waitButton.onClick.AddListener(OnWaitClicked);
        }

        if (endTurnButton != null)
        {
            endTurnButton.onClick.AddListener(OnEndTurnClicked);
        }

        // Показываем панель подготовки по умолчанию
        ShowPreparationUI();
    }

    void OnStartBattleClicked()
    {
        Debug.Log("Начало боя!");
        ShowBattleUI();

        // Уведомляем BattleManager
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.EndPreparationPhase();
        }
    }

    void OnContinueClicked()
    {
        Debug.Log("Продолжить");
        ShowPreparationUI();
    }

    void OnMoveClicked()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.SetActionMode(ActionMode.Move);
        }
    }

    void OnAttackClicked()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.SetActionMode(ActionMode.Attack);
        }
    }

    void OnWaitClicked()
    {
        if (currentSelectedUnit != null && BattleManager.Instance != null)
        {
            BattleManager.Instance.SkipTurn();
        }
    }

    void OnEndTurnClicked()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.SkipTurn();
        }
    }

    public void ShowPreparationUI()
    {
        if (preparationPanel != null) preparationPanel.SetActive(true);
        if (battlePanel != null) battlePanel.SetActive(false);
        if (unitInfoPanel != null) unitInfoPanel.SetActive(false);
        if (resultsPanel != null) resultsPanel.SetActive(false);
    }

    public void ShowBattleUI()
    {
        if (preparationPanel != null) preparationPanel.SetActive(false);
        if (battlePanel != null) battlePanel.SetActive(true);
        if (unitInfoPanel != null) unitInfoPanel.SetActive(true);
        if (resultsPanel != null) resultsPanel.SetActive(false);
    }

    public void ShowResults(bool playerWon, int round)
    {
        if (preparationPanel != null) preparationPanel.SetActive(false);
        if (battlePanel != null) battlePanel.SetActive(false);
        if (unitInfoPanel != null) unitInfoPanel.SetActive(false);
        if (resultsPanel != null) resultsPanel.SetActive(true);

        if (resultsText != null)
        {
            resultsText.text = playerWon ? "ПОБЕДА!" : "ПОРАЖЕНИЕ!";
            resultsText.color = playerWon ? Color.green : Color.red;
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
        if (unitNameText != null) unitNameText.text = unit.GetUnitData().unitName;

        UpdateUnitInfo();
    }

    public void UpdateUnitInfo()
    {
        if (currentSelectedUnit == null) return;

        if (healthText != null)
        {
            healthText.text = $"{currentSelectedUnit.GetCurrentHealth()}/{currentSelectedUnit.GetMaxHealth()}";
        }

        if (unitHealthText != null)
        {
            unitHealthText.text = $"{currentSelectedUnit.GetCurrentHealth()}/{currentSelectedUnit.GetMaxHealth()}";
        }

        if (healthSlider != null)
        {
            healthSlider.maxValue = currentSelectedUnit.GetMaxHealth();
            healthSlider.value = currentSelectedUnit.GetCurrentHealth();
        }
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
}