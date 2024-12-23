using UnityEngine;

[CreateAssetMenu(fileName = "EnergyData", menuName = "Collectables/EnergyData")]
public class EnergyDataSO : ScriptableObject
{
    [field: SerializeField] public EnergyType EnergyType { get; set; }
    [field: SerializeField] public int Points { get; set; }
    [field: SerializeField] public GameObject Prefab { get; set; }
}