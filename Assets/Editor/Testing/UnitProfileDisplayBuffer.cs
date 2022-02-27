using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UnitProfileDisplayBuffer
{
    protected SerializedObject m_serialisedObject;

    ModelDisplayBuffer m_modelDisplay = new ModelDisplayBuffer();

    List<SerializedProperty> m_targetProperites = new List<SerializedProperty>();

    Rect m_currentPosition;
    float m_currentDataWidth;

    Rect m_modelPreviewRect = Rect.zero;
    Rect m_nextDataFieldRect = Rect.zero;

    // This is the rect used when the user will mouse over an area to resize the window
    Rect m_dataControlRect = Rect.zero;

    public float currentDataWidth { get { return m_currentDataWidth; } }

    public bool serialisedObjectIsDestroyed { get { return m_serialisedObject != null && m_serialisedObject.targetObject == null; } }

    public UnitProfileDisplayBuffer()
    {
        m_currentDataWidth = MysteryEditorSettings.defaultDataWidth;
    }

    public void SetSerialisedObject(UnitProfile unitProfile)
    {
        if(unitProfile != null)
        {
            m_serialisedObject = new SerializedObject(unitProfile);
            CacheProperties();
        }
        else
        {
            m_serialisedObject = null;
            m_targetProperites.Clear();
        }
    }

    public void Destroy()
    {
        DestroyModelObjectEditor();
    }

    public void DrawGUI(EditorWindow window, Rect position)
    {
        m_currentPosition = position;

        CalculateRects(position);

        MysteryEditorUtility.GUI.ControlRect(window, m_dataControlRect, MouseCursor.ResizeHorizontal, ResizeDataWidth);
        EditorGUI.DrawRect(m_dataControlRect, new Color(0.9f, 0.9f, 0.9f));

        if (m_serialisedObject != null)
        {
            // unitName
            DrawNextField(m_targetProperites[0]);

            // modelobject
            SerializedProperty modelProp = m_targetProperites[1];
            DrawNextField(modelProp);

            ShowModelPreview(m_modelPreviewRect, modelProp.objectReferenceValue as GameObject);

            // base stats
            DrawNextField(m_targetProperites[2]);

            // growth stats
            DrawNextField(m_targetProperites[3]);

            if(Apply())
            {
                // Field has changed
                Debug.Log("Changed");
                string assetPath = AssetDatabase.GetAssetPath(m_serialisedObject.targetObject);
                AssetDatabase.RenameAsset(assetPath, m_targetProperites[0].stringValue + "_Profile");
                AssetDatabase.Refresh();
            }
        }
        else
        {
            ShowModelPreview(m_modelPreviewRect, null);
        }
    }

    void CalculateRects(Rect position)
    {
        float previewWidth = position.width - m_currentDataWidth;

        // calc data control rect
        m_dataControlRect.width = MysteryEditorSettings.windowControlSize;
        m_dataControlRect.height = position.height + MysteryEditorSettings.windowControlSize * 2;
        m_dataControlRect.x = position.x + previewWidth - (m_dataControlRect.width / 2.0f);
        m_dataControlRect.y = position.y - MysteryEditorSettings.windowControlSize;


        m_modelPreviewRect.x = position.x;
        m_modelPreviewRect.y = position.y;
        m_modelPreviewRect.width = previewWidth;
        m_modelPreviewRect.height = position.height;

        // reuse nextRect for each field, the y position will incremently add when a field is to be drawn
        m_nextDataFieldRect.x = position.x + previewWidth;
        m_nextDataFieldRect.y = position.y;
        m_nextDataFieldRect.width = m_currentDataWidth;
        m_nextDataFieldRect.height = EditorGUIUtility.singleLineHeight;

        m_modelPreviewRect = MysteryEditorUtility.GUI.RemoveControlBuffer(m_modelPreviewRect, true, false);
        m_nextDataFieldRect = MysteryEditorUtility.GUI.RemoveControlBuffer(m_nextDataFieldRect, true, false);
    }

    void CacheProperties()
    {
        m_targetProperites.Clear();
        m_targetProperites.Add(GetProperty("m_unitName"));
        m_targetProperites.Add(GetProperty("m_modelGameObjectPrefab"));
        m_targetProperites.Add(GetProperty("m_baseStats"));
        m_targetProperites.Add(GetProperty("m_statGrowth"));
    }

    // creates an editor to draw the model preview
    protected void ShowModelPreview(Rect previewRect, GameObject modelObject)
    {
        m_modelDisplay.DrawGUI(previewRect, modelObject);
    }

    // destroy the model preview editor
    protected void DestroyModelObjectEditor()
    {
        m_modelDisplay.Destroy();
    }

    // get a property from this serialisedobject
    protected SerializedProperty GetProperty(string propName)
    {
        if (m_serialisedObject != null)
        {
            return m_serialisedObject.FindProperty(propName);
        }
        return null;
    }

    // draw target property
    protected void DrawNextField(SerializedProperty prop, bool drawChildren = true)
    {
        MysteryEditorUtility.GUI.DrawField(m_nextDataFieldRect, prop, drawChildren);
        m_nextDataFieldRect.y += m_nextDataFieldRect.height;
        if (prop.isExpanded)
        {
            var cpy = prop.Copy();
            m_nextDataFieldRect.y += m_nextDataFieldRect.height * cpy.CountInProperty();
        }
    }

    // apply property field changes.
    // returns true if a change was made.
    protected bool Apply()
    {
        return m_serialisedObject.ApplyModifiedProperties();
    }

    public void ResizeDataWidth(Vector2 mousePosition)
    {
        m_currentDataWidth = m_currentPosition.max.x - mousePosition.x;
        float min = MysteryEditorSettings.minDataWidth;
        float max = m_currentPosition.width - MysteryEditorSettings.minDisplayWidth;
        m_currentDataWidth = Mathf.Clamp(m_currentDataWidth, min, max);
    }
}
