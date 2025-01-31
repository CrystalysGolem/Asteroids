using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public GameObject UFOPref;
    public GameObject AsteroidPref;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<DifficultyManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<ScoreManager>().AsSingle();
        Container.Bind<PlayerMove>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerTeleport>().FromComponentInHierarchy().AsSingle();

        Container.Bind<GameObject>().WithId("UFOPref").FromInstance(UFOPref);
        Container.Bind<GameObject>().WithId("AsteroidPref").FromInstance(AsteroidPref);

        Container.Bind<PlayerShoot>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ObjectPoolService>().AsSingle().NonLazy();
        Container.Bind<PrefabFactory>().AsSingle();
        Container.Bind<SpawnManager>().AsSingle();
    }

    public override void Start()
    {
        var spawnManager = Container.Resolve<SpawnManager>();
        spawnManager.Initialize();
    }

}
