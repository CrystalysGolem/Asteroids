using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class PrefabFactory
{
    private readonly ObjectPoolService _objectPoolService;

    [Inject]
    public PrefabFactory(ObjectPoolService objectPoolService)
    {
        _objectPoolService = objectPoolService;
    }

    public void Initialize(List<SpawnConfig> spawnConfigs)
    {
        Debug.Log("PrefabFactory initialized.");
        _objectPoolService.Initialize();

        foreach (var config in spawnConfigs)
        {
            _objectPoolService.RegisterPrefab(config.Prefab, config.PoolSize);
            InitializeSpawnLoop(config);
        }
    }

    private void InitializeSpawnLoop(SpawnConfig config)
    {
        SpawnPrefabLoop(config).Forget();
    }

    private async UniTaskVoid SpawnPrefabLoop(SpawnConfig config)
    {
        for (int i = 0; i < config.SpawnCount; i++)
        {
            await UniTask.Delay((int)(config.SpawnInterval * 1000));
            SpawnPrefab(config.Prefab);
        }
    }

    private void SpawnPrefab(GameObject prefab)
    {
        GameObject spawnedObject = _objectPoolService.GetObject(prefab);
        spawnedObject.transform.position = Vector3.zero;
        spawnedObject.SetActive(true);

        var component = spawnedObject.GetComponent<IEnemy>();
        component?.StartUP();
    }

    public class SpawnConfig
    {
        public GameObject Prefab { get; }
        public int PoolSize { get; }
        public float SpawnInterval { get; }
        public int SpawnCount { get; }

        public SpawnConfig(GameObject prefab, int poolSize, float spawnInterval, int spawnCount)
        {
            Prefab = prefab;
            PoolSize = poolSize;
            SpawnInterval = spawnInterval;
            SpawnCount = spawnCount;
        }
    }
}
