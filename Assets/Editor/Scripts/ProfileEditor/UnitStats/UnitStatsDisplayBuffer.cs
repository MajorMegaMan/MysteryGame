using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MysterySystems.UnitStats;
using MysterySystems.UnitStats.Serialised;

public class UnitStatsDisplayBuffer : EditorDisplayBuffer
{
    enum PanelIndex
    {
        baseValues,
        growthValues
    }

    PanelIndex m_panelIndex = 0;

    public delegate void DeleteEvent(ScriptableUnitProfile profile);
    DeleteEvent m_deleteEvent = null;

    public bool deleteEventIsNull { get { return m_deleteEvent == null; } }

    public void SetSerialisedProfile(ScriptableUnitProfile profile)
    {
        SetSerialisedObject(profile);
    }

    protected override void OnGUI()
    {
        if(serialisedTarget != null)
        {
            DisplayGUI();
        }
        else
        {
            AdvanceRect();
            DrawPanelButtons();
        }
    }

    void DisplayGUI()
    {
        SerializedProperty unitProfileProperty = serialisedTarget.FindProperty("testProfile");
        SerializedProperty nameProperty = unitProfileProperty.FindPropertyRelative("m_unitName");

        DrawNextField(nameProperty);
        if(serialisedTarget.hasModifiedProperties)
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(serialisedTarget.targetObject), nameProperty.stringValue);
        }

        AdvanceRect();
        DrawPanelButtons();
        switch(m_panelIndex)
        {
            case PanelIndex.baseValues:
                {
                    DrawStatsPanel(unitProfileProperty, MysteryEditorUtility.Styles.solidTextures[0], "baseValue");
                    break;
                }
            case PanelIndex.growthValues:
                {
                    DrawStatsPanel(unitProfileProperty, MysteryEditorUtility.Styles.solidTextures[1], "growthValue");
                    break;
                }
        }
        
        // Delete button
        if(DrawButton("Delete"))
        {
            var profile = serialisedTarget.targetObject as ScriptableUnitProfile;
            m_deleteEvent?.Invoke(profile);
            SetSerialisedObject(null);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(profile));
            return;
        }
        // Clean button
        if (DrawButton("Clean Profile"))
        {
            var profile = serialisedTarget.targetObject as ScriptableUnitProfile;
            profile.testProfile.Clean();
            EditorUtility.SetDirty(profile);
            AssetDatabase.SaveAssets();
            return;
        }


        if (Apply())
        {
            EditorUtility.SetDirty(serialisedTarget.targetObject);
            AssetDatabase.SaveAssets();
        }
    }

    void DrawPanelButtons()
    {
        Rect buttonRect = currentDataRect;
        buttonRect.width *= 0.5f;

        var panelStyles = MysteryEditorUtility.Styles.panelStyles;
        if (GUI.Button(buttonRect, "Base Values", panelStyles[0]))
        {
            GUI.FocusControl(null);
            m_panelIndex = PanelIndex.baseValues;
        }

        buttonRect.x += buttonRect.width;
        if (GUI.Button(buttonRect, "Growth Values", panelStyles[1]))
        {
            GUI.FocusControl(null);
            m_panelIndex = PanelIndex.growthValues;
        }
    }

    void DrawStatsPanel(SerializedProperty unitProfileProperty, Texture2D panelTexture, string statValueType)
    {
        SerializedProperty resourceListProperty = unitProfileProperty.FindPropertyRelative("m_resourceKeyValues");
        SerializedProperty coreListProperty = unitProfileProperty.FindPropertyRelative("m_coreKeyValues");

        int heightCount = 1;
        Rect valuesRect = currentDataRect;
        valuesRect.y += valuesRect.height;
        heightCount += 2 + FindStatListHeightCount<ResourceStatKey>(resourceListProperty) + FindStatListHeightCount<CoreStatKey>(coreListProperty);
        valuesRect.height *= heightCount;
        valuesRect.x -= MysteryEditorSettings.windowControlSize;
        valuesRect.width += MysteryEditorSettings.windowControlSize * 2;
        GUI.DrawTexture(valuesRect, panelTexture);

        AdvanceRect();
        DrawLabel("Resource Values");
        DrawStats<ResourceStatKey>(resourceListProperty, statValueType);
        DrawLabel("Core Stats");
        DrawStats<CoreStatKey>(coreListProperty, statValueType);
        AdvanceRect();
    }

    void DrawStats<TStatKey>(SerializedProperty statListProperty, string statValueType) where TStatKey : System.Enum
    {
        if(statListProperty.arraySize == 0)
        {
            DrawLabel("------");
            return;
        }

        System.Array enumValues = System.Enum.GetValues(typeof(TStatKey));
        // enum string keys
        List<string> stringKeys = new List<string>(enumValues.Length);
        foreach(var value in enumValues)
        {
            TStatKey key = (TStatKey)value;
            stringKeys.Add(key.ToString());
        }

        // for each stat List as a property
        foreach (SerializedProperty prop in statListProperty)
        {
            // Get the string Id for this stat
            SerializedProperty keyStringIDProp = prop.FindPropertyRelative("keyStringID");
            string keyString = keyStringIDProp.stringValue;

            // If this stringId is an enum string
            if (stringKeys.Contains(keyStringIDProp.stringValue))
            {
                // All Good
                stringKeys.Remove(keyString);
            }
            else
            {
                // This property exists in the profile but doesn't exist as an enum
                keyString = keyString.Insert(0, "(Missing) ");
            }
            SerializedProperty statValueProp = prop.FindPropertyRelative(statValueType);

            GUIContent label = new GUIContent(keyString);

            Rect currentRect = GetDrawRect();
            currentRect = MysteryEditorUtility.GUI.RemoveControlBuffer(currentRect, true, false);

            EditorGUI.PropertyField(currentRect, statValueProp, label);
            AdvanceRect();
        }

        foreach (string keyString in stringKeys)
        {
            DrawLabel("(Uninitialised) " + keyString);
        }
    }

    public void AddDeleteEvent(DeleteEvent deleteEvent)
    {
        m_deleteEvent += deleteEvent;
    }

    public void RemoveDeleteEvent(DeleteEvent deleteEvent)
    {
        m_deleteEvent -= deleteEvent;
    }

    string UpperFirst(string text)
    {
        char[] charString = text.ToCharArray();
        charString[0] = charString.ToString().ToUpper()[0];
        return new string(charString);
    }

    int FindStatListHeightCount<TStatKey>(SerializedProperty statListProperty)
    {
        System.Array enumValues = System.Enum.GetValues(typeof(TStatKey));
        return Mathf.Max(enumValues.Length, statListProperty.arraySize);
    }
}
