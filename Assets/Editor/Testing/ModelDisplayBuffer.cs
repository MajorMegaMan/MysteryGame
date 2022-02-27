using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ModelDisplayBuffer
{
    Editor m_modelObjectEditor = null;
    Object m_previousModelObject = null;

    bool m_isWaiting = false;

    public void Destroy()
    {
        DestroyModelObjectEditor();
    }

    public void DrawGUI(Rect position, Object model)
    {
        ShowModelPreview(position, model);
    }

    // creates an editor to draw the model preview
    protected void ShowModelPreview(Rect previewRect, Object modelObject)
    {
        string label;
        if (EditorApplication.isPlayingOrWillChangePlaymode || m_isWaiting)
        {
            label = "Application is playing";
            GUI.Box(previewRect, "", MysteryEditorUtility.Styles.blackStyle);
        }
        else
        {
            if (modelObject != null)
            {
                label = modelObject.name;

                if (m_modelObjectEditor == null)
                {
                    // create editor for the model
                    CreateModelObjectEditor(modelObject);
                }
                else if (modelObject != m_previousModelObject)
                {
                    // destroy old and create new editor for the model
                    Object.DestroyImmediate(m_modelObjectEditor);
                    CreateModelObjectEditor(modelObject);
                }

                m_modelObjectEditor.DrawPreview(previewRect);
            }
            else
            {
                label = "No model Loaded";
                GUI.Box(previewRect, "", MysteryEditorUtility.Styles.blackStyle);
            }
        }

        Rect labelRect = previewRect;
        labelRect.height = 40;
        Vector2 labelPos = labelRect.position;
        labelPos.y = previewRect.position.y + previewRect.height - labelRect.height * 2;
        labelRect.position = labelPos;

        GUI.Label(labelRect, label, MysteryEditorUtility.Styles.previewTextStyle);
    }

    // create the model preview editor
    protected void CreateModelObjectEditor(Object modelObject)
    {
        m_modelObjectEditor = Editor.CreateEditor(modelObject);
        m_previousModelObject = modelObject;
    }

    // destroy the model preview editor
    protected void DestroyModelObjectEditor()
    {
        if (m_modelObjectEditor != null)
        {
            Object.DestroyImmediate(m_modelObjectEditor);
            m_modelObjectEditor = null;
        }
    }
}
