using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public GameObject UFOPref;
    public GameObject AsteroidPref;

    public override void InstallBindings()
    {
        Container.Bind<DifficultyManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ScoreManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerMove>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerShoot>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerTeleport>().FromComponentInHierarchy().AsSingle();

        Container.BindFactory<UFO, UFO.Factory>().FromComponentInHierarchy();
        Container.BindFactory<Asteroid, Asteroid.Factory>().FromComponentInHierarchy();
        Container.Bind<EnemySpawner>().AsSingle();

    }
}
