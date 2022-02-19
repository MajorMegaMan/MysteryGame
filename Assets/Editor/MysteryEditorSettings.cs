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

    [Header("Other Settings")]
    [SerializeField] float m_minDataWidth = 320;

    string combinedUnitProfilesPath { get { return m_assetPath + m_scriptablePath + m_unitProfilesPath; } }
    string combinedModelObjectsPath { get { return m_assetPath + m_prefabPath + m_modelObjectsPath; } }

    static MysteryEditorSettings instance { get { return _instanceGetter.Invoke(); } }
    public static string unitProfilesPath { get { return instance.combinedUnitProfilesPath; } }
    public static string modelObjectsPath { get { return instance.combinedModelObjectsPath; } }
    public static float minDataWidth { get { return instance.m_minDataWidth; } }

    static MysteryEditorSettings CreateInstance()
    {
        if(_instance == null)
        {
            string settingsPath = "Assets/Editor/MysteryEditorSettings.asset";
            _instance = AssetDatabase.LoadAssetAtPath<MysteryEditorSettings>(settingsPath);

            if(_instance == null)
            {
                _instance = UnitEditorHelpers.CreateScriptableObject<MysteryEditorSettings>("Assets/Editor/MysteryEditorSettings.asset");
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
        _instanceGetter = CreateInstance;
    }
}
