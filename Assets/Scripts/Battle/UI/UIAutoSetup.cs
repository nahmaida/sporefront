using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

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
        {
            setup.SetupAllUI();
        }

        if (GUILayout.Button("Создать только Top Panel"))
        {
            setup.CreateTopPanel();
        }

        if (GUILayout.Button("Создать Battle UI"))
        {
            setup.CreateBattleUI();
        }

        if (GUILayout.Button("Создать Shop UI"))
        {
            setup.CreateShopUI();
        }

        if (GUILayout.Button("Очистить весь UI"))
        {
            ClearAllUI();
        }
    }

    void ClearAllUI()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas != null)
        {
            DestroyImmediate(canvas.gameObject);
        }

        EventSystem eventSystem = FindAnyObjectByType<EventSystem>();
        if (eventSystem != null)
        {
            DestroyImmediate(eventSystem.gameObject);
        }
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
        if (canvas == null)
        {
            canvas = FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                CreateCanvas();
            }
        }

        SetupAllUI();
    }

    void CreateCanvas()
    {
        GameObject canvasGO = new GameObject("Canvas");
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Создаем EventSystem если его нет
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
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
        {
            canvas = FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                CreateCanvas();
            }
        }

        // Удаляем старую панель если существует
        if (topPanel != null)
            DestroyImmediate(topPanel);

        topPanel = CreatePanel("TopPanel", canvas.transform);
        SetRectTransform(topPanel, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                        new Vector2(0.5f, 1f), Vector2.zero, topPanelSize);

        // Добавляем горизонтальный layout
        HorizontalLayoutGroup hlg = topPanel.AddComponent<HorizontalLayoutGroup>();
        hlg.padding = new RectOffset(20, 20, 10, 10);
        hlg.spacing = 40;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;

        // Создаем три текстовых поля
        turnText = CreateText("TurnText", "Ход игрока", topPanel.transform,
                            TextAlignmentOptions.Left, new Vector2(400, 50));
        goldText = CreateText("GoldText", "Золото: 100", topPanel.transform,
                            TextAlignmentOptions.Center, new Vector2(400, 50));
        roundText = CreateText("RoundText", "Раунд: 1", topPanel.transform,
                             TextAlignmentOptions.Right, new Vector2(400, 50));

        // Цвет фона панели
        Image panelImage = topPanel.GetComponent<Image>();
        if (panelImage != null)
        {
            panelImage.color = topPanelColor;
        }
    }

    public void CreateBattleUI()
    {
        if (canvas == null)
        {
            canvas = FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                CreateCanvas();
            }
        }

        // Удаляем старые панели если существуют
        if (battlePanel != null)
            DestroyImmediate(battlePanel);
        if (unitInfoPanel != null)
            DestroyImmediate(unitInfoPanel);

        // Основная панель боя
        battlePanel = CreatePanel("BattlePanel", canvas.transform);
        SetRectTransform(battlePanel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1920, 1080));
        battlePanel.SetActive(false); // Скрываем по умолчанию

        // Панель информации о юните
        unitInfoPanel = CreatePanel("UnitInfoPanel", battlePanel.transform);
        SetRectTransform(unitInfoPanel, new Vector2(0f, 0f), new Vector2(0f, 0f),
                        new Vector2(0f, 0f), new Vector2(20, 20), unitInfoPanelSize);

        // Заполняем UnitInfoPanel
        CreateUnitInfoElements();

        // Панель подготовки
        preparationPanel = CreatePanel("PreparationPanel", canvas.transform);
        SetRectTransform(preparationPanel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(600, 400));

        TextMeshProUGUI prepText = CreateText("PrepText", "ФАЗА ПОДГОТОВКИ\n\nКупите юнитов в магазине\nи нажмите 'Начать бой'",
                                            preparationPanel.transform, TextAlignmentOptions.Center, new Vector2(500, 200));
        prepText.fontSize = 32;

        GameObject startButton = CreateButton("StartBattleButton", "НАЧАТЬ БОЙ", preparationPanel.transform,
                    new Vector2(0, -100), new Vector2(200, 50));

        // Назначаем обработчик на кнопку
        Button buttonComp = startButton.GetComponent<Button>();
        if (buttonComp != null)
        {
            buttonComp.onClick.AddListener(() => {
                Debug.Log("Кнопка 'Начать бой' нажата!");
                // Переключаем видимость
                if (preparationPanel != null) preparationPanel.SetActive(false);
                if (battlePanel != null) battlePanel.SetActive(true);
                if (shopPanel != null) shopPanel.SetActive(false);
            });
        }

        // Панель результатов
        resultsPanel = CreatePanel("ResultsPanel", canvas.transform);
        SetRectTransform(resultsPanel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(600, 400));
        resultsPanel.SetActive(false);

        TextMeshProUGUI resultsText = CreateText("ResultsText", "РЕЗУЛЬТАТЫ",
                                               resultsPanel.transform, TextAlignmentOptions.Center, new Vector2(500, 200));
        resultsText.fontSize = 36;
        resultsText.color = Color.green;
    }

    void CreateUnitInfoElements()
    {
        // Имя юнита
        TextMeshProUGUI unitName = CreateText("UnitNameText", "Юнит: Не выбран",
                                            unitInfoPanel.transform, TextAlignmentOptions.Left, new Vector2(280, 30));
        unitName.transform.localPosition = new Vector3(10, 150, 0);

        // Полоса здоровья - исправленная версия
        CreateSimpleSlider("HealthSlider", unitInfoPanel.transform, new Vector2(0, 100), new Vector2(280, 20));

        // Текст здоровья
        TextMeshProUGUI healthText = CreateText("HealthText", "100/100",
                                              unitInfoPanel.transform, TextAlignmentOptions.Center, new Vector2(280, 30));
        healthText.transform.localPosition = new Vector3(0, 70, 0);

        // Кнопки действий
        GameObject actionPanel = CreatePanel("ActionPanel", unitInfoPanel.transform);
        SetRectTransform(actionPanel, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                        new Vector2(0.5f, 0f), new Vector2(0, 10), new Vector2(280, 80));

        CreateButton("MoveButton", "ДВИЖЕНИЕ", actionPanel.transform,
                    new Vector2(-90, 0), new Vector2(80, 40));
        CreateButton("AttackButton", "АТАКА", actionPanel.transform,
                    new Vector2(0, 0), new Vector2(80, 40));
        CreateButton("WaitButton", "ЖДАТЬ", actionPanel.transform,
                    new Vector2(90, 0), new Vector2(80, 40));

        // Кнопка конца хода
        CreateButton("EndTurnButton", "ЗАКОНЧИТЬ ХОД", unitInfoPanel.transform,
                    new Vector2(0, -60), new Vector2(150, 40));
    }

    public void CreateShopUI()
    {
        if (canvas == null)
        {
            canvas = FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                CreateCanvas();
            }
        }

        // Удаляем старую панель если существует
        if (shopPanel != null)
            DestroyImmediate(shopPanel);

        // Создаем основную панель магазина
        shopPanel = CreatePanel("ShopPanel", canvas.transform);
        SetRectTransform(shopPanel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f), Vector2.zero, shopPanelSize);

        Image shopPanelImage = shopPanel.GetComponent<Image>();
        if (shopPanelImage != null)
        {
            shopPanelImage.color = shopPanelColor;
        }

        // Заголовок магазина
        TextMeshProUGUI shopTitle = CreateText("ShopTitle", "МАГАЗИН",
                                             shopPanel.transform, TextAlignmentOptions.Center, new Vector2(400, 50));
        shopTitle.fontSize = 32;
        shopTitle.transform.localPosition = new Vector3(0, 280, 0);

        // Панель слотов магазина
        GameObject shopSlotsPanel = CreatePanel("ShopSlotsPanel", shopPanel.transform);
        SetRectTransform(shopSlotsPanel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f), new Vector2(0, 50), new Vector2(1100, 200));

        HorizontalLayoutGroup shopHLG = shopSlotsPanel.AddComponent<HorizontalLayoutGroup>();
        shopHLG.spacing = 20;
        shopHLG.childAlignment = TextAnchor.MiddleCenter;

        // Создаем 5 слотов магазина
        for (int i = 0; i < 5; i++)
        {
            GameObject slot = CreateShopSlot($"ShopSlot{i + 1}", shopSlotsPanel.transform);

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
        SetRectTransform(benchPanel, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                        new Vector2(0.5f, 0f), new Vector2(0, 20), new Vector2(1100, 150));

        GridLayoutGroup benchGLG = benchPanel.AddComponent<GridLayoutGroup>();
        benchGLG.cellSize = benchSlotSize;
        benchGLG.spacing = new Vector2(10, 10);
        benchGLG.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        benchGLG.constraintCount = 2;

        // Создаем 10 слотов скамейки
        for (int i = 0; i < 10; i++)
        {
            GameObject benchSlot = CreateBenchSlot($"BenchSlot{i + 1}", benchPanel.transform);
        }

        // Кнопки магазина
        GameObject refreshButton = CreateButton("RefreshButton", "ОБНОВИТЬ (2 золота)", shopPanel.transform,
                    new Vector2(-200, -280), new Vector2(200, 50));

        Button refreshBtnComp = refreshButton.GetComponent<Button>();
        if (refreshBtnComp != null)
        {
            refreshBtnComp.onClick.AddListener(() => OnRefreshButtonClicked());
        }

        GameObject closeButton = CreateButton("CloseShopButton", "ЗАКРЫТЬ МАГАЗИН", shopPanel.transform,
                    new Vector2(200, -280), new Vector2(200, 50));

        Button closeBtnComp = closeButton.GetComponent<Button>();
        if (closeBtnComp != null)
        {
            closeBtnComp.onClick.AddListener(() => OnCloseShopButtonClicked());
        }

        Debug.Log("Магазин создан успешно!");
    }

    GameObject CreateShopSlot(string name, Transform parent)
    {
        GameObject slot = CreatePanel(name, parent);
        SetRectTransform(slot, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f), Vector2.zero, shopSlotSize);

        // Цвет фона слота
        Image slotImage = slot.GetComponent<Image>();
        if (slotImage != null)
        {
            slotImage.color = shopSlotColor;
        }

        // Добавляем компонент ShopSlotUI если он есть
        if (slot.GetComponent<ShopSlotUI>() == null)
        {
            slot.AddComponent<ShopSlotUI>();
        }

        // Иконка юнита
        GameObject icon = CreateImage("UnitIcon", slot.transform);
        SetRectTransform(icon, new Vector2(0.5f, 0.7f), new Vector2(0.5f, 0.7f),
                        new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(150, 150));

        // Добавляем компонент Image для иконки
        Image iconImage = icon.GetComponent<Image>();
        if (iconImage != null)
        {
            iconImage.color = new Color(0.8f, 0.8f, 0.9f, 1f); // Светло-серый цвет для иконки
        }

        // Имя юнита
        TextMeshProUGUI unitName = CreateText("UnitName", "Юнит", slot.transform,
                                            TextAlignmentOptions.Center, new Vector2(180, 30));
        unitName.transform.localPosition = new Vector3(0, 30, 0);

        // Цена
        TextMeshProUGUI costText = CreateText("CostText", "100 золота", slot.transform,
                                            TextAlignmentOptions.Center, new Vector2(180, 30));
        costText.transform.localPosition = new Vector3(0, -40, 0);

        // Кнопка покупки
        GameObject buyButton = CreateButton("BuyButton", "КУПИТЬ", slot.transform,
                    new Vector2(0, -90), new Vector2(100, 30));

        return slot;
    }

    GameObject CreateBenchSlot(string name, Transform parent)
    {
        GameObject slot = CreatePanel(name, parent);
        SetRectTransform(slot, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f), Vector2.zero, benchSlotSize);

        // Цвет фона слота скамейки
        Image slotImage = slot.GetComponent<Image>();
        if (slotImage != null)
        {
            slotImage.color = benchSlotColor;
        }

        // Добавляем компонент BenchSlotUI если он есть
        if (slot.GetComponent<BenchSlotUI>() == null)
        {
            slot.AddComponent<BenchSlotUI>();
        }

        // Текст "Пусто"
        TextMeshProUGUI emptyText = CreateText("EmptyText", "Пусто", slot.transform,
                                             TextAlignmentOptions.Center, new Vector2(80, 30));
        emptyText.fontSize = 14;
        emptyText.color = new Color(0.7f, 0.7f, 0.7f, 1f);

        return slot;
    }

    // Методы для обработки кликов в магазине
    void OnBuyButtonClicked(int slotIndex)
    {
        Debug.Log($"Купить юнит из слота {slotIndex + 1}");
        // Здесь будет логика покупки юнита
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.BuyUnit(slotIndex);
        }
    }

    void OnRefreshButtonClicked()
    {
        Debug.Log("Обновить магазин");
        // Здесь будет логика обновления магазина
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.RefreshShop();
        }
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
        {
            panel.transform.SetParent(parent, false);
        }

        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.3f, 0.8f);

        return panel;
    }

    TextMeshProUGUI CreateText(string name, string text, Transform parent,
                              TextAlignmentOptions alignment, Vector2 size)
    {
        GameObject textGO = new GameObject(name);
        if (parent != null)
        {
            textGO.transform.SetParent(parent, false);
        }

        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = textColor;
        tmp.alignment = alignment;

        if (tmpFont != null)
        {
            tmp.font = tmpFont;
        }

        // Настройка RectTransform
        RectTransform rt = textGO.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.sizeDelta = size;
        }

        return tmp;
    }

    GameObject CreateButton(string name, string buttonText, Transform parent,
                          Vector2 position, Vector2 size)
    {
        GameObject buttonGO = new GameObject(name);
        if (parent != null)
        {
            buttonGO.transform.SetParent(parent, false);
        }

        // Image для фона кнопки
        Image image = buttonGO.AddComponent<Image>();
        image.color = new Color(0.2f, 0.4f, 0.8f, 1f);

        // Button компонент
        Button button = buttonGO.AddComponent<Button>();

        // Текст кнопки
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);

        TextMeshProUGUI text = textGO.AddComponent<TextMeshProUGUI>();
        text.text = buttonText;
        text.fontSize = fontSize - 4;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;

        // Настройка RectTransform кнопки
        RectTransform rt = buttonGO.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.sizeDelta = size;
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
        {
            sliderGO.transform.SetParent(parent, false);
        }

        Slider slider = sliderGO.AddComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.value = 75;

        // Настройка RectTransform
        RectTransform rt = sliderGO.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = position;
            rt.sizeDelta = size;
        }

        return sliderGO;
    }

    GameObject CreateImage(string name, Transform parent)
    {
        GameObject imageGO = new GameObject(name);
        if (parent != null)
        {
            imageGO.transform.SetParent(parent, false);
        }

        Image image = imageGO.AddComponent<Image>();
        image.color = new Color(0.5f, 0.5f, 0.5f, 1f);

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
        {
            shopPanel.AddComponent<ShopUI>();
        }

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