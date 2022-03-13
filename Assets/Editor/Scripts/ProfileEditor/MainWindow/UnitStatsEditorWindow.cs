using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MysterySystems.UnitStats;
using MysterySystems.UnitStats.Serialised;

public class UnitStatsEditorWindow : ExtendedEditorWindow
{
    UnitProfileSet m_targetProfileSet = null;
    SideBarDisplayBuffer m_sideBarDisplay = new SideBarDisplayBuffer();
    UnitStatsDisplayBuffer m_unitProfileDisplay = new UnitStatsDisplayBuffer();

    Rect m_sideBarPosition = Rect.zero;
    Rect m_statsPosition = Rect.zero;

    float m_currentSidebarWidth = 200;

    [MenuItem("MysteryEditors/Unit Stats")]
    public static void OpenWindow()
    {
        OpenWindow(MysteryEditorSettings.currentProfileSet, null);
    }

    public static void OpenWindow(Object targetProfile)
    {
        OpenWindow(MysteryEditorSettings.currentProfileSet, targetProfile);
    }

    public static void OpenWindow(UnitProfileSet profileSet, Object targetProfile)
    {
        UnitStatsEditorWindow window = GetWindow<UnitStatsEditorWindow>("Unit Stat Editor");
        window.Show();

        window.Init(profileSet, targetProfile);
    }

    void Init(UnitProfileSet profileSet, Object target)
    {
        m_targetProfileSet = profileSet;

        m_sideBarDisplay.Init(profileSet);
        m_unitProfileDisplay.SetSerialisedObject(target);

        m_currentSidebarWidth = MysteryEditorSettings.defaultSideBarWidth;

        m_sideBarDisplay.AddSelectEvent(m_unitProfileDisplay.SetSerialisedProfile);
        m_unitProfileDisplay.AddDeleteEvent(OnProfileDelete);
    }

    private void OnGUI()
    {
        EnsureWindowIsFunctioning();
        CalcWindowSizes();

        Rect siderBarControlRect = position;
        siderBarControlRect.width = MysteryEditorSettings.windowControlSize * 2;
        siderBarControlRect.x = m_currentSidebarWidth - MysteryEditorSettings.windowControlSize;
        siderBarControlRect.y = 0;
        MysteryEditorUtility.GUI.ControlRect(this, siderBarControlRect, MouseCursor.ResizeHorizontal, ResizeSideBar);
        GUI.DrawTexture(siderBarControlRect, MysteryEditorUtility.Styles.solidTextures[0]);

        m_sideBarDisplay.DrawGUI(m_sideBarPosition);
        m_unitProfileDisplay.DrawGUI(m_statsPosition);
    }

    void OnProfileDelete(ScriptableUnitProfile profile)
    {
        UnitProfileSetEditor.RemoveUnitProfile(profile);
    }

    void EnsureWindowIsFunctioning()
    {
        Vector2 min = minSize;
        min.x = MysteryEditorSettings.minSideBarWidth + MysteryEditorSettings.minDataWidth;
        minSize = min;

        if (m_targetProfileSet == null)
        {
            Init(MysteryEditorSettings.currentProfileSet, null);
        }
        else
        {
            if (m_sideBarDisplay.selectEventIsNull)
            {
                m_sideBarDisplay.AddSelectEvent(m_unitProfileDisplay.SetSerialisedProfile);
            }
            if (m_unitProfileDisplay.deleteEventIsNull)
            {
                m_unitProfileDisplay.AddDeleteEvent(OnProfileDelete);
            }
        }
    }

    void CalcWindowSizes()
    {
        // This is the absolute min size
        Vector2 min = minSize;
        min.x = MysteryEditorSettings.minSideBarWidth + MysteryEditorSettings.minDataWidth;
        minSize = min;

        m_sideBarPosition = position;
        m_sideBarPosition.x = 0;
        m_sideBarPosition.y = 0;
        m_sideBarPosition.width = m_currentSidebarWidth;

        m_statsPosition = m_sideBarPosition;
        m_statsPosition.x = m_sideBarPosition.width;
        m_statsPosition.width = position.width - m_sideBarPosition.width;

        if(m_statsPosition.width < MysteryEditorSettings.minDataWidth)
        {
            m_statsPosition.width = MysteryEditorSettings.minDataWidth;
            m_currentSidebarWidth = position.width - m_statsPosition.width;
            m_sideBarPosition.width = m_currentSidebarWidth;
            m_statsPosition.x = m_sideBarPosition.width;
        }

        m_sideBarPosition = MysteryEditorUtility.GUI.RemoveControlBuffer(m_sideBarPosition);
        m_statsPosition = MysteryEditorUtility.GUI.RemoveControlBuffer(m_statsPosition);
    }

    void ResizeSideBar(Vector2 mousePosition)
    {
        m_currentSidebarWidth = mousePosition.x;
        m_currentSidebarWidth = Mathf.Max(m_currentSidebarWidth, MysteryEditorSettings.minSideBarWidth);
    }
}
