using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonBase<T> where T : SingletonBase<T>
{
    private static readonly Lazy<T> _lazyInstance = new Lazy<T>(() => Activator.CreateInstance(typeof(T), true) as T);
    public static T instance { get { return _lazyInstance.Value; } }
}