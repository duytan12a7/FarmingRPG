using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonMonobehaviour<PoolManager>
{
    private Dictionary<int, Queue<GameObject>> poolDictionary = new();
    [SerializeField] private Pool[] pools = null;
    [SerializeField] private Transform objectPoolTransform = null;

    [System.Serializable]
    public struct Pool
    {
        public int poolSize;
        public GameObject prefab;
    }

    private void Start()
    {
        foreach (var pool in pools)
        {
            CreatePool(pool.prefab, Mathf.Max(pool.poolSize, 1));
        }
    }

    private void CreatePool(GameObject prefab, int poolSize)
    {
        int poolKey = prefab.GetInstanceID();

        GameObject parentGameObject = new($"{prefab.name}Pool");
        parentGameObject.transform.SetParent(objectPoolTransform);

        if (!poolDictionary.ContainsKey(poolKey))
        {
            Queue<GameObject> objectPool = new();
            poolDictionary.Add(poolKey, objectPool);

            for (int i = 0; i < poolSize; i++)
            {
                GameObject newObject = Instantiate(prefab, parentGameObject.transform);
                newObject.name = prefab.name;
                newObject.SetActive(false);
                objectPool.Enqueue(newObject);
            }
        }
    }

    public GameObject ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();

        if (poolDictionary.TryGetValue(poolKey, out Queue<GameObject> objectPool))
        {
            GameObject objectToReuse = GetObjectFromPool(objectPool, prefab);
            if (objectToReuse != null)
            {
                ResetObject(position, rotation, objectToReuse, prefab);
                objectToReuse.SetActive(true);
                return objectToReuse;
            }
        }

        Debug.LogWarning($"No object pool for {prefab.name}");
        return null;
    }

    private GameObject GetObjectFromPool(Queue<GameObject> objectPool, GameObject prefab)
    {
        if (objectPool.Count > 0)
        {
            for (int i = 0; i < objectPool.Count; i++)
            {
                GameObject obj = objectPool.Dequeue();
                if (!obj.activeSelf)
                {
                    objectPool.Enqueue(obj);
                    return obj;
                }
                objectPool.Enqueue(obj);
            }
        }

        return CreateNewObject(prefab, objectPool, transform.Find($"{prefab.name}Pool"));
    }



    private GameObject CreateNewObject(GameObject prefab, Queue<GameObject> objectPool, Transform parent)
    {
        GameObject newObject = Instantiate(prefab, parent);
        newObject.name = prefab.name;
        newObject.SetActive(false);
        objectPool.Enqueue(newObject);
        return newObject;
    }

    private static void ResetObject(Vector3 position, Quaternion rotation, GameObject objectToReuse, GameObject prefab)
    {
        objectToReuse.transform.SetPositionAndRotation(position, rotation);

        objectToReuse.transform.localScale = prefab.transform.localScale;
    }
}