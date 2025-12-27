// BattleUIAutoAssign.cs (упрощенная версия)
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleUIAutoAssign : MonoBehaviour
{
    [ContextMenu("Auto Assign UI Elements")]
    void AutoAssign()
    {
        BattleUI battleUI = GetComponent<BattleUI>();
        if (battleUI == null) return;

        // Автоматически находим элементы по имени
        battleUI.turnText = FindComponent<TextMeshProUGUI>("TurnText");
        battleUI.goldText = FindComponent<TextMeshProUGUI>("GoldText");
        battleUI.roundText = FindComponent<TextMeshProUGUI>("RoundText");

        // Панели
        battleUI.topPanel = GameObject.Find("TopPanel");
        battleUI.battlePanel = GameObject.Find("BattlePanel");
        battleUI.unitInfoPanel = GameObject.Find("UnitInfoPanel");
        battleUI.preparationPanel = GameObject.Find("PreparationPanel");
        battleUI.resultsPanel = GameObject.Find("ResultsPanel");

        // Кнопки
        battleUI.moveButton = FindComponent<Button>("MoveButton");
        battleUI.attackButton = FindComponent<Button>("AttackButton");
        battleUI.waitButton = FindComponent<Button>("WaitButton");
        battleUI.endTurnButton = FindComponent<Button>("EndTurnButton");
        battleUI.startBattleButton = FindComponent<Button>("StartBattleButton");
        battleUI.continueButton = FindComponent<Button>("ContinueButton");

        Debug.Log("UI элементы автоматически назначены!");
    }

    T FindComponent<T>(string name) where T : Component
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null) return obj.GetComponent<T>();
        return null;
    }
}