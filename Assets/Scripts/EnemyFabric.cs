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

            // ������� ������ ����� �������
            for (int j = 0; j < maxEnemiesPerPrefab[i]; j++)
            {
                IEnemy enemy;
                if (i == 0)  // ��� UFO
                {
                    enemy = _ufoFactory.Create();  // ������� UFO ����� �������
                }
                else  // ��� Asteroid
                {
                    enemy = _asteroidFactory.Create();  // ������� Asteroid ����� �������
                }

                // �������� ����� � ��������� � ���������
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
            await UniTask.Delay((int)(spawnTime * 1000));  // ������� � ���������� spawnTime
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
                    return;  // ����� ��������� ������ ����� ������� �� ������
                }
            }
        }
    }
}
