using Zenject;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class SpawnManager : IInitializable
{
    private readonly PrefabFactory _prefabFactory;
    private readonly GameObject _asteroidPrefab;
    private readonly GameObject _ufoPrefab;

    [Inject]
    public SpawnManager(PrefabFactory prefabFactory, GameObject asteroidPrefab, GameObject ufoPrefab)
    {
        _prefabFactory = prefabFactory;
        _asteroidPrefab = asteroidPrefab;
        _ufoPrefab = ufoPrefab;
    }

    public void Initialize()
    {
        Debug.Log("SpawnManager initialized.");

        var spawnConfigs = new List<PrefabFactory.SpawnConfig>
        {
            new PrefabFactory.SpawnConfig(_ufoPrefab, 10, 3f, 10),  // 10 UFO каждые 3 секунды
            new PrefabFactory.SpawnConfig(_asteroidPrefab, 20, 5f, 5)   // 5 астероидов каждые 5 секунд
        };

        _prefabFactory.Initialize(spawnConfigs);
    }
}
