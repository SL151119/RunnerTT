using UnityEngine;
using VContainer;
using VContainer.Unity;

public class EnergySpawner
{
    private readonly IObjectResolver _resolver;
    private readonly ThemeData _themeData;

    public EnergySpawner(IObjectResolver resolver, ThemeData themeData)
    {
        _resolver = resolver;
        _themeData = themeData;
    }

    public void SpawnEnergy(Vector3 position, Quaternion rotation, Transform parent)
    {
        // Select a random energy prefab from ThemeData
        EnergyDataSO randomEnergyData = _themeData.CollectablePrefabs[Random.Range(0, _themeData.CollectablePrefabs.Length)];

        GameObject energyObject = _resolver.Instantiate(randomEnergyData.Prefab, position, rotation);

        energyObject.transform.SetParent(parent, true);
    }
}
