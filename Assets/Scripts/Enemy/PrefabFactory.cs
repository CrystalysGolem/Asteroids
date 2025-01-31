using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;

public class PrefabFactory : IInitializable
{
    private readonly ObjectPoolService _objectPoolService;

    [Inject(Id = "UFOPref")]
    private GameObject UFOPref;

    [Inject(Id = "AsteroidPref")]
    private GameObject AsteroidPref;

    [Inject]
    public PrefabFactory(ObjectPoolService objectPoolService)
    {
        _objectPoolService = objectPoolService;
    }

    public void Initialize()
    {
        Debug.Log("PrefabFactory initialized.");
        _objectPoolService.Initialize();
        _objectPoolService.RegisterPrefab(UFOPref, 10);
        _objectPoolService.RegisterPrefab(AsteroidPref, 20);
    }

    public void InitializeSpawnLoop(GameObject prefab, float spawnInterval, int spawnCount)
    {
        SpawnPrefabLoop(prefab, spawnInterval, spawnCount).Forget();
    }

    private async UniTaskVoid SpawnPrefabLoop(GameObject prefab, float spawnInterval, int spawnCount)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            await UniTask.Delay((int)(spawnInterval * 1000));
            SpawnPrefab(prefab);
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
}