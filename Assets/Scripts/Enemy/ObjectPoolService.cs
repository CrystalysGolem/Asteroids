using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ObjectPoolService : IInitializable
{
    [Inject] private readonly DiContainer _container;

    private readonly Dictionary<GameObject, Queue<GameObject>> _pools;
    private readonly Dictionary<GameObject, int> _poolSizes;

    public ObjectPoolService(DiContainer container)
    {
        _container = container;
        _pools = new Dictionary<GameObject, Queue<GameObject>>();
        _poolSizes = new Dictionary<GameObject, int>();
    }

    public void Initialize()
    {
    }

    public void RegisterPrefab(GameObject prefab, int initialSize)
    {
        if (!_pools.ContainsKey(prefab))
        {
            _pools[prefab] = new Queue<GameObject>();
            _poolSizes[prefab] = initialSize;

            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = CreateNewObject(prefab);
                obj.SetActive(false);
                _pools[prefab].Enqueue(obj);
            }
        }
    }

    public GameObject GetObject(GameObject prefab)
    {
        if (!_pools.ContainsKey(prefab))
        {
            RegisterPrefab(prefab, 1);
        }

        if (_pools[prefab].Count > 0)
        {
            GameObject obj = _pools[prefab].Dequeue();
            obj.SetActive(true);
            return obj;
        }

        GameObject newObj = CreateNewObject(prefab);
        newObj.SetActive(true);
        return newObj;
    }

    public void ReturnObject(GameObject prefab, GameObject obj)
    {
        if (!_pools.ContainsKey(prefab))
        {
            obj.SetActive(false);
            return;
        }

        obj.SetActive(false);
        _pools[prefab].Enqueue(obj);
    }

    private GameObject CreateNewObject(GameObject prefab)
    {
        GameObject obj = _container.InstantiatePrefab(prefab);
        obj.SetActive(false);
        return obj;
    }
}
