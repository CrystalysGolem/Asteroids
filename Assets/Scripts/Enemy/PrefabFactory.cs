using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class PrefabFactory
{
    private readonly ObjectPoolService _objectPoolService;
    private GameObject _fragmentPrefab;
    private int MillisecondsInSecond = 1000;

    [Inject]
    public PrefabFactory(ObjectPoolService objectPoolService)
    {
        _objectPoolService = objectPoolService;
    }

    public void Initialize(List<SpawnConfig> spawnConfigs)
    {
        _objectPoolService.Initialize();

        foreach (var config in spawnConfigs)
        {
            _objectPoolService.RegisterPrefab(config.Prefab, config.PoolSize);
            InitializeSpawnLoop(config);
        }
    }

    public void SetFragmentPrefab(GameObject fragmentPrefab)
    {
        _fragmentPrefab = fragmentPrefab;
    }

    private async void InitializeSpawnLoop(SpawnConfig config)
    {
        await UniTask.Delay(MillisecondsInSecond);
        SpawnPrefabLoop(config).Forget();
    }

    private async UniTaskVoid SpawnPrefabLoop(SpawnConfig config)
    {
        for (int i = 0; i < config.SpawnCount; i++)
        {
            await UniTask.Delay((int)(config.SpawnInterval * MillisecondsInSecond));
            SpawnPrefab(config.Prefab);
        }
    }

    public void SpawnPrefabInstantly(GameObject prefab, Vector3 position)
    {
        SpawnPrefab(prefab, position);
    }

    private void SpawnPrefab(GameObject prefab)
    {
        GameObject spawnedObject = _objectPoolService.GetObject(prefab);
        spawnedObject.SetActive(true);

        if (spawnedObject.TryGetComponent(out IEnemy enemyComponent))
        {
            if (spawnedObject.TryGetComponent(out Asteroid asteroid))
            {
                asteroid.SetFragmentPrefab(_fragmentPrefab);
            }
            enemyComponent.StartUP();
        }
    }

    private void SpawnPrefab(GameObject prefab, Vector3 position)
    {
        GameObject spawnedObject = _objectPoolService.GetObject(prefab);
        spawnedObject.SetActive(true);
        spawnedObject.transform.position = position;
        if (spawnedObject.TryGetComponent(out IEnemy enemyComponent))
        {
            enemyComponent.StartUP();
        }
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
