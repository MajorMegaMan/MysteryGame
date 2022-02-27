using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MysteryEditorSettings : ScriptableObject
{
    static MysteryEditorSettings _instance = null;
    delegate MysteryEditorSettings InstanceGetter();
    static InstanceGetter _instanceGetter = CreateInstance;

    [Header("Paths")]
    [SerializeField] string m_assetPath = "Assets/";
    [SerializeField] string m_scriptablePath = "Scriptables/";
    [SerializeField] string m_prefabPath = "Prefabs/";

    [SerializeField] string m_unitProfilesPath = "Units/Profiles/";
    [SerializeField] string m_modelObjectsPath = "Units/Models/";

    [Header("Window Settings")]
    [SerializeField] float m_minDataWidth = 100;
    [SerializeField] float m_defaultDataWidth = 320;
    [SerializeField] float m_minDisplayWidth = 100;

    [Header("Sidebar")]
    [SerializeField] float m_defaultSideBarWidth = 200;
    [SerializeField] float m_minSideBarWidth = 100;

    [Header("Control")]
    [SerializeField] float m_windowControlSize = 4;

    // getters
    static MysteryEditorSettings instance { get { return _instanceGetter.Invoke(); } }
    // private paths
    string combinedUnitProfilesPath { get { return m_assetPath + m_scriptablePath + m_unitProfilesPath; } }
    string combinedModelObjectsPath { get { return m_assetPath + m_prefabPath + m_modelObjectsPath; } }

    // public paths
    public static string unitProfilesPath { get { return instance.combinedUnitProfilesPath; } }
    public static string modelObjectsPath { get { return instance.combinedModelObjectsPath; } }

    // window settings
    public static float minDataWidth { get { return instance.m_minDataWidth; } }
    public static float defaultSideBarWidth { get { return instance.m_defaultSideBarWidth; } }
    public static float minDisplayWidth { get { return instance.m_minDisplayWidth; } }

    // sidebar
    public static float minSideBarWidth { get { return instance.m_minSideBarWidth; } }
    public static float defaultDataWidth { get { return instance.m_defaultDataWidth; } }

    // control
    public static float windowControlSize { get { return instance.m_windowControlSize; } }

    static MysteryEditorSettings CreateInstance()
    {
        if(_instance == null)
        {
            string settingsPath = "Assets/Editor/MysteryEditorSettings.asset";
            _instance = AssetDatabase.LoadAssetAtPath<MysteryEditorSettings>(settingsPath);

            if(_instance == null)
            {
                _instance = MysteryEditorUtility.Creators.CreateScriptableObject<MysteryEditorSettings>("Assets/Editor/MysteryEditorSettings.asset");
            }
        }
        _instanceGetter = GetInstance;
        return _instance;
    }

    static MysteryEditorSettings GetInstance()
    {
        return _instance;
    }

    private void OnDestroy()
    {
        _instance = null;
        _instanceGetter = CreateInstance;
    }
}
