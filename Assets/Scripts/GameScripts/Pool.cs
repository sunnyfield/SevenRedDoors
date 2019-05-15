using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Group {VFX_Meat, VFX_BloodExplosion, Bullet};

public struct PoolableObject
{
    public GameObject prefab;
    public Group group;
    public int initialPoolSize;

    public PoolableObject(GameObject prefab_, Group group_, int initialPoolSize_)
    {
        prefab = prefab_;
        group = group_;
        initialPoolSize = initialPoolSize_;
    }
}

public class Pool : MonoBehaviour
{
    private static Dictionary<Group, Pool> pools = new Dictionary<Group, Pool>();
    private static Dictionary<PoolableMonobehavior, Pool> monobehaviorPools = new Dictionary<PoolableMonobehavior, Pool>(); 

    public static Dictionary<Group, PoolableObject> objectsToPool = new Dictionary<Group, PoolableObject>();

    private Queue<GameObject> objects = new Queue<GameObject>();
    private Queue<PoolableMonobehavior> monobehaviors = new Queue<PoolableMonobehavior>();

    private PoolableObject objectToPool;
    private PoolableMonobehavior monobehaviorPrefab;

    private int iterator = 0;
    private int monobehaviorIterator = 0;

    private Pool()
    { }

    //public static Pool CreatePool(PoolableObject poolableObject)
    //{
    //    var pool = new Pool();
    //    objectsToPool.Add(poolableObject.group, poolableObject);
    //    pool = GetPool(poolableObject.group);
    //    return pool;
    //}

    public static Pool GetPool(Group group)
    {
        if (pools.ContainsKey(group))
            return pools[group];

        var pool = new GameObject("Pool_" + group.ToString()).AddComponent<Pool>();
        pool.objectToPool = objectsToPool[group];

        pool.GrowPool();
        pools.Add(group, pool);
        return pool;
    }

    public static Pool GetMonobehaviorPool(PoolableMonobehavior prefab)
    {
        if (monobehaviorPools.ContainsKey(prefab))
            return monobehaviorPools[prefab];

        var pool = new GameObject("MonobehaviorPool_" + (prefab as Component).name).AddComponent<Pool>();
        pool.monobehaviorPrefab = prefab;

        pool.GrowMonobehaviorPool();
        monobehaviorPools.Add(prefab, pool);
        return pool;
    }

    public T GetMonobehavior<T>() where T : PoolableMonobehavior
    {
        if (monobehaviors.Count == 0)
            AddPoolable();

        var pooledObject = monobehaviors.Dequeue();

        return pooledObject as T;
    }

    private void GrowPool()
    {
        for(int i = 0; i < objectToPool.initialPoolSize; i++)
        {
            AddObject();           
        }
    }

<<<<<<< HEAD
    private void GrowMonobehaviorPool()
    {
        for(int i = 0; i < monobehaviorPrefab.InitialPoolSize; i++)
        {
            AddPoolable();
=======
            pooledObject.name += "_" + iterator;
            objects.Enqueue(pooledObject);
            pooledObject.SetActive(false);
            pooledObject.transform.SetParent(transform);           
>>>>>>> master
        }
    }

    private void AddMonobehaviorToAvaliable(PoolableMonobehavior pooledObject)
    {
        monobehaviors.Enqueue(pooledObject);
        if (!pooledObject.gameObject.activeInHierarchy)
            (pooledObject as Component).transform.SetParent(transform);
    }

    private void AddPoolable()
    {
        var pooledObject = Instantiate(monobehaviorPrefab) as PoolableMonobehavior;
        (pooledObject as Component).gameObject.name += "_" + monobehaviorIterator;

        pooledObject.OnDestroyMethod = () => AddMonobehaviorToAvaliable(pooledObject);

        (pooledObject as Component).gameObject.SetActive(false);
        monobehaviorIterator++;
    }

    private void AddObject()
    {
        var pooledObject = Instantiate(objectToPool.prefab);

        pooledObject.name += "_" + iterator;
        objects.Enqueue(pooledObject);
        pooledObject.SetActive(false);
        pooledObject.transform.SetParent(transform);
        iterator++;
    }

    public static GameObject Pull(Group group, bool enable = true)
    {
        var pool = GetPool(group);
        if (pool.objects.Count == 0)
            pool.AddObject();

        var pooledObject = pool.objects.Dequeue();
        pooledObject.SetActive(enable);
        return pooledObject;
    }

    public static GameObject Pull(Group group, Vector3 position, Quaternion rotation, bool enable = true)
    {
        var pool = GetPool(group);
        if (pool.objects.Count == 0)
            pool.AddObject();

        var pooledObject = pool.objects.Dequeue();
        pooledObject.transform.localPosition = position;
        pooledObject.transform.localRotation = rotation;
        pooledObject.SetActive(enable);
        return pooledObject;
    }

    public static GameObject Pull(Group group, Transform parent, Vector3 relativePosition, Quaternion relativeRotation, bool enable = true)
    {
        var pool = GetPool(group);
        if (pool.objects.Count == 0)
            pool.AddObject();

        var pooledObject = pool.objects.Dequeue();
        pooledObject.transform.SetParent(parent);
        pooledObject.transform.localPosition = relativePosition;
        pooledObject.transform.localRotation = relativeRotation;
        pooledObject.SetActive(enable);
        return pooledObject;
    }

    public static GameObject Pull(Group group, Vector3 position, Quaternion rotation, float existTime)
    {
        var pool = GetPool(group);
        if (pool.objects.Count == 0)
            pool.AddObject();

        var pooledObject = pool.objects.Dequeue();
        pooledObject.transform.localPosition = position;
        pooledObject.transform.localRotation = rotation;
        pooledObject.SetActive(true);

        pool.StartCoroutine(pool.DelayedPushCoroutine(pooledObject, existTime));
        return pooledObject;
    }

    public static void Push(Group group, GameObject objectToPool)
    {
        var pool = GetPool(group);
        pool.objects.Enqueue(objectToPool);
        objectToPool.SetActive(false);
        objectToPool.transform.SetParent(pool.transform);
    }

    public static void Push(Group group, GameObject objectToPool, float delay)
    {
        var pool = GetPool(group);
        pool.StartCoroutine(pool.DelayedPushCoroutine(objectToPool, delay));
    }

    IEnumerator DelayedPushCoroutine(GameObject objectToPool, float delay)
    {
        yield return new WaitForSeconds(delay);
        objects.Enqueue(objectToPool);
        objectToPool.SetActive(false);
        objectToPool.transform.SetParent(transform);
    }
}
