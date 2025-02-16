using Zenject;
using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : IInitializable
{
    private readonly PrefabFactory _prefabFactory;
    private readonly GameObject _asteroidPrefab;
    private readonly GameObject _ufoPrefab;
    private readonly GameObject _fragmentPrefab;

    [Inject]
    public SpawnManager(PrefabFactory prefabFactory, GameObject asteroidPrefab, GameObject ufoPrefab, GameObject fragmentPrefab)
    {
        _prefabFactory = prefabFactory;
        _asteroidPrefab = asteroidPrefab;
        _ufoPrefab = ufoPrefab;
        _fragmentPrefab = fragmentPrefab;
    }

    public void Initialize()
    {
        _prefabFactory.SetFragmentPrefab(_fragmentPrefab); // Передаём фрагмент в фабрику

        var spawnConfigs = new List<PrefabFactory.SpawnConfig>
        {
            new PrefabFactory.SpawnConfig(_ufoPrefab, 10, 5f, 5),
            new PrefabFactory.SpawnConfig(_asteroidPrefab, 20, 3f, 15)
        };

        _prefabFactory.Initialize(spawnConfigs);
    }
}
