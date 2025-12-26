// Assets/Scripts/UI/ShopSlotUI.cs
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotUI : MonoBehaviour
{
    [SerializeField] private Image unitIcon;
    [SerializeField] private Text unitNameText;
    [SerializeField] private Text unitCostText;
    [SerializeField] private Button buyButton;
    [SerializeField] private GameObject soldOutOverlay;

    private ShopManager.ShopSlot currentSlot;
    private int slotIndex;

    void Start()
    {
        if (buyButton != null) buyButton.onClick.AddListener(OnBuyClicked);
    }

    public void Initialize(int index)
    {
        slotIndex = index;
    }

    public void UpdateSlot(ShopManager.ShopSlot slot)
    {
        currentSlot = slot;

        if (unitIcon != null && slot.unitData.portrait != null)
        {
            unitIcon.sprite = slot.unitData.portrait;
        }

        if (unitNameText != null) unitNameText.text = slot.unitData.unitName;
        if (unitCostText != null) unitCostText.text = $"{slot.cost} золота";
        if (soldOutOverlay != null) soldOutOverlay.SetActive(slot.isSold);
        if (buyButton != null) buyButton.interactable = !slot.isSold;
    }

    void OnBuyClicked()
    {
        if (currentSlot != null && !currentSlot.isSold)
        {
            ShopManager.Instance?.BuyUnit(slotIndex);
        }
    }
}