using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MysteryEditorSettings : ScriptableSingletonBase<MysteryEditorSettings>
{
    [SerializeField] UnitProfileSet m_currentProfileSet = null;

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
    public static UnitProfileSet currentProfileSet { get { return instance.GetCurrentProfileSet(); } }

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

    UnitProfileSet GetCurrentProfileSet()
    {
        if(m_currentProfileSet == null)
        {
            string[] guids;

            // search for a ScriptObject called ScriptObj
            guids = AssetDatabase.FindAssets("t:UnitProfileSet");
            if(guids.Length > 0)
            {
                return AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(UnitProfileSet)) as UnitProfileSet;
            }
            else
            {
                return MysteryEditorUtility.Creators.CreateScriptableObject<UnitProfileSet>(unitProfilesPath);
            }
        }
        else
        {
            return m_currentProfileSet;
        }
    }
}

class SettingsAssetHandler : UnityEditor.AssetModificationProcessor
{
    static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions opt)
    {
        System.Type assetType = AssetDatabase.GetMainAssetTypeAtPath(path);
        if (assetType == typeof(MysteryEditorSettings))
        {
            MysteryEditorSettings.CleanInstance();
        }
        return AssetDeleteResult.DidNotDelete;
    }
}
