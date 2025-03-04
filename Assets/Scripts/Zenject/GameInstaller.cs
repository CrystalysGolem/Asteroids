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
        Container.Bind<OptionsManager>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<DifficultyManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<ScoreManager>().AsSingle();
        Container.Bind<PlayerMove>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerTeleport>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerShoot>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerHealth>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ObjectPoolService>().AsSingle().NonLazy();
        Container.Bind<PrefabFactory>().AsSingle();

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
