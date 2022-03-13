using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public  class ScriptableSingletonBase<T> : ScriptableObject where T : ScriptableSingletonBase<T>
{
    static T _instance = null;
    delegate T InstanceGetter();
    static InstanceGetter _instanceGetter = CreateInstance;
    public static T instance { get { return _instanceGetter.Invoke(); } }
    
    static T CreateInstance()
    {
        if (_instance == null)
        {
            string settingsPath = "Assets/Editor/" + typeof(T).Name + ".asset";
            _instance = AssetDatabase.LoadAssetAtPath<T>(settingsPath);
    
            if (_instance == null)
            {
                _instance = MysteryEditorUtility.Creators.CreateScriptableObject<T>(settingsPath);
            }
        }
        _instanceGetter = GetInstance;
        return _instance;
    }
    
    static T GetInstance()
    {
        return _instance;
    }
    
    // This should be called whenever the scriptable is deleted or the user wants to lose the reference.
    public static void CleanInstance()
    {
        _instance = null;
        _instanceGetter = CreateInstance;
    }
}
