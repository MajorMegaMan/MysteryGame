using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UnitProfileEditorWindow : ExtendedEditorWindow
{
    UnitProfile m_selectedUnitProfile = null;
    UnitProfileDisplayBuffer m_displayBuffer;

    List<UnitProfile> m_unitProfiles;

    Rect m_sideBarPosition;

    string m_newUnitProfileName = "NewUnit";

    [MenuItem("MysteryEditors/Unit Profile")]
    public static void OpenWindow()
    {
        OpenWindow(null);
    }

    // opens a window for unit profiles
    public static void OpenWindow(UnitProfile unitProfile)
    {
        UnitProfileEditorWindow window = GetWindow<UnitProfileEditorWindow>("Unit Profile Editor");

        window.m_unitProfiles = LoadUnitProfiles();
        window.m_displayBuffer = new UnitProfileDisplayBuffer();
        window.SelectUnitProfile(unitProfile);

        window.ResetSideBarPosition();
        window.Show();
    }

    // Selects a unit profile to focus on
    void SelectUnitProfile(UnitProfile unitProfile)
    {
        m_selectedUnitProfile = unitProfile;
        m_displayBuffer.SetSerialisedObject(unitProfile);
    }

    // Sets the min size of the window by calculating the min width with current panel sizings.
    // This is called once on window open and during the update step while resizing a window.
    void FindMinHorizontal()
    {
        Vector2 minSize = this.minSize;
        float sideBarConstraint = Mathf.Max(MysteryEditorSettings.minSideBarWidth, m_sideBarPosition.width);
        float dataConstraint = Mathf.Max(MysteryEditorSettings.minDataWidth, m_displayBuffer.currentDataWidth);
        minSize.x = sideBarConstraint + MysteryEditorSettings.minDisplayWidth + dataConstraint;
        this.minSize = minSize;
    }

    // Loads a UnitProfile list from mystery settings unit profile path
    static List<UnitProfile> LoadUnitProfiles()
    {
        List<UnitProfile> result = new List<UnitProfile>();

        // get the data path without the assets suffix
        string appDataPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);

        // get all files in directory
        string[] files = System.IO.Directory.GetFiles(appDataPath + MysteryEditorSettings.unitProfilesPath);
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".meta"))
            {
                continue;
            }

            string localPath = files[i].Substring(appDataPath.Length);
            UnitProfile unitProfile = AssetDatabase.LoadAssetAtPath<UnitProfile>(localPath);
            result.Add(unitProfile);
        }

        return result;
    }

    private void OnGUI()
    {
        // This is a bandaid fix for when unity recompiles scripts. The references are lost as these classes are not serialised.
        // To serialise them properly takes more effort than only adding the serializable attribute, so this will do for now.
        if(m_displayBuffer == null)
        {
            m_displayBuffer = new UnitProfileDisplayBuffer();
            SelectUnitProfile(m_selectedUnitProfile);
        }
        else
        {
            if(m_displayBuffer.serialisedObjectIsDestroyed)
            {
                m_unitProfiles = LoadUnitProfiles();
                SelectUnitProfile(null);
            }
        }

        // calc side bar control rect
        Rect sideBarControlRect = m_sideBarPosition;
        sideBarControlRect.width = MysteryEditorSettings.windowControlSize;
        sideBarControlRect.height = position.height;
        sideBarControlRect.x = m_sideBarPosition.width - (sideBarControlRect.width / 2.0f);

        ControlRect(sideBarControlRect, MouseCursor.ResizeHorizontal, ResizeSideBar);

        Rect fieldPosition = MysteryEditorUtility.GUI.RemoveControlBuffer(m_sideBarPosition, true, false);

        // Show unitProfile List buttons
        for(int i = 0; i < m_unitProfiles.Count; i++)
        {
            UnitProfile listedProfile = m_unitProfiles[i];
            if (listedProfile == null)
            {
                m_unitProfiles.RemoveAt(i);
                i--;
                continue;
            }

            DisplayProfileButton(fieldPosition, listedProfile);

            fieldPosition.y += fieldPosition.height;
        }

        Rect bottomPosition = MysteryEditorUtility.GUI.RemoveControlBuffer(m_sideBarPosition, true, false);
        bottomPosition.y = position.height - m_sideBarPosition.height * 3;
        DisplayCreateProfileButton(bottomPosition, m_newUnitProfileName);
        bottomPosition.y += bottomPosition.height;
        m_newUnitProfileName = EditorGUI.TextField(bottomPosition, m_newUnitProfileName);

        Rect displayRect = Rect.zero;
        displayRect.x += m_sideBarPosition.width;
        displayRect.width = position.width - m_sideBarPosition.width;
        displayRect.height = position.height;

        displayRect = MysteryEditorUtility.GUI.RemoveControlBuffer(displayRect, false, true);

        m_displayBuffer.DrawGUI(this, displayRect);

        EditorGUI.DrawRect(sideBarControlRect, new Color(0.9f, 0.9f, 0.9f));
    }

    void ResetSideBarPosition()
    {
        m_sideBarPosition = Rect.zero;
        m_sideBarPosition.width = MysteryEditorSettings.defaultSideBarWidth;
        m_sideBarPosition.height = EditorGUIUtility.singleLineHeight;

        FindMinHorizontal();
    }

    void ResizeSideBar(Vector2 mousePosition)
    {
        m_sideBarPosition.width = mousePosition.x - m_sideBarPosition.x;
        float min = MysteryEditorSettings.minSideBarWidth;
        float max = position.width - (MysteryEditorSettings.minDisplayWidth + m_displayBuffer.currentDataWidth);
        m_sideBarPosition.width = Mathf.Clamp(m_sideBarPosition.width, min, max);
    }

    void DisplayProfileButton(Rect buttonRect, UnitProfile unitProfile)
    {
        if (GUI.Button(buttonRect, unitProfile.name))
        {
            if (unitProfile != m_selectedUnitProfile)
            {
                SelectUnitProfile(unitProfile);
                GUI.FocusControl(null);
            }
        }
    }

    void DisplayCreateProfileButton(Rect buttonRect, string newProfileName)
    {
        if (GUI.Button(buttonRect, "Create New Profile"))
        {
            UnitProfile newUnitProfile = MysteryEditorUtility.Creators.CreateNewUnitProfile(newProfileName);
            if (newUnitProfile != m_selectedUnitProfile)
            {
                SelectUnitProfile(newUnitProfile);
            }
            m_unitProfiles = LoadUnitProfiles();
        }
    }
}
