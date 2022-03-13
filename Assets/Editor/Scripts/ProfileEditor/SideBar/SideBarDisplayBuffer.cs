using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SideBarDisplayBuffer : EditorDisplayBuffer
{
    SerializedProperty m_profileListProperty = null;
    int listCount;

    public delegate void SelectEvent(ScriptableUnitProfile target);
    SelectEvent m_selectEvent = null;

    public bool selectEventIsNull { get { return m_selectEvent == null; } }

    public void Init(UnitProfileSet unitProfilesSet)
    {
        SetSerialisedObject(unitProfilesSet);
        listCount = unitProfilesSet.count;
        m_profileListProperty = serialisedTarget.FindProperty("m_serialisedUnitProfileList");
    }

    protected override void OnGUI()
    {
        if(m_profileListProperty == null)
        {
            Init(MysteryEditorSettings.currentProfileSet);
        }

        foreach (SerializedProperty prop in m_profileListProperty)
        {
            //DrawButton(prop.FindPropertyRelative("m_unitName").stringValue);
            if (DrawButton(prop.objectReferenceValue.name))
            {
                InvokeSelectEvent(prop.objectReferenceValue as ScriptableUnitProfile);
            }
        }

        if (DrawButton("Create New"))
        {
            var newProfile = UnitProfileSetEditor.CreateUnitProfile("Test");
            InvokeSelectEvent(newProfile);
        }

        if (DrawButton("Clean All"))
        {
            foreach (SerializedProperty prop in m_profileListProperty)
            {
                var scriptableProfile = prop.objectReferenceValue as ScriptableUnitProfile;
                scriptableProfile.testProfile.Clean();
                EditorUtility.SetDirty(scriptableProfile);
            }
            AssetDatabase.SaveAssets();
        }
    }

    public void InvokeSelectEvent(ScriptableUnitProfile profile)
    {
        GUI.FocusControl(null);
        m_selectEvent?.Invoke(profile);
    }

    public void AddSelectEvent(SelectEvent selectEvent)
    {
        m_selectEvent += selectEvent;
    }

    public void RemoveSelectEvent(SelectEvent selectEvent)
    {
        m_selectEvent -= selectEvent;
    }
}
