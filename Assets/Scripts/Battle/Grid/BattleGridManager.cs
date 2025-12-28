// Замените ВЕСЬ файл BattleGridManager.cs на этот:

using UnityEngine;
using System.Collections.Generic;

public class BattleGridManager : MonoBehaviour
{
    [System.Serializable]
    public class GridCellData
    {
        public Vector2Int gridPosition;
        public ZoneType zoneType;
        public BattleUnit occupiedUnit;
        public GameObject cellObject;
        public Renderer cellRenderer;
        public Material originalMaterial;
    }

    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 5;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector3 gridCenter = Vector3.zero; // Центр всей сетки

    [Header("Zones")]
    [SerializeField] private int allyZoneColumns = 4;
    [SerializeField] private int enemyZoneColumns = 4;

    [Header("Visual")]
    [SerializeField] private GameObject gridCellPrefab;
    [SerializeField] private Material allyZoneMaterial;
    [SerializeField] private Material enemyZoneMaterial;
    [SerializeField] private Material neutralZoneMaterial;
    [SerializeField] private Material highlightMaterial;

    private GridCellData[,] gridCells;

    public static BattleGridManager Instance { get; private set; }
    public int GridWidth => gridWidth;
    public int GridHeight => gridHeight;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        gridCells = new GridCellData[gridWidth, gridHeight];

        // Расчет левого нижнего угла как начала координат
        Vector3 leftBottomCorner = gridCenter - new Vector3(
            gridWidth * cellSize / 2f,
            gridHeight * cellSize / 2f,
            0
        );

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Позиция центра клетки
                Vector3 worldPos = leftBottomCorner + new Vector3(
                    x * cellSize + cellSize / 2f,
                    y * cellSize + cellSize / 2f,
                    0
                );

                GameObject cellObj = Instantiate(gridCellPrefab, worldPos, Quaternion.identity, transform);
                cellObj.name = $"Cell_{x}_{y}";

                Renderer renderer = cellObj.GetComponent<Renderer>();
                ZoneType zone = GetZoneForPosition(x);
                Material zoneMaterial = GetMaterialForZone(zone);

                if (renderer != null)
                {
                    renderer.material = zoneMaterial;
                }

                GridCellData cellData = new GridCellData
                {
                    gridPosition = new Vector2Int(x, y),
                    zoneType = zone,
                    occupiedUnit = null,
                    cellObject = cellObj,
                    cellRenderer = renderer,
                    originalMaterial = zoneMaterial
                };

                // Добавляем компонент для кликов
                GridCellClickHandler clickHandler = cellObj.GetComponent<GridCellClickHandler>();
                if (clickHandler == null)
                {
                    clickHandler = cellObj.AddComponent<GridCellClickHandler>();
                }
                clickHandler.Initialize(new Vector2Int(x, y));

                gridCells[x, y] = cellData;
            }
        }

        Debug.Log($"Grid initialized. Cell (0,0) at position: {GetWorldPosition(new Vector2Int(0, 0))}");
        Debug.Log($"Cell (4,0) at position: {GetWorldPosition(new Vector2Int(4, 0))}");
        Debug.Log($"Cell (9,4) at position: {GetWorldPosition(new Vector2Int(9, 4))}");
    }

    public Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        if (!IsPositionInGrid(gridPos)) return Vector3.zero;

        Vector3 leftBottomCorner = gridCenter - new Vector3(
            gridWidth * cellSize / 2f,
            gridHeight * cellSize / 2f,
            0
        );

        return leftBottomCorner + new Vector3(
            gridPos.x * cellSize + cellSize / 2f,
            gridPos.y * cellSize + cellSize / 2f,
            0
        );
    }

    public Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        Vector3 leftBottomCorner = gridCenter - new Vector3(
            gridWidth * cellSize / 2f,
            gridHeight * cellSize / 2f,
            0
        );

        Vector3 localPos = worldPos - leftBottomCorner;
        return new Vector2Int(
            Mathf.FloorToInt(localPos.x / cellSize),
            Mathf.FloorToInt(localPos.y / cellSize)
        );
    }

    public bool IsPositionInGrid(Vector2Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < gridWidth && gridPos.y >= 0 && gridPos.y < gridHeight;
    }

    public bool IsCellOccupied(Vector2Int gridPos)
    {
        if (!IsPositionInGrid(gridPos)) return false;
        return gridCells[gridPos.x, gridPos.y].occupiedUnit != null;
    }

    public BattleUnit GetUnitAt(Vector2Int gridPos)
    {
        if (!IsPositionInGrid(gridPos)) return null;
        return gridCells[gridPos.x, gridPos.y].occupiedUnit;
    }

    public bool PlaceUnit(Vector2Int gridPos, BattleUnit unit)
    {
        if (!IsPositionInGrid(gridPos) || IsCellOccupied(gridPos)) return false;

        gridCells[gridPos.x, gridPos.y].occupiedUnit = unit;
        unit.CurrentGridPosition = gridPos;
        unit.transform.position = GetWorldPosition(gridPos);
        return true;
    }

    public bool MoveUnit(Vector2Int fromPos, Vector2Int toPos)
    {
        if (!IsPositionInGrid(toPos) || IsCellOccupied(toPos)) return false;

        BattleUnit unit = GetUnitAt(fromPos);
        if (unit == null) return false;

        gridCells[fromPos.x, fromPos.y].occupiedUnit = null;
        gridCells[toPos.x, toPos.y].occupiedUnit = unit;
        unit.CurrentGridPosition = toPos;
        unit.transform.position = GetWorldPosition(toPos);
        return true;
    }

    public List<Vector2Int> GetValidMovePositions(Vector2Int startPos, int moveRange)
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();

        for (int x = -moveRange; x <= moveRange; x++)
        {
            for (int y = -moveRange; y <= moveRange; y++)
            {
                Vector2Int checkPos = startPos + new Vector2Int(x, y);
                int distance = Mathf.Abs(x) + Mathf.Abs(y);

                if (distance <= moveRange && IsPositionInGrid(checkPos) && !IsCellOccupied(checkPos))
                {
                    validPositions.Add(checkPos);
                }
            }
        }

        return validPositions;
    }

    public void HighlightCell(Vector2Int gridPos, Color color)
    {
        if (!IsPositionInGrid(gridPos)) return;

        var cellData = gridCells[gridPos.x, gridPos.y];
        if (cellData.cellRenderer != null && highlightMaterial != null)
        {
            Material highlightMat = new Material(highlightMaterial);
            highlightMat.color = color;
            cellData.cellRenderer.material = highlightMat;
        }
    }

    public void ClearHighlights()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                var cellData = gridCells[x, y];
                if (cellData.cellRenderer != null && cellData.originalMaterial != null)
                {
                    cellData.cellRenderer.material = cellData.originalMaterial;
                }
            }
        }
    }

    public void RemoveUnit(Vector2Int gridPos)
    {
        if (!IsPositionInGrid(gridPos)) return;
        gridCells[gridPos.x, gridPos.y].occupiedUnit = null;
    }

    private ZoneType GetZoneForPosition(int x)
    {
        if (x < allyZoneColumns) return ZoneType.Ally;
        if (x >= gridWidth - enemyZoneColumns) return ZoneType.Enemy;
        return ZoneType.Neutral;
    }

    private Material GetMaterialForZone(ZoneType zone)
    {
        switch (zone)
        {
            case ZoneType.Ally: return allyZoneMaterial;
            case ZoneType.Enemy: return enemyZoneMaterial;
            default: return neutralZoneMaterial;
        }
    }

    // Метод для отладки - показывает все позиции клеток
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 pos = GetWorldPosition(new Vector2Int(x, y));
                Gizmos.DrawWireCube(pos, new Vector3(cellSize * 0.9f, cellSize * 0.9f, 0.1f));

                // Показываем координаты
#if UNITY_EDITOR
                UnityEditor.Handles.Label(pos + Vector3.up * 0.2f, $"({x},{y})");
#endif
            }
        }
    }
}