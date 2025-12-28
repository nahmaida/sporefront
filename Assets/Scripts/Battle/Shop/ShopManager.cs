// Assets/Scripts/Shop/ShopManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    [System.Serializable]
    public class ShopSlot
    {
        public UnitDataSO unitData;
        public int cost;
        public bool isSold;
    }

    [System.Serializable]
    public class UnitInstance
    {
        public UnitDataSO unitData;
        public int starLevel;
        public int experience;
    }

    [Header("Shop Settings")]
    [SerializeField] private int shopSlots = 5;
    [SerializeField] private int refreshCost = 2;
    [SerializeField] private List<UnitDataSO> allUnitPool = new List<UnitDataSO>();

    [Header("Current Shop")]
    [SerializeField] private List<ShopSlot> currentShop = new List<ShopSlot>();

    private List<UnitInstance> benchUnits = new List<UnitInstance>();

    public static ShopManager Instance { get; private set; }
    public List<ShopSlot> CurrentShop => new List<ShopSlot>(currentShop);
    public List<UnitInstance> BenchUnits => new List<UnitInstance>(benchUnits);

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        LoadUnitPool();
        RefreshShop(true);
    }

    void LoadUnitPool()
    {
        var loadedUnits = Resources.LoadAll<UnitDataSO>("Units");
        allUnitPool.AddRange(loadedUnits);
    }

    public void RefreshShop(bool freeRefresh = false)
    {
        if (!freeRefresh && BattleManager.Instance != null)
        {
            if (!BattleManager.Instance.SpendGold(refreshCost)) return;
        }

        currentShop.Clear();

        for (int i = 0; i < shopSlots; i++)
        {
            if (allUnitPool.Count == 0) continue;

            UnitDataSO randomUnit = allUnitPool[Random.Range(0, allUnitPool.Count)];
            currentShop.Add(new ShopSlot
            {
                unitData = randomUnit,
                cost = randomUnit.shopCost,
                isSold = false
            });
        }

        if (ShopUI.Instance != null)
        {
            ShopUI.Instance.UpdateShopUI(currentShop);
        }
    }

    public void BuyUnit(int shopIndex)
    {
        if (shopIndex < 0 || shopIndex >= currentShop.Count) return;

        ShopSlot slot = currentShop[shopIndex];
        if (slot.isSold) return;

        if (BattleManager.Instance != null && !BattleManager.Instance.SpendGold(slot.cost)) return;

        UnitInstance newUnit = new UnitInstance
        {
            unitData = slot.unitData,
            starLevel = 1,
            experience = 0
        };

        benchUnits.Add(newUnit);
        slot.isSold = true;

        if (ShopUI.Instance != null)
        {
            ShopUI.Instance.UpdateShopUI(currentShop);
            ShopUI.Instance.UpdateBenchUI(benchUnits);
        }
    }

    public void SellUnit(int benchIndex)
    {
        if (benchIndex < 0 || benchIndex >= benchUnits.Count) return;

        UnitInstance unit = benchUnits[benchIndex];
        int sellPrice = unit.unitData.shopCost * unit.starLevel / 2;

        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.AddGold(sellPrice);
        }

        benchUnits.RemoveAt(benchIndex);

        if (ShopUI.Instance != null)
        {
            ShopUI.Instance.UpdateBenchUI(benchUnits);
        }
    }
}