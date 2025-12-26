// Assets/Scripts/UI/BenchSlotUI.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BenchSlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image unitIcon;
    [SerializeField] private Text unitNameText;
    [SerializeField] private GameObject emptySlotIndicator;

    private ShopManager.UnitInstance currentUnit;
    private int slotIndex;

    public void Initialize(int index)
    {
        slotIndex = index;
        ClearSlot();
    }

    public void UpdateSlot(ShopManager.UnitInstance unit)
    {
        currentUnit = unit;

        if (emptySlotIndicator != null) emptySlotIndicator.SetActive(false);

        if (unitIcon != null && unit.unitData.portrait != null)
        {
            unitIcon.sprite = unit.unitData.portrait;
            unitIcon.color = Color.white;
        }

        if (unitNameText != null) unitNameText.text = unit.unitData.unitName;
    }

    public void ClearSlot()
    {
        currentUnit = null;

        if (emptySlotIndicator != null) emptySlotIndicator.SetActive(true);
        if (unitIcon != null)
        {
            unitIcon.sprite = null;
            unitIcon.color = new Color(0, 0, 0, 0);
        }
        if (unitNameText != null) unitNameText.text = "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && currentUnit != null)
        {
            ShopManager.Instance?.SellUnit(slotIndex);
        }
    }
}