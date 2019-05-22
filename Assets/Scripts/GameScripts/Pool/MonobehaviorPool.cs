using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonobehaviorPool : MonoBehaviour
{
    private static Dictionary<PoolableMonobehavior, MonobehaviorPool> monobehaviorPools = new Dictionary<PoolableMonobehavior, MonobehaviorPool>();

    private Queue<PoolableMonobehavior> monobehaviors = new Queue<PoolableMonobehavior>();

    private PoolableMonobehavior monobehaviorPrefab;

    private int monobehaviorIterator = 0;

    public static MonobehaviorPool GetMonobehaviorPool(PoolableMonobehavior prefab)
    {
        if (monobehaviorPools.ContainsKey(prefab))
            return monobehaviorPools[prefab];

        var pool = new GameObject("MonobehaviorPool_" + (prefab as Component).name).AddComponent<MonobehaviorPool>();
        pool.monobehaviorPrefab = prefab;

        pool.GrowMonobehaviorPool();
        monobehaviorPools.Add(prefab, pool);
        return pool;
    }

    public T GetMonobehavior<T>() where T : PoolableMonobehavior
    {
        if (monobehaviors.Count == 0)
            AddMonobehavior();

        var pooledObject = monobehaviors.Dequeue();

        return pooledObject as T;
    }

    private void GrowMonobehaviorPool()
    {
        for (int i = 0; i < monobehaviorPrefab.InitialPoolSize; i++)
        {
            AddMonobehavior();
        }
    }

    private void AddMonobehaviorToAvaliable(PoolableMonobehavior pooledObject)
    {
        monobehaviors.Enqueue(pooledObject);
        if (!pooledObject.gameObject.activeInHierarchy)
            (pooledObject as Component).transform.SetParent(transform);
    }

    private void AddMonobehavior()
    {
        var pooledObject = Instantiate(monobehaviorPrefab) as PoolableMonobehavior;
        (pooledObject as Component).gameObject.name += "_" + monobehaviorIterator;

        pooledObject.OnDestroyMethod = () => AddMonobehaviorToAvaliable(pooledObject);

        (pooledObject as Component).gameObject.SetActive(false);
        monobehaviorIterator++;
    }
}
