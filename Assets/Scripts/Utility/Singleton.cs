using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonBase<T> where T : SingletonBase<T>
{
    private static readonly Lazy<T> _lazyInstance = new Lazy<T>(() => Activator.CreateInstance(typeof(T), true) as T);
    public static T instance { get { return _lazyInstance.Value; } }
}

public abstract class MonoSingletonBase<T> : MonoBehaviour where T : MonoSingletonBase<T>
{
    private static readonly Lazy<T> _lazyInstance = new Lazy<T>(CreateSingleton);
    public static T instance { get { return _lazyInstance.Value; } }

    private static T CreateSingleton()
    {
        var ownerObject = new GameObject($"{typeof(T).Name} (singleton)");
        var instance = ownerObject.AddComponent<T>();
        DontDestroyOnLoad(ownerObject);
        return instance;
    }
}