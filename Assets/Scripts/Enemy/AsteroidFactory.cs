using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;

public class AsteroidFactory : IInitializable
{
    private readonly GameObject _asteroidPrefab;
    private readonly DiContainer _diContainer;
    private readonly PlayerTeleport _playerTeleport;
    private readonly PlayerMove _playerMove;

    [Inject]
    public AsteroidFactory(GameObject asteroidPrefab, DiContainer diContainer, PlayerTeleport playerTeleport, PlayerMove playerMove)
    {
        _asteroidPrefab = asteroidPrefab;
        _diContainer = diContainer;
        _playerTeleport = playerTeleport;
        _playerMove = playerMove;
    }

    public void Initialize()
    {
        Debug.Log("AsteroidFactory initialized and spawning loop started.");
        SpawnAsteroidsLoop().Forget();
    }

    private async UniTaskVoid SpawnAsteroidsLoop()
    {
        while (true)
        {
            await UniTask.Delay(100);
            SpawnAsteroid();
        }
    }

    private void SpawnAsteroid()
    {
        GameObject asteroid = _diContainer.InstantiatePrefab(_asteroidPrefab);
        asteroid.transform.position = Vector3.zero;
        asteroid.SetActive(true);

        var asteroidComponent = asteroid.GetComponent<Asteroid>();
        asteroidComponent.StartUP();
    }
}
