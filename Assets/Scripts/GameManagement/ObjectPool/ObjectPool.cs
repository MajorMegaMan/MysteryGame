using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool<T> where T : PooledObject
{
    Queue<T> m_pooledObjects;

    public delegate T CreateAction(T objectPrefab);
    CreateAction m_createAction = Object.Instantiate;

    public delegate void PoolAction(T pooledObject);
    PoolAction m_destroyAction = (pooledObject) => { Object.Destroy(pooledObject.gameObject); };

    public GameObjectPool(T objectPrefab, int poolCount)
    {
        Init(objectPrefab, poolCount);
    }

    public GameObjectPool(T objectPrefab, int poolCount, CreateAction createAction, PoolAction destroyAction)
    {
        m_createAction = createAction;
        m_destroyAction = destroyAction;

        Init(objectPrefab, poolCount);
    }

    void Init(T objectPrefab, int poolCount)
    {
        m_pooledObjects = new Queue<T>();

        for(int i = 0; i < poolCount; i++)
        {
            CreateGameObject(objectPrefab);
        }
    }

    // Activates the next available poolObject and returns it.
    // Returns null if all pooled objects are already active
    public T ActivateObject()
    {
        // Get Next pooled object in queue.
        T initialPooledObject = m_pooledObjects.Dequeue();
        m_pooledObjects.Enqueue(initialPooledObject);

        // Test if it can be used.
        if(!initialPooledObject.isActiveInPool)
        {
            // Pooled object can be used, set it active and return it.
            initialPooledObject.SetIsActiveInPool(true);
            return initialPooledObject;
        }

        // Initial pooled object cannot be used. continue searching for next available.
        T pooledObject;
        do
        {
            // Get Next pooled object in queue.
            pooledObject = m_pooledObjects.Dequeue();
            m_pooledObjects.Enqueue(pooledObject);

            // Test if it can be used.
            if (!pooledObject.isActiveInPool)
            {
                // Pooled object can be used, set it active and return it.
                pooledObject.SetIsActiveInPool(true);
                return pooledObject;
            }
        }
        while (pooledObject != initialPooledObject);
        // If all objects have been searched, and none are available. return null

        return null;
    }

    // Simply Deactivates target pooled object. Probably unneccesary, but makes it easier to call it from here maybe.
    public void DeactivateObject(T pooledObject)
    {
        pooledObject.SetIsActiveInPool(false);
    }

    void CreateGameObject(T objectPrefab)
    {
        T newObject = m_createAction.Invoke(objectPrefab);
        m_pooledObjects.Enqueue(newObject);
        newObject.SetIsActiveInPool(false);
    }

    void DestroyGameObject(T pooledObject)
    {
        m_destroyAction.Invoke(pooledObject);
    }

    public void ExpandPool(T objectPrefab, int additionalCount)
    {
        for (int i = 0; i < additionalCount; i++)
        {
            CreateGameObject(objectPrefab);
        }
    }

    public void ResizePool(T objectPrefab, int count)
    {
        Queue<T> oldPooledObjects = m_pooledObjects;

        m_pooledObjects = new Queue<T>();

        // Copy old elements
        for (int i = 0; i < count; i++)
        {
            if(oldPooledObjects.Count > 0)
            {
                m_pooledObjects.Enqueue(oldPooledObjects.Dequeue());
            }
            else
            {
                break;
            }
        }

        // If the size is not correct, fill with new objects
        while(m_pooledObjects.Count < count)
        {
            CreateGameObject(objectPrefab);
        }

        // If oldObject still has elements, destroy them
        while (oldPooledObjects.Count > 0)
        {
            DestroyGameObject(oldPooledObjects.Dequeue());
        }
    }
}

public class PooledObject : MonoBehaviour
{
    bool m_isActiveInPool = false;
    public bool isActiveInPool { get { return m_isActiveInPool; } }

    public void SetIsActiveInPool(bool isActive)
    {
        gameObject.SetActive(isActive);
        m_isActiveInPool = isActive;
    }
}
