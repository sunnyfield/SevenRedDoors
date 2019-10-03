using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScripts.Pool
{
    public enum Group
    {
        VFX_MEAT,
        VFX_BLOOD_EXPLOSION,
        VFX_BOX_CRUSH,
        PROJECTILE,
        VFX_MUZZLE,
        VFX_HIT,
        FIREBALL
    };

    public struct PoolableObject
    {
        public readonly GameObject Prefab;
        public readonly Group Group;
        public readonly int InitialPoolSize;

        public PoolableObject(GameObject prefab, Group @group, int initialPoolSize)
        {
            Prefab = prefab;
            Group = @group;
            InitialPoolSize = initialPoolSize;
        }
    }

    public class Pool : MonoBehaviour
    {
        private static readonly int _groupCount = System.Enum.GetValues(typeof(Group)).Length;
        private static readonly Pool[] _pools = new Pool[_groupCount];

        public static readonly PoolableObject[] ObjectsToPool = new PoolableObject[_groupCount];

        private readonly Queue<GameObject> _objects = new Queue<GameObject>();
    
        private PoolableObject _objectToPool;
    
        private int _iterator;

        private Pool() { }

        public static Pool GetPool(Group group)
        {
            return _pools[(int)group];
        }

        public static void CreatePools()
        {
            for (int i = 0; i < _pools.Length; i++)
            {
                var pool = new GameObject("Pool_" + ObjectsToPool[i].Group.ToString()).AddComponent<Pool>();
                pool._objectToPool = ObjectsToPool[i];

                pool.GrowPool();
                _pools[i] = pool;
            }
        }

        private void GrowPool()
        {
            for(int i = 0; i < _objectToPool.InitialPoolSize; i++)
            {
                AddObject();           
            }
        }

        private void AddObject()
        {
            var pooledObject = Instantiate(_objectToPool.Prefab, transform, true);

            pooledObject.name += "_" + _iterator;
            _objects.Enqueue(pooledObject);
            pooledObject.SetActive(false);
            _iterator++;
        }

        public static GameObject Pull(Group group, bool enable = true)
        {
            var pool = GetPool(group);
            if (pool._objects.Count == 0)
                pool.AddObject();

            var pooledObject = pool._objects.Dequeue();
            pooledObject.SetActive(enable);
            return pooledObject;
        }

        public static GameObject Pull(Group group, Vector3 position, Quaternion rotation, bool enable = true)
        {
            var pool = GetPool(group);
            if (pool._objects.Count == 0)
                pool.AddObject();

            var pooledObject = pool._objects.Dequeue();
            pooledObject.transform.localPosition = position;
            pooledObject.transform.localRotation = rotation;
            pooledObject.SetActive(enable);
            return pooledObject;
        }

        public static GameObject Pull(Group group, Transform parent, Vector3 relativePosition, Quaternion relativeRotation, bool enable = true)
        {
            var pool = GetPool(group);
            if (pool._objects.Count == 0)
                pool.AddObject();

            var pooledObject = pool._objects.Dequeue();
            pooledObject.transform.SetParent(parent);
            pooledObject.transform.localPosition = relativePosition;
            pooledObject.transform.localRotation = relativeRotation;
            pooledObject.SetActive(enable);
            return pooledObject;
        }

        public static GameObject Pull(Group group, Vector3 position, Quaternion rotation, float existTime)
        {
            var pool = GetPool(group);
            if (pool._objects.Count == 0)
                pool.AddObject();

            var pooledObject = pool._objects.Dequeue();
            pooledObject.transform.localPosition = position;
            pooledObject.transform.localRotation = rotation;
            pooledObject.SetActive(true);

            pool.StartCoroutine(pool.DelayedPushCoroutine(pooledObject, existTime));
            return pooledObject;
        }

        public static void Push(Group group, GameObject objectToPool)
        {
            var pool = GetPool(group);
            pool._objects.Enqueue(objectToPool);
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
            _objects.Enqueue(objectToPool);
            objectToPool.SetActive(false);
            objectToPool.transform.SetParent(transform);
        }
    }
}