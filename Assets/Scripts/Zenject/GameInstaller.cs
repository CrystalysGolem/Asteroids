using UnityEngine;
using Zenject;
using System.Collections.Generic;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private GameObject ufoPrefab;
    [SerializeField] private GameObject fragmentPrefab;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<DifficultyManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<ScoreManager>().AsSingle();
        Container.Bind<PlayerMove>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerTeleport>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerShoot>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ObjectPoolService>().AsSingle().NonLazy();
        Container.Bind<PrefabFactory>().AsSingle();

        // Передаем префабы при создании SpawnManager
        Container.Bind<SpawnManager>()
            .AsSingle()
            .WithArguments(asteroidPrefab, ufoPrefab, fragmentPrefab);
    }

    public override void Start()
    {
        var spawnManager = Container.Resolve<SpawnManager>();
        spawnManager.Initialize();
    }
}
