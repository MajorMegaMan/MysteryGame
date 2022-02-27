using System;

public abstract class SingletonBase<T> where T : SingletonBase<T>
{
    private static readonly Lazy<T> _lazyInstance = new Lazy<T>(() => Activator.CreateInstance(typeof(T), true) as T);
    public static T Instance => _lazyInstance.Value;
}