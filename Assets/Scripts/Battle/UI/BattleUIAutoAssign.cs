// BattleUIAutoAssign.cs - добавьте этот скрипт к BattleUI GameObject
using UnityEngine;
using UnityEngine.UI;

public class BattleUIAutoAssign : MonoBehaviour
{
    [ContextMenu("Auto Assign References")]
    void AutoAssign()
    {
        BattleUI battleUI = GetComponent<BattleUI>();
        if (battleUI == null) return;

        // Находим все элементы по имени
        battleUI.turnText = GameObject.Find("TurnText")?.GetComponent<Text>();
        battleUI.goldText = GameObject.Find("GoldText")?.GetComponent<Text>();
        battleUI.roundText = GameObject.Find("RoundText")?.GetComponent<Text>();

        // Панели
        battleUI.preparationPanel = GameObject.Find("PreparationPanel")?.gameObject;
        battleUI.battlePanel = GameObject.Find("BattleUI")?.gameObject; // или другая панель
        battleUI.resultsPanel = GameObject.Find("ResultsPanel")?.gameObject;
        battleUI.resultsText = GameObject.Find("ResultsText")?.GetComponent<Text>();

        // Панель информации о юните
        battleUI.unitInfoPanel = GameObject.Find("UnitInfoPanel")?.gameObject;
        battleUI.unitNameText = GameObject.Find("UnitNameText")?.GetComponent<Text>();
        battleUI.unitHealthText = GameObject.Find("HealthText")?.GetComponent<Text>();
        battleUI.healthSlider = GameObject.Find("HealthSlider")?.GetComponent<Slider>();

        // Кнопки
        battleUI.moveButton = GameObject.Find("MoveButton")?.GetComponent<Button>();
        battleUI.attackButton = GameObject.Find("AttackButton")?.GetComponent<Button>();
        battleUI.waitButton = GameObject.Find("WaitButton")?.GetComponent<Button>();
        battleUI.endTurnButton = GameObject.Find("EndTurnButton")?.GetComponent<Button>();

        Debug.Log("Auto-assignment completed!");
    }

    void Start()
    {
        AutoAssign();
    }
}