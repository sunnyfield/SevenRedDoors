using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Group {VFX_Meat, VFX_BloodExplosion, VFX_BoxCrush, Projectile, VFX_Muzzle, VFX_Hit, Fireball};

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
    private static int groupCount = System.Enum.GetValues(typeof(Group)).Length;
    private static Pool[] pools = new Pool[groupCount];

    public static PoolableObject[] objectsToPool = new PoolableObject[groupCount];

    private Queue<GameObject> objects = new Queue<GameObject>();
    
    private PoolableObject objectToPool;
    
    private int iterator = 0;

    private Pool()
    { }

    public static Pool GetPool(Group group)
    {
        return pools[(int)group];
    }

    public static void CreatePools()
    {
        for (int i = 0; i < pools.Length; i++)
        {
            var pool = new GameObject("Pool_" + objectsToPool[i].group.ToString()).AddComponent<Pool>();
            pool.objectToPool = objectsToPool[i];

            pool.GrowPool();
            pools[i] = pool;
        }
    }

    private void GrowPool()
    {
        for(int i = 0; i < objectToPool.initialPoolSize; i++)
        {
            AddObject();           
        }
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
