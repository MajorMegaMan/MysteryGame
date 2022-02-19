using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UnitProfileEditorWindow : MysteryWindow
{
    UnitProfile m_unitProfile = null;

    bool m_searchingForModel = false;
    bool m_modelPickerWindowOpen = false;
    int m_modelPickerWindow = 0;

    string m_unitName;

    // Add menu named "My Window" to the Window menu
    [MenuItem("MysteryEditors/Unit Profile")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        UnitProfileEditorWindow window = (UnitProfileEditorWindow)EditorWindow.GetWindow(typeof(UnitProfileEditorWindow));
        window.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        Rect previewRect = GUILayoutUtility.GetRect(position.width - MysteryEditorSettings.minDataWidth, position.height);

        EditorGUILayout.BeginVertical();
        m_unitProfile = FindUnitProfileLayout(m_unitProfile);

        GUILayout.Label("Unit Info", EditorStyles.boldLabel);

        GUILayout.Space(8);

        UnitProfileInfo info;
        if (m_unitProfile != null)
        {
            info = EditorUnitProfile.GetProfileInfo(m_unitProfile);
            info.unitName = EditorGUILayout.TextField("Name", info.unitName);
            m_unitName = null;
        }
        else
        {
            info = UnitProfileInfo.zero;
            m_unitName = EditorGUILayout.TextField("Name", m_unitName);
            info.unitName = m_unitName;
        }

        EditorGUI.BeginDisabledGroup(m_unitProfile == null);

        GUILayout.Space(16);
        info.modelObject = FindModelLayout(info.modelObject);

        info = ShowUnitStats(info);
        EditorGUI.EndDisabledGroup();
        ShowModelPreview(previewRect, info.modelObject);

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        if (m_unitProfile != null)
        {
            EditorUnitProfile.SetProfileInfo(m_unitProfile, info);
        }
    }

    UnitProfileInfo ShowUnitStats(UnitProfileInfo info)
    {
        GUILayout.Space(24);
        GUILayout.Label("Base Stats", EditorStyles.boldLabel);
        info.baseStats = UnitStatsLayout(info.baseStats);
        GUILayout.Space(24);
        GUILayout.Label("Growth Stats", EditorStyles.boldLabel);
        info.growthStats = UnitStatsLayout(info.growthStats);

        return info;
    }

    int ShowPicker(string searchFocus)
    {
        int pickerWindow = EditorGUIUtility.GetControlID(FocusType.Passive) + 100;
        EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, searchFocus, pickerWindow);
        return pickerWindow;
    }

    UnitProfile FindUnitProfileLayout(UnitProfile unitProfile)
    {
        unitProfile = EditorGUILayout.ObjectField("UnitProfile", unitProfile, typeof(UnitProfile), false) as UnitProfile;
        if(GUILayout.Button("Create New Profile"))
        {
            unitProfile = CreateScriptableObject<UnitProfile>(MysteryEditorSettings.unitProfilesPath + m_unitName + "_Profile.asset", EditUnitProfile, m_unitName);
        }
        return unitProfile;
    }

    void EditUnitProfile<T>(T scriptableSource, object[] additionalObjects) where T : UnitProfile
    {
        UnitProfile.Edit.SetUnitName(scriptableSource, additionalObjects[0] as string);
    }

    GameObject FindModelLayout(GameObject modelObject)
    {
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Model", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        if(modelObject != null)
        {
            GUILayout.Label(modelObject.name);
        }
        else
        {
            GUILayout.Label("null");
        }
        if (GUILayout.Button("Search"))
        {
            m_searchingForModel = true;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        if(m_searchingForModel)
        {
            if(!m_modelPickerWindowOpen)
            {
                m_modelPickerWindowOpen = true;
                m_modelPickerWindow = ShowPicker("ModelObject");
            }

            if(EditorGUIUtility.GetObjectPickerControlID() == m_modelPickerWindow)
            {
                if (Event.current.commandName == "ObjectSelectorUpdated")
                {
                    modelObject = EditorGUIUtility.GetObjectPickerObject() as GameObject;
                }
                else if (Event.current.commandName == "ObjectSelectorClosed")
                {
                    m_modelPickerWindowOpen = false;
                    m_searchingForModel = false;
                }
            }
        }

        return modelObject;
    }

    UnitStats UnitStatsLayout(UnitStats unitStats)
    {
        UnitStats result = UnitStats.zero;
        result.health = EditorGUILayout.FloatField("health", unitStats.health);
        result.strength = EditorGUILayout.FloatField("strength", unitStats.strength);
        result.speed = EditorGUILayout.FloatField("speed", unitStats.speed);
        return result;
    }
}

struct UnitProfileInfo
{
    public string unitName;
    public GameObject modelObject;
    public UnitStats baseStats;
    public UnitStats growthStats;

    static UnitProfileInfo _zero;
    public static UnitProfileInfo zero { get { return _zero; } }

    static UnitProfileInfo()
    {
        _zero = new UnitProfileInfo();
        _zero.unitName = "null";
        _zero.modelObject = null;
        _zero.baseStats = UnitStats.zero;
        _zero.growthStats = UnitStats.zero;
    }
}

static class EditorUnitProfile
{
    public static UnitProfileInfo GetProfileInfo(UnitProfile profile)
    {
        UnitProfileInfo info = new UnitProfileInfo();
        info.unitName = profile.unitName;
        info.modelObject = UnitProfile.Edit.GetModelGameObject(profile);
        info.baseStats = profile.baseStats;
        info.growthStats = profile.statGrowth;
        return info;
    }

    public static void SetProfileInfo(UnitProfile profile, UnitProfileInfo info)
    {
        bool nameChanged = UnitProfile.Edit.SetUnitName(profile, info.unitName);
        bool modelChanged = UnitProfile.Edit.SetModelGameObject(profile, info.modelObject);
        bool baseChanged = UnitProfile.Edit.SetBaseStats(profile, info.baseStats);
        bool growChanged = UnitProfile.Edit.SetGrowthStats(profile, info.growthStats);

        if(nameChanged || modelChanged || baseChanged || growChanged)
        {
            EditorUtility.SetDirty(profile);
        }
    }
}