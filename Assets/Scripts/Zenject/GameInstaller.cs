using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
     public GameObject asteroidPrefab;
     public GameObject ufoPrefab;
     public GameObject fragmentPrefab;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<AsteroidConfigLoader>().AsSingle();
        Container.BindInterfacesAndSelfTo<AsteroidFragmentConfigLoader>().AsSingle();
        Container.BindInterfacesAndSelfTo<UFOConfigLoader>().AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerConfigLoader>().AsSingle();
        Container.BindInterfacesAndSelfTo<ScoreProviderConfigLoader>().AsSingle();

        Container.Bind<OptionsProvider>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<DifficultyProvider>().AsSingle();
        Container.BindInterfacesAndSelfTo<ScoreProvider>().AsSingle();
        Container.Bind<PlayerMove>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerShoot>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerHealth>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ObjectPoolService>().AsSingle().NonLazy();
        Container.Bind<PrefabFactory>().AsSingle();

        Container.Bind<SpawnProvider>()
            .AsSingle()
            .WithArguments(asteroidPrefab, ufoPrefab, fragmentPrefab);
    }

    public override void Start()
    {
        var spawnManager = Container.Resolve<SpawnProvider>();
        spawnManager.Initialize();
    }
}
