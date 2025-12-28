// Assets/Scripts/Core/UnitSelectionManager.cs
using UnityEngine;
using System.Collections.Generic;

public class UnitSelectionManager : MonoBehaviour
{
    [System.Serializable]
    public class SelectableUnit
    {
        public GameObject unitPrefab;
        public Sprite unitIcon;
        public string unitName;
        public CharacterClass characterClass;
        public Race race;
        public int tier = 1;
    }

    [Header("Available Units")]
    [SerializeField] private List<SelectableUnit> allUnits = new List<SelectableUnit>();

    [Header("Selected Units")]
    [SerializeField] private List<GameObject> selectedUnits = new List<GameObject>();
    [SerializeField] private int maxSelectedUnits = 8;

    public static UnitSelectionManager Instance { get; private set; }
    public List<GameObject> SelectedUnits => new List<GameObject>(selectedUnits);

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddUnitToSelection(GameObject unitPrefab)
    {
        if (selectedUnits.Count < maxSelectedUnits)
        {
            selectedUnits.Add(unitPrefab);
        }
    }

    public void RemoveUnitFromSelection(GameObject unitPrefab)
    {
        selectedUnits.Remove(unitPrefab);
    }

    public void ClearSelection()
    {
        selectedUnits.Clear();
    }
}