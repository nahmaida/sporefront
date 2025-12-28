// Assets/Scripts/Core/BattleManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BattleManager : MonoBehaviour
{


    [Header("Battle Settings")]
    [SerializeField] private int maxRounds = 30;
    [SerializeField] private int currentRound = 1;
    [SerializeField] private float turnDelay = 1f;

    [Header("Teams")]
    [SerializeField] private List<BattleUnit> playerUnits = new List<BattleUnit>();
    [SerializeField] private List<BattleUnit> enemyUnits = new List<BattleUnit>();

    [Header("Economy")]
    [SerializeField] private int playerGold = 0;
    [SerializeField] private int goldPerRound = 10;

    [Header("UI References")]
    [SerializeField] private BattleUI battleUI;
    [SerializeField] private ShopUI shopUI;

    private BattleState currentState = BattleState.Preparation;
    private ActionMode currentActionMode = ActionMode.None;
    private BattleUnit selectedUnit;
    private List<Vector2Int> validMovePositions = new List<Vector2Int>();
    private List<BattleUnit> validAttackTargets = new List<BattleUnit>();

    public static BattleManager Instance { get; private set; }
    public int PlayerGold => playerGold;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        InitializeBattle();
    }

    void InitializeBattle()
    {
        AddGold(100);
        StartPreparationPhase();
    }

    void StartPreparationPhase()
    {
        currentState = BattleState.Preparation;

        if (battleUI != null)
        {
            battleUI.ShowPreparationUI();
            battleUI.UpdateRound(currentRound);
            battleUI.UpdateGold(playerGold);
        }

        if (shopUI != null)
        {
            shopUI.OpenShop();
        }

        AddGold(goldPerRound);
    }

    public void EndPreparationPhase()
    {
        if (shopUI != null) shopUI.CloseShop();
        StartBattlePhase();
    }

    void StartBattlePhase()
    {
        currentState = BattleState.Battle;
        if (battleUI != null) battleUI.ShowBattleUI();
        StartNextTurn();
    }

    void StartNextTurn()
    {
        if (currentState != BattleState.Battle) return;

        if (CheckBattleEnd())
        {
            EndBattle();
            return;
        }

        BattleUnit nextUnit = GetNextActiveUnit();
        if (nextUnit != null)
        {
            StartUnitTurn(nextUnit);
        }
    }

    BattleUnit GetNextActiveUnit()
    {
        var allUnits = new List<BattleUnit>();
        allUnits.AddRange(playerUnits);
        allUnits.AddRange(enemyUnits);

        return allUnits
            .Where(unit => unit != null && unit.IsAlive() && !unit.HasActed())
            .OrderByDescending(unit => unit.GetSpeed())
            .FirstOrDefault();
    }

    void StartUnitTurn(BattleUnit unit)
    {
        selectedUnit = unit;
        unit.StartTurn();

        if (unit.GetTeamId() == 0)
        {
            EnablePlayerControl(unit);
        }
        else
        {
            StartEnemyAI(unit);
        }
    }

    void EnablePlayerControl(BattleUnit unit)
    {
        if (battleUI != null) battleUI.SelectUnit(unit);
        ShowAvailableActions(unit);
    }

    void ShowAvailableActions(BattleUnit unit)
    {
        ClearHighlights();

        validMovePositions = BattleGridManager.Instance.GetValidMovePositions(
            unit.CurrentGridPosition,
            unit.GetUnitData().moveRange
        );

        foreach (var pos in validMovePositions)
        {
            BattleGridManager.Instance.HighlightCell(pos, new Color(0, 1, 0, 0.3f));
        }

        FindAttackTargets(unit);
    }

    void FindAttackTargets(BattleUnit unit)
    {
        validAttackTargets.Clear();
        int searchRange = Mathf.CeilToInt(unit.GetUnitData().attackRange);

        for (int x = -searchRange; x <= searchRange; x++)
        {
            for (int y = -searchRange; y <= searchRange; y++)
            {
                Vector2Int checkPos = unit.CurrentGridPosition + new Vector2Int(x, y);
                if (BattleGridManager.Instance.IsCellOccupied(checkPos))
                {
                    BattleUnit target = BattleGridManager.Instance.GetUnitAt(checkPos);
                    if (target != null && target.GetTeamId() != unit.GetTeamId() && target.IsAlive())
                    {
                        float distance = Vector2Int.Distance(unit.CurrentGridPosition, checkPos);
                        if (distance <= unit.GetUnitData().attackRange)
                        {
                            validAttackTargets.Add(target);
                            BattleGridManager.Instance.HighlightCell(checkPos, new Color(1, 0, 0, 0.3f));
                        }
                    }
                }
            }
        }
    }

    public void OnCellClicked(Vector2Int gridPos)
    {
        if (currentState != BattleState.Battle || selectedUnit == null) return;

        if (currentActionMode == ActionMode.Move && validMovePositions.Contains(gridPos))
        {
            if (BattleGridManager.Instance.MoveUnit(selectedUnit.CurrentGridPosition, gridPos))
            {
                selectedUnit.MoveTo(gridPos);
                ClearHighlights();
                EndUnitTurn(selectedUnit);
            }
        }
        else if (currentActionMode == ActionMode.Attack)
        {
            BattleUnit target = validAttackTargets.FirstOrDefault(t => t.CurrentGridPosition == gridPos);
            if (target != null)
            {
                selectedUnit.Attack(target);
                ClearHighlights();
                EndUnitTurn(selectedUnit);
            }
        }
    }

    void ClearHighlights()
    {
        BattleGridManager.Instance.ClearHighlights();
        validMovePositions.Clear();
        validAttackTargets.Clear();
        currentActionMode = ActionMode.None;
    }

    void StartEnemyAI(BattleUnit enemy)
    {
        StartCoroutine(EnemyTurnCoroutine(enemy));
    }

    System.Collections.IEnumerator EnemyTurnCoroutine(BattleUnit enemy)
    {
        yield return new WaitForSeconds(0.5f);

        BattleUnit closestPlayer = FindClosestPlayerUnit(enemy);

        if (closestPlayer != null)
        {
            float distance = Vector2Int.Distance(enemy.CurrentGridPosition, closestPlayer.CurrentGridPosition);

            if (distance <= enemy.GetUnitData().attackRange)
            {
                enemy.Attack(closestPlayer);
            }
            else
            {
                Vector2Int moveDirection = FindMoveDirection(enemy, closestPlayer);
                Vector2Int targetPos = enemy.CurrentGridPosition + moveDirection;

                if (BattleGridManager.Instance.IsPositionInGrid(targetPos) &&
                    !BattleGridManager.Instance.IsCellOccupied(targetPos))
                {
                    BattleGridManager.Instance.MoveUnit(enemy.CurrentGridPosition, targetPos);
                    enemy.MoveTo(targetPos);
                }
            }
        }

        yield return new WaitForSeconds(turnDelay);
        EndUnitTurn(enemy);
    }

    Vector2Int FindMoveDirection(BattleUnit enemy, BattleUnit target)
    {
        Vector2Int direction = target.CurrentGridPosition - enemy.CurrentGridPosition;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return new Vector2Int(direction.x > 0 ? 1 : -1, 0);
        }
        else
        {
            return new Vector2Int(0, direction.y > 0 ? 1 : -1);
        }
    }

    BattleUnit FindClosestPlayerUnit(BattleUnit enemy)
    {
        BattleUnit closest = null;
        float minDistance = float.MaxValue;

        foreach (var playerUnit in playerUnits)
        {
            if (playerUnit == null || !playerUnit.IsAlive()) continue;

            float distance = Vector2Int.Distance(enemy.CurrentGridPosition, playerUnit.CurrentGridPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = playerUnit;
            }
        }

        return closest;
    }

    void EndUnitTurn(BattleUnit unit)
    {
        unit.EndTurn();
        selectedUnit = null;
        StartCoroutine(DelayNextTurn());
    }

    System.Collections.IEnumerator DelayNextTurn()
    {
        yield return new WaitForSeconds(0.5f);
        StartNextTurn();
    }

    public void OnUnitDied(BattleUnit unit)
    {
        if (unit.GetTeamId() == 0)
        {
            playerUnits.Remove(unit);
        }
        else
        {
            enemyUnits.Remove(unit);
            AddGold(5);
        }
    }

    bool CheckBattleEnd()
    {
        if (playerUnits.Count == 0 || playerUnits.All(unit => unit == null || !unit.IsAlive()))
        {
            Debug.Log("Player loses!");
            return true;
        }

        if (enemyUnits.Count == 0 || enemyUnits.All(unit => unit == null || !unit.IsAlive()))
        {
            Debug.Log("Player wins!");
            AddGold(goldPerRound * 2);
            return true;
        }

        return false;
    }

    void EndBattle()
    {
        currentState = BattleState.Ended;
        bool playerWon = playerUnits.Any(unit => unit != null && unit.IsAlive());

        if (battleUI != null) battleUI.ShowResults(playerWon, currentRound);

        if (currentRound < maxRounds && playerWon)
        {
            currentRound++;
            Invoke("StartPreparationPhase", 3f);
        }
    }

    public void AddGold(int amount)
    {
        playerGold += amount;
        if (battleUI != null) battleUI.UpdateGold(playerGold);
    }

    public bool SpendGold(int amount)
    {
        if (playerGold >= amount)
        {
            playerGold -= amount;
            if (battleUI != null) battleUI.UpdateGold(playerGold);
            return true;
        }
        return false;
    }

    public void SetActionMode(ActionMode mode)
    {
        currentActionMode = mode;
        if (selectedUnit != null)
        {
            ClearHighlights();
            if (mode == ActionMode.Move) ShowAvailableActions(selectedUnit);
            else if (mode == ActionMode.Attack) FindAttackTargets(selectedUnit);
        }
    }

    public void SkipTurn()
    {
        if (selectedUnit != null) EndUnitTurn(selectedUnit);
    }

    public void AddPlayerUnit(BattleUnit unit)
    {
        if (!playerUnits.Contains(unit))
        {
            playerUnits.Add(unit);
            unit.OnUnitDied += OnUnitDied;
        }
    }

    public void SpawnStartingUnits(List<GameObject> unitPrefabs)
    {
        int spawnIndex = 0;
        for (int x = 0; x < 4 && spawnIndex < unitPrefabs.Count; x++)
        {
            for (int y = 0; y < 5 && spawnIndex < unitPrefabs.Count; y++)
            {
                Vector2Int spawnPos = new Vector2Int(x, y);
                GameObject unitObj = Instantiate(unitPrefabs[spawnIndex]);
                BattleUnit unit = unitObj.GetComponent<BattleUnit>();

                if (unit != null)
                {
                    BattleGridManager.Instance.PlaceUnit(spawnPos, unit);
                    AddPlayerUnit(unit);
                }
                spawnIndex++;
            }
        }

        SpawnEnemyUnits();
    }

    void SpawnEnemyUnits()
    {
        var enemyDataList = Resources.LoadAll<UnitDataSO>("EnemyUnits");
        int enemyStartX = BattleGridManager.Instance.GridWidth - 4;

        for (int x = enemyStartX; x < BattleGridManager.Instance.GridWidth; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                if (enemyDataList.Length == 0) continue;

                UnitDataSO randomData = enemyDataList[Random.Range(0, enemyDataList.Length)];
                Vector2Int spawnPos = new Vector2Int(x, y);

                GameObject enemyObj = Instantiate(randomData.unitPrefab);
                BattleUnit enemyUnit = enemyObj.GetComponent<BattleUnit>();

                if (enemyUnit != null)
                {
                    enemyUnit.Initialize(randomData, Random.Range(1, 4), 1);
                    BattleGridManager.Instance.PlaceUnit(spawnPos, enemyUnit);
                    enemyUnits.Add(enemyUnit);
                    enemyUnit.OnUnitDied += OnUnitDied;
                }
            }
        }
    }
}

public enum BattleState
{
    Preparation,
    Battle,
    Ended
}

public enum ActionMode
{
    None,
    Move,
    Attack,
    Ultimate
}