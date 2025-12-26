using UnityEngine;

public class GridCellClickHandler : MonoBehaviour
{
    private Vector2Int gridPosition;

    public void Initialize(Vector2Int position)
    {
        gridPosition = position;
    }

    void OnMouseDown()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnCellClicked(gridPosition);
        }
    }
}