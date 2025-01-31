using Zenject;
using UnityEngine;

public class SpawnManager : IInitializable
{
    [Inject(Id = "UFOPref")] private GameObject UFOPref;
    [Inject(Id = "AsteroidPref")] private GameObject AsteroidPref;
    [Inject] private PrefabFactory _prefabFactory;

    [Inject]
    public void Construct(PrefabFactory prefabFactory)
    {
        _prefabFactory = prefabFactory;
    }

    public void Initialize()
    {
        Debug.Log("SpawnManager initialized.");
        _prefabFactory.Initialize();
        _prefabFactory.InitializeSpawnLoop(UFOPref, 3f, 10);
        _prefabFactory.InitializeSpawnLoop(AsteroidPref, 5f, 5);
    }
}
