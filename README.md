пример что нужно сделать 


# ПОЛНАЯ ИНСТРУКЦИЯ ПО НАСТРОЙКЕ В UNITY

## 1. СТРУКТУРА ПРОЕКТА

### Создайте папки:
1. **Scripts** (внутри создайте):
   - Core
   - Units
   - Grid
   - Shop
   - UI

2. **Prefabs**
   - Units
   - Grid
   - UI

3. **Sprites**
   - Characters
   - UI
   - Icons

4. **Materials**
5. **Resources**
   - Units (для ScriptableObjects)
   - EnemyUnits

6. **Scenes**
   - MainMenu
   - BattleScene

## 2. СОЗДАНИЕ СКРИПТОВ

### Шаг 1: Создайте все скрипты
Поместите каждый скрипт в соответствующую папку (как указано в структуре выше).

## 3. НАСТРОЙКА СЦЕНЫ БИТВЫ

### Шаг 1: Новая сцена
1. File → New Scene → Basic (Built-in)
2. Сохраните как "BattleScene" в папке Scenes

### Шаг 2: Создайте иерархию объектов
```
Main Camera
Directional Light
BattleSystem (пустой GameObject)
├── BattleManager
├── GridManager
├── ShopManager
└── UI (Canvas)
    ├── BattleUI
    ├── ShopUI
    └── EventSystem
```

### Шаг 3: Настройка GridManager
1. Создайте пустой GameObject "GridManager"
2. Добавьте компонент **BattleGridManager**
3. Настройте параметры:
   ```
   Grid Width: 10
   Grid Height: 5
   Cell Size: 1
   Grid Origin: (0, 0, 0)
   Ally Zone Columns: 4
   Enemy Zone Columns: 4
   ```

4. **Создайте префаб клетки GridCell**:
   - GameObject → 3D Object → Cube
   - Назовите "GridCell"
   - Scale: (0.95, 0.95, 0.1)
   - Перетащите в папку Prefabs/Grid
   - Удалите со сцены

5. **Создайте материалы для зон**:
   - Правой кнопкой в папке Materials → Create → Material
   - Создайте 3 материала:
     - AllyZoneMaterial (синий, Transparency 0.3)
     - EnemyZoneMaterial (красный, Transparency 0.3)
     - NeutralZoneMaterial (серый, Transparency 0.1)
     - HighlightMaterial (желтый, Transparency 0.5)

6. В GridManager назначьте:
   - Grid Cell Prefab: GridCell префаб
   - Материалы для соответствующих зон
   - Highlight Material

### Шаг 4: Настройка BattleManager
1. Создайте пустой GameObject "BattleManager"
2. Добавьте компонент **BattleManager**
3. Настройте параметры:
   ```
   Max Rounds: 30
   Current Round: 1
   Turn Delay: 1
   Player Gold: 100
   Gold Per Round: 10
   ```

### Шаг 5: Создание UI Canvas
1. GameObject → UI → Canvas
2. Настройте Canvas:
   - Render Mode: Screen Space - Overlay
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920 x 1080

3. **Создайте BattleUI**:
   - GameObject → UI → Panel (назовите "BattleUI")
   - Добавьте компонент **BattleUI**
   - Создайте внутри UI элементы:

4. **Элементы BattleUI**:
   ```
   BattleUI (Panel)
   ├── TopPanel (Panel)
   │   ├── TurnText (Text)
   │   ├── GoldText (Text)
   │   └── RoundText (Text)
   ├── UnitInfoPanel (Panel)
   │   ├── UnitNameText (Text)
   │   ├── HealthSlider (Slider)
   │   ├── HealthText (Text)
   │   ├── ActionPanel (Panel)
   │   │   ├── MoveButton (Button)
   │   │   ├── AttackButton (Button)
   │   │   └── WaitButton (Button)
   │   └── EndTurnButton (Button)
   ├── PreparationPanel (Panel)
   │   └── StartBattleButton (Button)
   └── ResultsPanel (Panel)
       ├── ResultsText (Text)
       └── ContinueButton (Button)
   ```

5. **Назначьте ссылки в BattleUI компоненте**:
   - Привяжите все Text, Slider, Button и Panel элементы

6. **Создайте ShopUI**:
   - GameObject → UI → Panel (назовите "ShopUI")
   - Добавьте компонент **ShopUI**
   - Отключите GameObject (по умолчанию не активен)

7. **Структура ShopUI**:
   ```
   ShopUI (Panel)
   ├── ShopSlotsPanel (Horizontal Layout Group)
   │   ├── ShopSlot1 (Panel с ShopSlotUI)
   │   ├── ShopSlot2 (Panel с ShopSlotUI)
   │   ├── ShopSlot3 (Panel с ShopSlotUI)
   │   ├── ShopSlot4 (Panel с ShopSlotUI)
   │   └── ShopSlot5 (Panel с ShopSlotUI)
   ├── BenchPanel (Grid Layout Group)
   │   ├── BenchSlot1 (Panel с BenchSlotUI)
   │   ├── BenchSlot2 (Panel с BenchSlotUI)
   │   └── ... (всего 10 слотов)
   ├── RefreshButton (Button)
   └── CloseShopButton (Button)
   ```

8. **Настройка ShopSlotUI**:
   - Для каждого ShopSlot создайте Panel
   - Добавьте компонент **ShopSlotUI**
   - Внутри Panel создайте:
     - Image (иконка юнита)
     - Text (имя юнита)
     - Text (цена)
     - Button (купить)
     - Panel (sold overlay)

9. **Настройка BenchSlotUI**:
   - Аналогично, но с правым кликом для продажи

### Шаг 6: Создание EventSystem
1. GameObject → UI → EventSystem (автоматически создается с Canvas)

## 4. СОЗДАНИЕ ЮНИТОВ

### Шаг 1: Создание ScriptableObjects
1. **Первый юнит**:
   - Правой кнопкой в папке Resources/Units
   - Create → Battle → Unit Data
   - Назовите "Knight"
   - Заполните данные:
     ```
     Unit Name: Рыцарь
     Character Class: Inquisitor
     Race: Human
     Base Star Level: 1
     Max Star Level: 3
     Max Health: 150
     Attack Power: 25
     Defense: 20
     Speed: 4
     Attack Range: 1.5
     Move Range: 3
     Shop Cost: 100
     Tier: 1
     ```

2. **Создайте еще несколько юнитов** разных классов и рас

### Шаг 2: Создание префабов юнитов
1. **Базовый префаб юнита**:
   - GameObject → 3D Object → Cube (или 2D Sprite если 2D игра)
   - Назовите "UnitBase"
   - Scale: (0.8, 0.8, 0.8)
   - Добавьте компоненты:
     - **BattleUnit**
     - SpriteRenderer (если 2D)
     - Animator (опционально)
     - Box Collider (для кликов)

2. **Настройка BattleUnit компонента**:
   - Selection Indicator: создайте небольшой куб/сферу как дочерний объект для подсветки
   - Sprite Renderer: привяжите компонент
   - Animator: создайте Animator Controller если нужны анимации

3. **Сохраните как префаб**:
   - Перетащите в Prefabs/Units
   - Назовите "KnightPrefab"

4. **Свяжите ScriptableObject**:
   - В префабе KnightPrefab
   - В компоненте BattleUnit в поле "Unit Data" выберите созданный "Knight" ScriptableObject

5. **Повторите для других юнитов**

## 5. НАСТРОЙКА ВРАЖЕСКИХ ЮНИТОВ

1. Создайте папку **Resources/EnemyUnits**
2. Создайте ScriptableObjects для врагов (аналогично игровым)
3. Настройте префабы врагов (можно использовать те же, но с другим ScriptableObject)

## 6. НАСТРОЙКА UnitSelectionManager

1. Создайте новый GameObject "UnitSelectionManager"
2. Добавьте компонент **UnitSelectionManager**
3. Настройте список доступных юнитов:
   - All Units: добавьте созданные префабы юнитов
   - Max Selected Units: 8

## 7. НАСТРОЙКА ShopManager

1. Создайте GameObject "ShopManager"
2. Добавьте компонент **ShopManager**
3. Настройка:
   ```
   Shop Slots: 5
   Refresh Cost: 2
   All Unit Pool: добавьте все ScriptableObjects юнитов
   ```

## 8. СВЯЗЫВАНИЕ ССЫЛОК

### В BattleManager:
- Battle UI: перетащите BattleUI GameObject
- Shop UI: перетащите ShopUI GameObject

### В BattleUI (компонент):
- Привяжите все UI элементы:
  - Turn Text, Gold Text, Round Text
  - Панели: Preparation, Battle, Results
  - Кнопки: Move, Attack, Wait, End Turn
  - Slider и Text для здоровья

### В ShopUI (компонент):
- Shop Slots: добавьте все 5 ShopSlotUI объекты
- Bench Slots: добавьте все 10 BenchSlotUI объекты
- Кнопки: Refresh, Close Shop

### В ShopManager:
- All Unit Pool: добавьте все UnitDataSO ScriptableObjects

### В BattleGridManager:
- Grid Cell Prefab: перетащите GridCell префаб
- Materials: назначьте созданные материалы

## 9. НАСТРОЙКА КАМЕРЫ

1. Выберите Main Camera
2. Настройте Position: (4.5, 2.5, -10)
3. Rotation: (0, 0, 0)
4. Projection: Orthographic (если 2D)
5. Size: 6 (если Orthographic)

## 10. ТЕСТОВАЯ СЦЕНА ДЛЯ ПРОВЕРКИ

### Создайте простую тестовую сцену:
1. **Создайте юнитов на сцене**:
   - Перетащите 2-3 префаба юнитов в сцену
   - Разместите в первых 4 столбцах (игрок)

2. **Создайте врагов**:
   - Перетащите 2-3 вражеских префаба
   - Разместите в последних 4 столбцах

3. **Настройте BattleManager в инспекторе**:
   - Включите Auto Spawn Starting Units: false (если есть такая опция)
   - Или добавьте юниты через код

## 11. СКРИПТ ДЛЯ БЫСТРОГО ТЕСТА

Создайте тестовый скрипт для инициализации:

```csharp
// TestInitializer.cs
using UnityEngine;

public class TestInitializer : MonoBehaviour
{
    [SerializeField] private GameObject[] testPlayerUnits;
    [SerializeField] private GameObject[] testEnemyUnits;
    
    void Start()
    {
        // Добавление тестовых юнитов игрока
        if (UnitSelectionManager.Instance != null)
        {
            foreach (var unit in testPlayerUnits)
            {
                UnitSelectionManager.Instance.AddUnitToSelection(unit);
            }
        }
        
        // Запуск боя
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.SpawnStartingUnits(
                UnitSelectionManager.Instance.SelectedUnits
            );
        }
    }
}
```

## 12. ПОРЯДОК ЗАПУСКА

1. **Запустите сцену BattleScene**
2. **Ожидаемое поведение**:
   - Появится сетка 10x5
   - Откроется магазин (Preparation Phase)
   - Можно покупать юнитов
   - Нажать "Start Battle" или "Close Shop"
   - Начнется бой с пошаговым управлением

## 13. ОТЛАДКА И ЧАСТЫЕ ПРОБЛЕМЫ

### Проблема 1: Null Reference Exceptions
**Решение**: Проверьте все ссылки в инспекторе

### Проблема 2: Юниты не появляются
**Решение**: 
1. Проверьте UnitSelectionManager
2. Проверьте SpawnStartingUnits метод
3. Убедитесь что префабы имеют компонент BattleUnit

### Проблема 3: UI не работает
**Решение**:
1. Проверьте EventSystem
2. Проверьте Canvas настройки
3. Проверьте Button onClick события

### Проблема 4: Клетки не подсвечиваются
**Решение**:
1. Проверьте GridCellClickHandler
2. Проверьте материалы клеток
3. Проверьте BattleGridManager.Instance

## 14. ДОПОЛНИТЕЛЬНЫЕ НАСТРОЙКИ

### Для 2D игры:
1. Измените камеру на Orthographic
2. Используйте Sprite вместо Cube для юнитов
3. Настройте Sorting Layers

### Для лучшей графики:
1. Добавьте освещение
2. Добавьте particle effects для атак
3. Добавьте звуки

### Для баланса:
1. Настройте параметры в ScriptableObjects
2. Измените gold per round
3. Настройте шансы появления в магазине

## 15. ГОТОВЫЕ ПРЕСЕТЫ ДЛЯ БЫСТРОГО СТАРТА

### GridCell Material настройки:
- Rendering Mode: Transparent
- Albedo: цвет с alpha 0.3
- Metallic: 0
- Smoothness: 0

### UI Color Scheme:
- Primary: #2C3E50 (темно-синий)
- Secondary: #ECF0F1 (светло-серый)
- Accent: #E74C3C (красный)
- Success: #27AE60 (зеленый)

### Размеры UI:
- ShopSlot: 200x300
- BenchSlot: 100x100
- Buttons: 160x50

## 16. ТЕСТИРОВАНИЕ КАЖДОЙ ФУНКЦИИ

1. **Магазин**:
   - Купить юнита
   - Обновить магазин
   - Продать юнита

2. **Боевая система**:
   - Выбрать юнита
   - Подвинуть юнита
   - Атаковать врага
   - Пропустить ход

3. **Сетка**:
   - Подсветка доступных клеток
   - Размещение юнитов
   - Движение юнитов

4. **Экономика**:
   - Получение золота за раунд
   - Трата золота
   - Продажа юнитов

Теперь у вас есть полностью рабочая основа для пошаговой тактической игры! Запустите сцену и проверьте все функции.
