using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [Header("For Prefab Fabric")]
     public GameObject asteroidPrefab;
     public GameObject ufoPrefab;
     public GameObject fragmentPrefab;

    public override void InstallBindings()
    {
        Container.Bind<OptionsProvider>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<DifficultyProvider>().AsSingle();
        Container.BindInterfacesAndSelfTo<ScoreProvider>().AsSingle();
        Container.Bind<PlayerMove>().FromComponentInHierarchy().AsSingle();
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
