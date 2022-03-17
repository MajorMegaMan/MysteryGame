using System.Collections;
using System.Collections.Generic;

public class GameObjectPool<T> where T : IPooledObject
{
    Queue<PooledObject> m_pooledObjects;

    public delegate T CreateAction(T objectPrefab);
    CreateAction m_createAction = null;

    public delegate void PoolAction(T pooledObject);
    PoolAction m_destroyAction = null;

    // PooledObject is the internal class used by the objectPool to manage the state of whether it is active in the pool or not.
    class PooledObject
    {
        T m_target = default;
        bool m_isActiveInPool = false;
        public bool isActiveInPool { get { return m_isActiveInPool; } }
        public T target { get { return m_target; } }

        public PooledObject(T target)
        {
            m_target = target;
        }

        public virtual void SetIsActiveInPool(bool isActive)
        {
            m_isActiveInPool = isActive;
            m_target.SetIsActiveInPool(isActive);
        }
    }

    public GameObjectPool(T objectPrefab, int poolCount, CreateAction createAction, PoolAction destroyAction)
    {
        m_createAction = createAction;
        m_destroyAction = destroyAction;

        Init(objectPrefab, poolCount);
    }

    void Init(T objectPrefab, int poolCount)
    {
        m_pooledObjects = new Queue<PooledObject>();

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
        var initialPooledObject = m_pooledObjects.Dequeue();
        m_pooledObjects.Enqueue(initialPooledObject);

        // Test if it can be used.
        if(!initialPooledObject.isActiveInPool)
        {
            // Pooled object can be used, set it active and return the target.
            initialPooledObject.SetIsActiveInPool(true);
            return initialPooledObject.target;
        }

        // Initial pooled object cannot be used. continue searching for next available.
        PooledObject pooledObject;
        do
        {
            // Get Next pooled object in queue.
            pooledObject = m_pooledObjects.Dequeue();
            m_pooledObjects.Enqueue(pooledObject);

            // Test if it can be used.
            if (!pooledObject.isActiveInPool)
            {
                // Pooled object can be used, set it active and return the target.
                pooledObject.SetIsActiveInPool(true);
                return pooledObject.target;
            }
        }
        while (pooledObject != initialPooledObject);
        // If all objects have been searched, and none are available. return null

        return default;
    }

    void CreateGameObject(T objectPrefab)
    {
        T newObject = m_createAction.Invoke(objectPrefab);
        PooledObject newPooledObject = new PooledObject(newObject);
        m_pooledObjects.Enqueue(newPooledObject);
        newPooledObject.SetIsActiveInPool(false);
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
        Queue<PooledObject> oldPooledObjects = m_pooledObjects;

        m_pooledObjects = new Queue<PooledObject>();

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
            DestroyGameObject(oldPooledObjects.Dequeue().target);
        }
    }
}