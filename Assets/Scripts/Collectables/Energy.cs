using UnityEngine;
using VContainer;

public enum EnergyType
{
    Red,
    Green,
    Purple
}

public class Energy : MonoBehaviour, ICollectable
{
    [SerializeField] private EnergyDataSO _energyData;
    private EnergyUIManager _energyUIManager;

    public EnergyType Type => _energyData.EnergyType;
    public int Points => _energyData.Points;

    [Inject]
    public void Initialize(EnergyUIManager energyUIManager)
    {
        _energyUIManager = energyUIManager;
    }

    public void Collect()
    {
        if (_energyUIManager == null)
        {
            Debug.LogError($"EnergyUIManager is null! Make sure Energy is resolved through VContainer.");
            return;
        }

        _energyUIManager.AddEnergyScore(Type, Points);

        Destroy(gameObject);
    }
}
