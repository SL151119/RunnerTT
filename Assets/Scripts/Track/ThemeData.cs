using UnityEngine;

[System.Serializable]
public struct ThemeZone
{
    public int length;
    public GameObject[] prefabList;
}

[CreateAssetMenu(fileName = "ThemeData", menuName = "Runner/Theme Data")]
public class ThemeData : ScriptableObject
{
    [Header("Theme Data")]
    [SerializeField] private string _themeName;

    [field: SerializeField] public ThemeZone[] Zones { get; set; }
    [field: SerializeField] public EnergyDataSO[] CollectablePrefabs { get; set; }
}
