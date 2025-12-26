// Assets/Scripts/UI/ShopUI.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance { get; private set; }

    [SerializeField] private List<ShopSlotUI> shopSlots = new List<ShopSlotUI>();
    [SerializeField] private List<BenchSlotUI> benchSlots = new List<BenchSlotUI>();
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button closeShopButton;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        if (refreshButton != null) refreshButton.onClick.AddListener(OnRefreshClicked);
        if (closeShopButton != null) closeShopButton.onClick.AddListener(OnCloseClicked);
    }

    public void OpenShop()
    {
        gameObject.SetActive(true);
        UpdateUI();
    }

    public void CloseShop()
    {
        gameObject.SetActive(false);
    }

    public void UpdateShopUI(List<ShopManager.ShopSlot> currentShop)
    {
        for (int i = 0; i < shopSlots.Count && i < currentShop.Count; i++)
        {
            shopSlots[i].UpdateSlot(currentShop[i]);
        }
    }

    public void UpdateBenchUI(List<ShopManager.UnitInstance> benchUnits)
    {
        for (int i = 0; i < benchSlots.Count; i++)
        {
            if (i < benchUnits.Count) benchSlots[i].UpdateSlot(benchUnits[i]);
            else benchSlots[i].ClearSlot();
        }
    }

    void UpdateUI()
    {
        if (ShopManager.Instance != null)
        {
            UpdateShopUI(ShopManager.Instance.CurrentShop);
            UpdateBenchUI(ShopManager.Instance.BenchUnits);
        }
    }

    void OnRefreshClicked()
    {
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.RefreshShop();
            UpdateUI();
        }
    }

    void OnCloseClicked()
    {
        CloseShop();
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.EndPreparationPhase();
        }
    }
}