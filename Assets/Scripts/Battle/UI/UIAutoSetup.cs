using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Reflection;
using UnityEngine.InputSystem.UI;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(UIAutoSetup))]
public class UIAutoSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UIAutoSetup setup = (UIAutoSetup)target;

        if (GUILayout.Button("СОЗДАТЬ ВЕСЬ UI"))
            setup.SetupAllUI();

        if (GUILayout.Button("Создать только Top Panel"))
            setup.CreateTopPanel();

        if (GUILayout.Button("Создать Battle UI"))
            setup.CreateBattleUI();

        if (GUILayout.Button("Создать Shop UI"))
            setup.CreateShopUI();

        if (GUILayout.Button("Очистить весь UI"))
            ClearAllUI();
    }

    void ClearAllUI()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas != null)
            DestroyImmediate(canvas.gameObject);

        EventSystem eventSystem = FindAnyObjectByType<EventSystem>();
        if (eventSystem != null)
            DestroyImmediate(eventSystem.gameObject);
    }
}
#endif

public class UIAutoSetup : MonoBehaviour
{
    [Header("Основные настройки")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Font textFont;
    [SerializeField] private TMP_FontAsset tmpFont;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private int fontSize = 24;

    [Header("Панели")]
    [SerializeField] private Color topPanelColor = new Color(0.1f, 0.1f, 0.2f, 0.9f);
    [SerializeField] private Color battlePanelColor = new Color(0.2f, 0.2f, 0.3f, 0.8f);
    [SerializeField] private Color shopPanelColor = new Color(0.3f, 0.3f, 0.4f, 0.9f);
    [SerializeField] private Color shopSlotColor = new Color(0.4f, 0.4f, 0.5f, 0.9f);
    [SerializeField] private Color benchSlotColor = new Color(0.3f, 0.3f, 0.4f, 0.8f);

    [Header("Размеры")]
    [SerializeField] private Vector2 topPanelSize = new Vector2(1920, 100);
    [SerializeField] private Vector2 unitInfoPanelSize = new Vector2(300, 200);
    [SerializeField] private Vector2 shopSlotSize = new Vector2(200, 300);
    [SerializeField] private Vector2 benchSlotSize = new Vector2(100, 100);
    [SerializeField] private Vector2 shopPanelSize = new Vector2(1200, 700);

    // Ссылки на созданные элементы
    private GameObject topPanel;
    private GameObject battlePanel;
    private GameObject shopPanel;
    private GameObject unitInfoPanel;
    private GameObject preparationPanel;
    private GameObject resultsPanel;

    private TextMeshProUGUI turnText;
    private TextMeshProUGUI goldText;
    private TextMeshProUGUI roundText;

    void Start()
    {
        if (canvas == null) canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null) CreateCanvas();

        SetupAllUI();

        EnsureEventSystem();
        Debug.Log($"EventSystem.current: {EventSystem.current}");
        Debug.Log($"CurrentInputModule: {EventSystem.current?.currentInputModule}");
    }

    void EnsureEventSystem()
    {
        var es = FindAnyObjectByType<EventSystem>();
        if (es == null)
        {
            var go = new GameObject("EventSystem");
            es = go.AddComponent<EventSystem>();
            go.AddComponent<InputSystemUIInputModule>();
        }
    }

    void CreateCanvas()
    {
        GameObject canvasGO = new GameObject("Canvas");
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // подгоняем размер чтобы интерфейс не "разъезжался" на разных разрешениях
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();
    }

    public void SetupAllUI()
    {
        // 1. Создаем основные панели
        CreateTopPanel();
        CreateBattleUI();
        CreateShopUI();

        // 2. Назначаем ссылки в менеджерах
        AssignReferences();

        // 3. Прячем ненужные панели
        if (shopPanel != null)
            shopPanel.SetActive(true); // Магазин видим по умолчанию

        if (resultsPanel != null)
            resultsPanel.SetActive(false);

        Debug.Log("UI автоматически настроен!");
    }

    public void CreateTopPanel()
    {
        if (canvas == null)
            canvas = FindAnyObjectByType<Canvas>();

        if (canvas == null)
            CreateCanvas();

        // Удаляем старую панель если существует
        if (topPanel != null)
            DestroyImmediate(topPanel);

        topPanel = CreatePanel("TopPanel", canvas.transform);

        // растягиваем по ширине экрана и задаем фикс. высоту.
        SetTopBarRect(topPanel, Mathf.Max(60f, topPanelSize.y));

        // Добавляем горизонтальный layout
        HorizontalLayoutGroup hlg = topPanel.AddComponent<HorizontalLayoutGroup>();
        hlg.padding = new RectOffset(20, 20, 10, 10);
        hlg.spacing = 40;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = true;
        hlg.childForceExpandHeight = true;

        // Создаем три текстовых поля
        turnText = CreateText("TurnText", "Ход игрока", topPanel.transform,
            TextAlignmentOptions.Left, new Vector2(0, 0));
        EnsureLayoutElement(turnText.gameObject, preferredWidth: 0, preferredHeight: 0, flexibleWidth: 1, flexibleHeight: 1);

        goldText = CreateText("GoldText", "Золото: 100", topPanel.transform,
            TextAlignmentOptions.Center, new Vector2(0, 0));
        EnsureLayoutElement(goldText.gameObject, preferredWidth: 0, preferredHeight: 0, flexibleWidth: 1, flexibleHeight: 1);

        roundText = CreateText("RoundText", "Раунд: 1", topPanel.transform,
            TextAlignmentOptions.Right, new Vector2(0, 0));
        EnsureLayoutElement(roundText.gameObject, preferredWidth: 0, preferredHeight: 0, flexibleWidth: 1, flexibleHeight: 1);

        // Цвет фона панели
        Image panelImage = topPanel.GetComponent<Image>();
        if (panelImage != null)
            panelImage.color = topPanelColor;
    }

    public void CreateBattleUI()
    {
        if (canvas == null)
            canvas = FindAnyObjectByType<Canvas>();

        if (canvas == null)
            CreateCanvas();

        // Удаляем старые панели если существуют
        if (battlePanel != null)
            DestroyImmediate(battlePanel);

        if (unitInfoPanel != null)
            DestroyImmediate(unitInfoPanel);

        // Основная панель боя
        battlePanel = CreatePanel("BattlePanel", canvas.transform);

        // растягиваем панель боя на весь экран
        SetStretchRect(battlePanel, Vector2.zero, Vector2.zero);

        Image battleImage = battlePanel.GetComponent<Image>();
        if (battleImage != null)
            battleImage.color = battlePanelColor;

        battlePanel.SetActive(false); // Скрываем по умолчанию

        // Панель информации о юните
        unitInfoPanel = CreatePanel("UnitInfoPanel", battlePanel.transform);

        // Привязываем в левый нижний угол с отступом
        SetAnchorBottomLeft(unitInfoPanel, new Vector2(20, 20), unitInfoPanelSize);

        // Заполняем UnitInfoPanel
        CreateUnitInfoElements();

        // Панель подготовки
        preparationPanel = CreatePanel("PreparationPanel", canvas.transform);
        SetCenterFixed(preparationPanel, new Vector2(600, 400));

        TextMeshProUGUI prepText = CreateText("PrepText",
            "ФАЗА ПОДГОТОВКИ\n\nКупите юнитов в магазине\nи нажмите 'Начать бой'",
            preparationPanel.transform, TextAlignmentOptions.Center, new Vector2(0, 0));
        prepText.fontSize = 32;
        EnsureLayoutElement(prepText.gameObject, preferredWidth: 0, preferredHeight: 0, flexibleWidth: 1, flexibleHeight: 1);

        // Кнопка старт (внизу панели подготовки)
        GameObject startButton = CreateButton("StartBattleButton", "НАЧАТЬ БОЙ",
            preparationPanel.transform, Vector2.zero, new Vector2(0, 50));
        EnsureLayoutElement(startButton, preferredWidth: 0, preferredHeight: 50, flexibleWidth: 1, flexibleHeight: 0);

        // Делаем вертикальный layout для панели подготовки
        SetupVerticalLayout(preparationPanel, padding: new RectOffset(20, 20, 20, 20), spacing: 15, alignment: TextAnchor.MiddleCenter);

        // Назначаем обработчик на кнопку
        Button buttonComp = startButton.GetComponent<Button>();
        if (buttonComp != null)
        {
            buttonComp.onClick.AddListener(() =>
            {
                Debug.Log("Кнопка 'Начать бой' нажата!");

                // Переключаем видимость
                if (preparationPanel != null) preparationPanel.SetActive(false);
                if (battlePanel != null) battlePanel.SetActive(true);
                if (shopPanel != null) shopPanel.SetActive(false);
            });
        }

        // Панель результатов
        resultsPanel = CreatePanel("ResultsPanel", canvas.transform);
        SetCenterFixed(resultsPanel, new Vector2(600, 400));
        resultsPanel.SetActive(false);

        TextMeshProUGUI resultsText = CreateText("ResultsText", "РЕЗУЛЬТАТЫ",
            resultsPanel.transform, TextAlignmentOptions.Center, new Vector2(0, 0));
        resultsText.fontSize = 36;
        resultsText.color = Color.green;
        EnsureLayoutElement(resultsText.gameObject, preferredWidth: 0, preferredHeight: 0, flexibleWidth: 1, flexibleHeight: 1);

        SetupVerticalLayout(resultsPanel, padding: new RectOffset(20, 20, 20, 20), spacing: 10, alignment: TextAnchor.MiddleCenter);

        void CreateUnitInfoElements()
        {
            // выравниваем через LayoutGroup, чтобы ничего не перекрывалось и не вылезало за границы
            SetupVerticalLayout(unitInfoPanel,
                padding: new RectOffset(10, 10, 10, 10),
                spacing: 8,
                alignment: TextAnchor.UpperLeft);

            // Имя юнита
            TextMeshProUGUI unitName = CreateText("UnitNameText", "Юнит: Не выбран",
                unitInfoPanel.transform, TextAlignmentOptions.Left, new Vector2(0, 0));
            EnsureLayoutElement(unitName.gameObject, preferredWidth: 0, preferredHeight: 30, flexibleWidth: 1, flexibleHeight: 0);

            // Полоса здоровья - исправленная версия
            GameObject hpSlider = CreateSimpleSlider("HealthSlider", unitInfoPanel.transform, Vector2.zero, new Vector2(0, 0));
            EnsureLayoutElement(hpSlider, preferredWidth: 0, preferredHeight: 20, flexibleWidth: 1, flexibleHeight: 0);

            // Текст здоровья
            TextMeshProUGUI healthText = CreateText("HealthText", "100/100",
                unitInfoPanel.transform, TextAlignmentOptions.Center, new Vector2(0, 0));
            EnsureLayoutElement(healthText.gameObject, preferredWidth: 0, preferredHeight: 24, flexibleWidth: 1, flexibleHeight: 0);

            // Кнопки действий...
            GameObject actionPanel = CreatePanel("ActionPanel", unitInfoPanel.transform);
            Image actionImg = actionPanel.GetComponent<Image>();
            if (actionImg != null) actionImg.color = new Color(0, 0, 0, 0); // прозрачный фон, чтобы не мешал

            EnsureLayoutElement(actionPanel, preferredWidth: 0, preferredHeight: 44, flexibleWidth: 1, flexibleHeight: 0);

            HorizontalLayoutGroup actionHLG = actionPanel.AddComponent<HorizontalLayoutGroup>();
            actionHLG.padding = new RectOffset(0, 0, 0, 0);
            actionHLG.spacing = 8;
            actionHLG.childAlignment = TextAnchor.MiddleCenter;
            actionHLG.childControlWidth = true;
            actionHLG.childControlHeight = true;
            actionHLG.childForceExpandWidth = true;
            actionHLG.childForceExpandHeight = true;

            GameObject moveBtn = CreateButton("MoveButton", "ДВИЖЕНИЕ", actionPanel.transform, Vector2.zero, new Vector2(0, 40));
            EnsureLayoutElement(moveBtn, preferredWidth: 0, preferredHeight: 40, flexibleWidth: 1, flexibleHeight: 1);

            GameObject attackBtn = CreateButton("AttackButton", "АТАКА", actionPanel.transform, Vector2.zero, new Vector2(0, 40));
            EnsureLayoutElement(attackBtn, preferredWidth: 0, preferredHeight: 40, flexibleWidth: 1, flexibleHeight: 1);

            GameObject waitBtn = CreateButton("WaitButton", "ЖДАТЬ", actionPanel.transform, Vector2.zero, new Vector2(0, 40));
            EnsureLayoutElement(waitBtn, preferredWidth: 0, preferredHeight: 40, flexibleWidth: 1, flexibleHeight: 1);

            // Кнопка конца хода
            GameObject endTurnBtn = CreateButton("EndTurnButton", "ЗАКОНЧИТЬ ХОД", unitInfoPanel.transform, Vector2.zero, new Vector2(0, 40));
            EnsureLayoutElement(endTurnBtn, preferredWidth: 0, preferredHeight: 40, flexibleWidth: 1, flexibleHeight: 0);
        }
    }

    public void CreateShopUI()
    {
        if (canvas == null)
            canvas = FindAnyObjectByType<Canvas>();

        if (canvas == null)
            CreateCanvas();

        // Удаляем старую панель если существует
        if (shopPanel != null)
            DestroyImmediate(shopPanel);

        // Создаем основную панель магазина
        shopPanel = CreatePanel("ShopPanel", canvas.transform);
        SetCenterFixed(shopPanel, shopPanelSize);

        Image shopPanelImage = shopPanel.GetComponent<Image>();
        if (shopPanelImage != null)
            shopPanelImage.color = shopPanelColor;

        // Главный вертикальный layout магазина:
        // Заголовок -> слоты -> скамейка -> кнопки
        SetupVerticalLayout(shopPanel,
            padding: new RectOffset(20, 20, 20, 20),
            spacing: 12,
            alignment: TextAnchor.UpperCenter);

        // Заголовок магазина
        TextMeshProUGUI shopTitle = CreateText("ShopTitle", "МАГАЗИН",
            shopPanel.transform, TextAlignmentOptions.Center, new Vector2(0, 0));
        shopTitle.fontSize = 32;
        EnsureLayoutElement(shopTitle.gameObject, preferredWidth: 0, preferredHeight: 60, flexibleWidth: 1, flexibleHeight: 0);

        // Панель слотов магазина
        GameObject shopSlotsPanel = CreatePanel("ShopSlotsPanel", shopPanel.transform);
        Image slotsImg = shopSlotsPanel.GetComponent<Image>();
        if (slotsImg != null) slotsImg.color = new Color(0, 0, 0, 0); // прозрачная подложка

        // высота слотов: ограничиваем чтобы не вылезало за shopPanel
        float slotsPanelHeight = Mathf.Clamp(shopPanelSize.y * 0.45f, 180f, 320f);
        EnsureLayoutElement(shopSlotsPanel, preferredWidth: 0, preferredHeight: slotsPanelHeight, flexibleWidth: 1, flexibleHeight: 0);

        HorizontalLayoutGroup shopHLG = shopSlotsPanel.AddComponent<HorizontalLayoutGroup>();
        shopHLG.padding = new RectOffset(0, 0, 0, 0);
        shopHLG.spacing = 20;
        shopHLG.childAlignment = TextAnchor.MiddleCenter;
        shopHLG.childControlWidth = true;
        shopHLG.childControlHeight = true;
        shopHLG.childForceExpandWidth = true;
        shopHLG.childForceExpandHeight = true;

        // Вычисляем размер слота (чтобы влезал в ширину)
        float availableWidth = (shopPanelSize.x - 40f) - (shopHLG.spacing * 4f); // padding shopPanel (20+20) + 4 промежутка
        float slotWidth = Mathf.Floor(availableWidth / 5f);
        float slotHeight = Mathf.Min((float)shopSlotSize.y, slotsPanelHeight); // не превышать высоту панели слотов
        Vector2 computedShopSlotSize = new Vector2(Mathf.Max(140f, slotWidth), Mathf.Max(180f, slotHeight));

        // Создаем 5 слотов магазина
        for (int i = 0; i < 5; i++)
        {
            GameObject slot = CreateShopSlot($"ShopSlot{i + 1}", shopSlotsPanel.transform, computedShopSlotSize);

            // Добавляем обработчик для кнопки покупки
            Button buyButton = slot.GetComponentInChildren<Button>();
            if (buyButton != null)
            {
                int slotIndex = i; // Для замыкания
                buyButton.onClick.AddListener(() => OnBuyButtonClicked(slotIndex));
            }
        }

        // Панель скамейки (Bench)
        GameObject benchPanel = CreatePanel("BenchPanel", shopPanel.transform);
        Image benchImg = benchPanel.GetComponent<Image>();
        if (benchImg != null) benchImg.color = new Color(0, 0, 0, 0);

        // Даем столько высоты чтобы 1 ряд + spacing гарантированно помещались
        float benchPanelHeight = Mathf.Clamp(shopPanelSize.y * 0.18f, 90f, 140f);
        EnsureLayoutElement(benchPanel, preferredWidth: 0, preferredHeight: benchPanelHeight, flexibleWidth: 1, flexibleHeight: 0);

        GridLayoutGroup benchGLG = benchPanel.AddComponent<GridLayoutGroup>();
        benchGLG.spacing = new Vector2(10, 10);
        benchGLG.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        benchGLG.constraintCount = 1;
        benchGLG.startAxis = GridLayoutGroup.Axis.Horizontal;
        benchGLG.childAlignment = TextAnchor.MiddleCenter;

        // Подгоняем cellSize под ширину панели
        // 10 колонок * 1 ряд = 10 ячеек
        float benchAvailableWidth = (shopPanelSize.x - 40f) - (benchGLG.spacing.x * 9f);
        float benchCell = Mathf.Floor(benchAvailableWidth / 10f);
        benchCell = Mathf.Clamp(benchCell, 50f, benchSlotSize.x);
        benchGLG.cellSize = new Vector2(benchCell, benchCell);

        // Создаем 10 слотов скамейки
        for (int i = 0; i < 10; i++)
            CreateBenchSlot($"BenchSlot{i + 1}", benchPanel.transform, benchGLG.cellSize);

        // Кнопки магазина (в одной строке)
        GameObject buttonsRow = CreatePanel("ShopButtonsRow", shopPanel.transform);
        Image rowImg = buttonsRow.GetComponent<Image>();
        if (rowImg != null) rowImg.color = new Color(0, 0, 0, 0);

        EnsureLayoutElement(buttonsRow, preferredWidth: 0, preferredHeight: 55, flexibleWidth: 1, flexibleHeight: 0);

        HorizontalLayoutGroup buttonsHLG = buttonsRow.AddComponent<HorizontalLayoutGroup>();
        buttonsHLG.padding = new RectOffset(0, 0, 0, 0);
        buttonsHLG.spacing = 12;
        buttonsHLG.childAlignment = TextAnchor.MiddleCenter;
        buttonsHLG.childControlWidth = true;
        buttonsHLG.childControlHeight = true;
        buttonsHLG.childForceExpandWidth = true;
        buttonsHLG.childForceExpandHeight = true;

        GameObject refreshButton = CreateButton("RefreshButton", "ОБНОВИТЬ (2 золота)", buttonsRow.transform, Vector2.zero, new Vector2(0, 50));
        EnsureLayoutElement(refreshButton,
            preferredWidth: 0, preferredHeight: 50,
            flexibleWidth: 1, flexibleHeight: 1,
            minWidth: 200, minHeight: 0);

        Button refreshBtnComp = refreshButton.GetComponent<Button>();
        if (refreshBtnComp != null)
            refreshBtnComp.onClick.AddListener(() => OnRefreshButtonClicked());

        GameObject closeButton = CreateButton("CloseShopButton", "ЗАКРЫТЬ МАГАЗИН", buttonsRow.transform, Vector2.zero, new Vector2(0, 50));
        var rt = closeButton.GetComponent<RectTransform>();
        Debug.Log($"Close RT size: {rt.rect.size}");
        EnsureLayoutElement(closeButton,
            preferredWidth: 0, preferredHeight: 50,
            flexibleWidth: 1, flexibleHeight: 1,
            minWidth: 200, minHeight: 0);

        Button closeBtnComp = closeButton.GetComponent<Button>();
        if (closeBtnComp != null)
            closeBtnComp.onClick.AddListener(() =>
            {
                Debug.Log("Close button clicked");
                var shopUI = shopPanel.GetComponent<ShopUI>();
                shopUI.CloseClicked();
            });

        Debug.Log("Магазин создан успешно!");

        GameObject CreateShopSlot(string name, Transform parent, Vector2 slotSize)
        {
            GameObject slot = CreatePanel(name, parent);

            // В LayoutGroup позиция задается автоматом
            // Для размера используем LayoutElement
            EnsureLayoutElement(slot, preferredWidth: slotSize.x, preferredHeight: slotSize.y, flexibleWidth: 0, flexibleHeight: 0);

            // Цвет фона слота
            Image slotImage = slot.GetComponent<Image>();
            if (slotImage != null)
                slotImage.color = shopSlotColor;

            // Добавляем компонент ShopSlotUI если он есть
            if (slot.GetComponent<ShopSlotUI>() == null)
                slot.AddComponent<ShopSlotUI>();

            // Вертикальная раскладка элементов внутри слота
            SetupVerticalLayout(slot,
                padding: new RectOffset(10, 10, 10, 10),
                spacing: 6,
                alignment: TextAnchor.UpperCenter);

            // Иконка юнита
            GameObject icon = CreateImage("UnitIcon", slot.transform);
            Image iconImage = icon.GetComponent<Image>();
            if (iconImage != null)
                iconImage.color = new Color(0.8f, 0.8f, 0.9f, 1f); // Светло-серый цвет для иконки

            float iconSize = Mathf.Min(slotSize.x - 20f, slotSize.y * 0.45f);
            EnsureLayoutElement(icon, preferredWidth: 0, preferredHeight: iconSize, flexibleWidth: 1, flexibleHeight: 0);

            // Имя юнита
            TextMeshProUGUI unitName = CreateText("UnitName", "Юнит",
                slot.transform, TextAlignmentOptions.Center, new Vector2(0, 0));
            EnsureLayoutElement(unitName.gameObject, preferredWidth: 0, preferredHeight: 26, flexibleWidth: 1, flexibleHeight: 0);

            // Цена
            TextMeshProUGUI costText = CreateText("CostText", "100 золота",
                slot.transform, TextAlignmentOptions.Center, new Vector2(0, 0));
            EnsureLayoutElement(costText.gameObject, preferredWidth: 0, preferredHeight: 24, flexibleWidth: 1, flexibleHeight: 0);

            // Кнопка покупки
            GameObject buyButton = CreateButton("BuyButton", "КУПИТЬ",
                slot.transform, Vector2.zero, new Vector2(0, 34));
            EnsureLayoutElement(buyButton, preferredWidth: 0, preferredHeight: 34, flexibleWidth: 1, flexibleHeight: 0);

            return slot;
        }

        GameObject CreateBenchSlot(string name, Transform parent, Vector2 cellSize)
        {
            GameObject slot = CreatePanel(name, parent);

            // Для GridLayoutGroup размер задается через cellSize
            // LayoutElement так то не обязателен
            // но можно оставить для надежности.
            EnsureLayoutElement(slot, preferredWidth: cellSize.x, preferredHeight: cellSize.y, flexibleWidth: 0, flexibleHeight: 0);

            // Цвет фона слота скамейки
            Image slotImage = slot.GetComponent<Image>();
            if (slotImage != null)
                slotImage.color = benchSlotColor;

            // Добавляем компонент BenchSlotUI если он есть
            if (slot.GetComponent<BenchSlotUI>() == null)
                slot.AddComponent<BenchSlotUI>();

            // Текст "Пусто"
            TextMeshProUGUI emptyText = CreateText("EmptyText", "Пусто", slot.transform,
                TextAlignmentOptions.Center, new Vector2(0, 0));
            emptyText.fontSize = 14;
            emptyText.color = new Color(0.7f, 0.7f, 0.7f, 1f);

            // Растягиваем текст на весь слот
            RectTransform rt = emptyText.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }

            return slot;
        }
    }

    // Методы для обработки кликов в магазине
    void OnBuyButtonClicked(int slotIndex)
    {
        Debug.Log($"Купить юнит из слота {slotIndex + 1}");

        // Здесь будет логика покупки юнита
        if (ShopManager.Instance != null)
            ShopManager.Instance.BuyUnit(slotIndex);
    }

    void OnRefreshButtonClicked()
    {
        Debug.Log("Обновить магазин");

        // Здесь будет логика обновления магазина
        if (ShopManager.Instance != null)
            ShopManager.Instance.RefreshShop();
    }

    void OnCloseShopButtonClicked()
    {
        Debug.Log("Закрыть магазин");

        // Скрываем магазин, показываем панель подготовки
        if (shopPanel != null) shopPanel.SetActive(false);
        if (preparationPanel != null) preparationPanel.SetActive(true);
    }

    // ===== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ =====
    GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name);
        if (parent != null)
            panel.transform.SetParent(parent, false);

        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.3f, 0.8f);

        image.raycastTarget = false;

        return panel;
    }

    // привязываем кнопки к действиям
    void AssignShopUIReferences(ShopUI shopUI)
    {
        if (shopUI == null || shopPanel == null) return;

        Button refreshBtn = shopPanel.transform.Find("ShopButtonsRow/RefreshButton")?.GetComponent<Button>();
        Button closeBtn = shopPanel.transform.Find("ShopButtonsRow/CloseShopButton")?.GetComponent<Button>();

        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        shopUI.GetType().GetField("refreshButton", flags)?.SetValue(shopUI, refreshBtn);
        shopUI.GetType().GetField("closeShopButton", flags)?.SetValue(shopUI, closeBtn);
    }

    TextMeshProUGUI CreateText(string name, string text, Transform parent,
        TextAlignmentOptions alignment, Vector2 size)
    {
        GameObject textGO = new GameObject(name);
        if (parent != null)
            textGO.transform.SetParent(parent, false);

        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = textColor;
        tmp.alignment = alignment;

        if (tmpFont != null)
            tmp.font = tmpFont;

        // Настройка RectTransform
        RectTransform rt = textGO.GetComponent<RectTransform>();
        if (rt != null)
        {
            // Если размер не задан (0,0) оставляем размер под LayoutElement/LayoutGroup
            if (size.x > 0 || size.y > 0)
                rt.sizeDelta = size;
            else
                rt.sizeDelta = Vector2.zero;
        }

        return tmp;
    }

    GameObject CreateButton(string name, string buttonText, Transform parent,
        Vector2 position, Vector2 size)
    {
        GameObject buttonGO = new GameObject(name);
        if (parent != null)
            buttonGO.transform.SetParent(parent, false);

        // Image для фона кнопки
        Image image = buttonGO.AddComponent<Image>();
        image.color = new Color(0.2f, 0.4f, 0.8f, 1f);

        // Button компонент
        Button button = buttonGO.AddComponent<Button>();

        // Текст кнопки
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);

        TextMeshProUGUI text = textGO.AddComponent<TextMeshProUGUI>();
        text.raycastTarget = false;
        text.text = buttonText;
        text.fontSize = fontSize - 4;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;

        // Настройка RectTransform кнопки
        RectTransform rt = buttonGO.GetComponent<RectTransform>();
        if (rt != null)
        {
            // В layout-режиме size обычно задается LayoutElement-ом,
            // но если size задан явно — применим
            if (size.x > 0 || size.y > 0)
                rt.sizeDelta = size;

            // anchoredPosition vможно оставлять, LayoutGroup все равно переопределит
            rt.anchoredPosition = position;
        }

        // Центрируем текст
        RectTransform textRT = textGO.GetComponent<RectTransform>();
        if (textRT != null)
        {
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.sizeDelta = Vector2.zero;
            textRT.anchoredPosition = Vector2.zero;
        }

        return buttonGO;
    }

    GameObject CreateSimpleSlider(string name, Transform parent, Vector2 position, Vector2 size)
    {
        GameObject sliderGO = new GameObject(name);
        if (parent != null)
            sliderGO.transform.SetParent(parent, false);

        Slider slider = sliderGO.AddComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.value = 75;

        // добавляем фон/филл, чтобы полоса была похожа на UI
        // потом заменим на префабы
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(sliderGO.transform, false);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0f, 0f, 0f, 0.4f);

        RectTransform bgRT = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;

        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderGO.transform, false);
        RectTransform fillAreaRT = fillArea.AddComponent<RectTransform>();
        fillAreaRT.anchorMin = Vector2.zero;
        fillAreaRT.anchorMax = Vector2.one;
        fillAreaRT.offsetMin = new Vector2(2, 2);
        fillAreaRT.offsetMax = new Vector2(-2, -2);

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = new Color(0.2f, 0.9f, 0.2f, 0.9f);

        RectTransform fillRT = fill.GetComponent<RectTransform>();
        fillRT.anchorMin = new Vector2(0, 0);
        fillRT.anchorMax = new Vector2(1, 1);
        fillRT.offsetMin = Vector2.zero;
        fillRT.offsetMax = Vector2.zero;

        slider.fillRect = fillRT;
        slider.targetGraphic = fillImg;

        // Настройка RectTransform
        RectTransform rt = sliderGO.GetComponent<RectTransform>();
        if (rt != null)
        {
            // Если слайдер используется внутри LayoutGroup — позицию не задаем "жестко"
            // но оставим совместимость с прежней версией
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);

            rt.anchoredPosition = position;

            if (size.x > 0 || size.y > 0)
                rt.sizeDelta = size;
            else
                rt.sizeDelta = Vector2.zero;
        }

        return sliderGO;
    }

    GameObject CreateImage(string name, Transform parent)
    {
        GameObject imageGO = new GameObject(name);
        if (parent != null)
            imageGO.transform.SetParent(parent, false);

        Image image = imageGO.AddComponent<Image>();
        image.color = new Color(0.5f, 0.5f, 0.5f, 1f);

        image.raycastTarget = false;

        return imageGO;
    }

    void SetRectTransform(GameObject obj, Vector2 anchorMin, Vector2 anchorMax,
        Vector2 pivot, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        RectTransform rt = obj.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = sizeDelta;
        }
        else
        {
            Debug.LogWarning($"Объект {obj.name} не имеет RectTransform!");
        }
    }


    void SetStretchRect(GameObject obj, Vector2 offsetMin, Vector2 offsetMax)
    {
        RectTransform rt = obj.GetComponent<RectTransform>();
        if (rt == null) return;

        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = offsetMin;   // паддинг слева, снизу
        rt.offsetMax = offsetMax;   // справа, сверху
    }

    void SetCenterFixed(GameObject obj, Vector2 size)
    {
        RectTransform rt = obj.GetComponent<RectTransform>();
        if (rt == null) return;

        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = size;
    }

    void SetAnchorBottomLeft(GameObject obj, Vector2 offset, Vector2 size)
    {
        RectTransform rt = obj.GetComponent<RectTransform>();
        if (rt == null) return;

        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(0f, 0f);
        rt.pivot = new Vector2(0f, 0f);
        rt.anchoredPosition = offset;
        rt.sizeDelta = size;
    }

    void SetTopBarRect(GameObject obj, float height)
    {
        RectTransform rt = obj.GetComponent<RectTransform>();
        if (rt == null) return;

        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);

        // фикс высота, растяжение по ширине
        rt.offsetMin = new Vector2(0f, -height);
        rt.offsetMax = new Vector2(0f, 0f);
    }

    void SetupVerticalLayout(GameObject obj, RectOffset padding, float spacing, TextAnchor alignment)
    {
        // На всякий случай убираем конфликтующие компоненты
        var oldHLG = obj.GetComponent<HorizontalLayoutGroup>();
        if (oldHLG != null) DestroyImmediate(oldHLG);

        var oldGLG = obj.GetComponent<GridLayoutGroup>();
        if (oldGLG != null) DestroyImmediate(oldGLG);

        var vlg = obj.GetComponent<VerticalLayoutGroup>();
        if (vlg == null) vlg = obj.AddComponent<VerticalLayoutGroup>();

        vlg.padding = padding;
        vlg.spacing = spacing;
        vlg.childAlignment = alignment;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        // Чтобы размеры корректно пересчитывались
        var fitter = obj.GetComponent<ContentSizeFitter>();
        if (fitter != null) DestroyImmediate(fitter);
    }

    LayoutElement EnsureLayoutElement(GameObject go, float preferredWidth, float preferredHeight, float flexibleWidth, float flexibleHeight)
    {
        LayoutElement le = go.GetComponent<LayoutElement>();
        if (le == null) le = go.AddComponent<LayoutElement>();

        // не фиксируем
        le.preferredWidth = preferredWidth > 0 ? preferredWidth : -1;
        le.preferredHeight = preferredHeight > 0 ? preferredHeight : -1;

        le.flexibleWidth = flexibleWidth;
        le.flexibleHeight = flexibleHeight;

        return le;
    }

    void EnsureLayoutElement(GameObject go, float preferredWidth, float preferredHeight, float flexibleWidth, float flexibleHeight, float minWidth = 0, float minHeight = 0)
    {
        LayoutElement le = EnsureLayoutElement(go, preferredWidth, preferredHeight, flexibleWidth, flexibleHeight);
        le.minWidth = minWidth;
        le.minHeight = minHeight;
    }

    // Перегрузка, чтобы удобно вызывать с фикс. высотой и "растяжением" по ширине
    LayoutElement EnsureLayoutElement(GameObject go, float preferredWidth, float preferredHeight, float flexibleWidth, float flexibleHeight, bool keep = true)
    {
        return EnsureLayoutElement(go, preferredWidth, preferredHeight, flexibleWidth, flexibleHeight);
    }

    void AssignReferences()
    {
        // Создаем BattleUI если его нет
        BattleUI battleUI = FindAnyObjectByType<BattleUI>();
        if (battleUI == null)
        {
            GameObject battleUIGO = new GameObject("BattleUI");
            battleUI = battleUIGO.AddComponent<BattleUI>();
            battleUIGO.transform.SetParent(canvas.transform, false);
        }

        // Создаем ShopUI если его нет
        ShopUI shopUI = FindAnyObjectByType<ShopUI>();
        if (shopUI == null && shopPanel != null)
            shopUI = shopPanel.AddComponent<ShopUI>();

        AssignShopUIReferences(shopUI);
        shopUI.RebindButtons();

        // Используем рефлексию для назначения ссылок
        var fields = battleUI.GetType().GetFields(
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Public);

        foreach (var field in fields)
        {
            try
            {
                // Для TextMeshProUGUI полей
                if (field.FieldType == typeof(TextMeshProUGUI))
                {
                    if (field.Name == "turnText") field.SetValue(battleUI, turnText);
                    if (field.Name == "goldText") field.SetValue(battleUI, goldText);
                    if (field.Name == "roundText") field.SetValue(battleUI, roundText);
                }
                // Для GameObject полей
                else if (field.FieldType == typeof(GameObject))
                {
                    if (field.Name == "topPanel" || field.Name == "topPanelObject")
                        field.SetValue(battleUI, topPanel);

                    if (field.Name == "battlePanel" || field.Name == "battlePanelObject")
                        field.SetValue(battleUI, battlePanel);

                    if (field.Name == "unitInfoPanel" || field.Name == "unitInfoPanelObject")
                        field.SetValue(battleUI, unitInfoPanel);

                    if (field.Name == "shopPanel" || field.Name == "shopPanelObject")
                        field.SetValue(battleUI, shopPanel);

                    if (field.Name == "preparationPanel" || field.Name == "preparationPanelObject")
                        field.SetValue(battleUI, preparationPanel);

                    if (field.Name == "resultsPanel" || field.Name == "resultsPanelObject")
                        field.SetValue(battleUI, resultsPanel);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Не удалось назначить поле {field.Name}: {e.Message}");
            }
        }
    }
}
