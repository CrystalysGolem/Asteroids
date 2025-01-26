using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

public class EnemySpawner : MonoBehaviour
{
    [Header("Max Enemies Per Prefab")]
    [SerializeField] private List<int> maxEnemiesPerPrefab;

    private List<List<IEnemy>> enemyStorages = new List<List<IEnemy>>();
    private float spawnTime;

    [Inject] private DifficultyManager difficultyManager;

    readonly UFO.Factory _ufoFactory;
    readonly Asteroid.Factory _asteroidFactory;

    public EnemySpawner(UFO.Factory ufoFactory, Asteroid.Factory asteroidFactory)
    {
        _ufoFactory = ufoFactory;
        _asteroidFactory = asteroidFactory;
    }

    private void Start()
    {
        SetSpawnTimeByDifficulty();
        InitializeEnemyStorages();
        StartSpawningEnemies().Forget();
    }

    private void SetSpawnTimeByDifficulty()
    {
        switch (difficultyManager?.CurrentDifficulty ?? DifficultyManager.Difficulty.Easy)
        {
            case DifficultyManager.Difficulty.Easy:
                spawnTime = 3f;
                break;
            case DifficultyManager.Difficulty.Medium:
                spawnTime = 2f;
                break;
            case DifficultyManager.Difficulty.Hard:
                spawnTime = 1f;
                break;
            default:
                spawnTime = 3f;
                break;
        }
    }

    private void InitializeEnemyStorages()
    {
        for (int i = 0; i < maxEnemiesPerPrefab.Count; i++)
        {
            List<IEnemy> storage = new List<IEnemy>();
            string storageName = i == 0 ? "UFOStorage" : "AsteroidStorage";

            GameObject storageObject = new GameObject(storageName);
            storageObject.transform.SetParent(transform);

            // Создаем врагов через фабрики
            for (int j = 0; j < maxEnemiesPerPrefab[i]; j++)
            {
                IEnemy enemy;
                if (i == 0)  // Для UFO
                {
                    enemy = _ufoFactory.Create();  // Создаем UFO через фабрику
                }
                else  // Для Asteroid
                {
                    enemy = _asteroidFactory.Create();  // Создаем Asteroid через фабрику
                }

                // Включаем врага и добавляем в хранилище
                enemy.gameObject.SetActive(false);
                storage.Add(enemy);
            }

            enemyStorages.Add(storage);
        }
    }

    private async UniTaskVoid StartSpawningEnemies()
    {
        while (this != null && gameObject.activeSelf)
        {
            SpawnEnemyIfAvailable();
            await UniTask.Delay((int)(spawnTime * 1000));  // Спавним с интервалом spawnTime
        }
    }

    private void SpawnEnemyIfAvailable()
    {
        foreach (var storage in enemyStorages)
        {
            foreach (var enemy in storage)
            {
                if (!enemy.gameObject.activeSelf)
                {
                    enemy.gameObject.SetActive(true);
                    enemy.StartUP();
                    return;  // После активации одного врага выходим из метода
                }
            }
        }
    }
}
