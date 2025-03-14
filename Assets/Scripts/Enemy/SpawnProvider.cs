using Zenject;
using UnityEngine;
using System.Collections.Generic;
using Firebase.Analytics;

public class SpawnProvider : IInitializable
{
    private readonly PrefabFactory _prefabFactory;
    private readonly GameObject _asteroidPrefab;
    private readonly GameObject _ufoPrefab;
    private readonly GameObject _fragmentPrefab;

    private int _asteroidPrefab_poolsize = 20;
    private float _asteroidPrefab_spawninterval = 3f;
    private int _asteroidPrefab_spawncount = 15;

    private int _ufoPrefab_poolsize = 10;
    private float _ufoPrefab_spawninterval = 5f;
    private int _ufoPrefab_spawncount = 5;

    public SpawnProvider(PrefabFactory prefabFactory, GameObject asteroidPrefab, GameObject ufoPrefab, GameObject fragmentPrefab)
    {
        _prefabFactory = prefabFactory;
        _asteroidPrefab = asteroidPrefab;
        _ufoPrefab = ufoPrefab;
        _fragmentPrefab = fragmentPrefab;
    }

    public void Initialize()
    {
        _prefabFactory.SetFragmentPrefab(_fragmentPrefab);

        var spawnConfigs = new List<PrefabFactory.SpawnConfig>
        {
            new PrefabFactory.SpawnConfig(_ufoPrefab, _ufoPrefab_poolsize, _ufoPrefab_spawninterval, _ufoPrefab_spawncount),
            new PrefabFactory.SpawnConfig(_asteroidPrefab, _asteroidPrefab_poolsize, _asteroidPrefab_spawninterval, _asteroidPrefab_spawncount)
        };

        _prefabFactory.Initialize(spawnConfigs);
    }
}
