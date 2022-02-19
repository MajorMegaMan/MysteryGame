using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class MysteryWindow : EditorWindow
{
    Editor m_modelObjectEditor = null;
    GameObject m_previousModelObject = null;

    bool m_isWaiting = false;

    protected void ShowModelPreview(Rect previewRect, GameObject modelObject)
    {
        string label;
        if(EditorApplication.isPlayingOrWillChangePlaymode || m_isWaiting)
        {
            label = "Application is playing";
            GUI.Box(previewRect, "", UnitEditorHelpers.blackStyle);
        }
        else
        {
            if (modelObject != null)
            {
                label = modelObject.name;

                if (m_modelObjectEditor == null)
                {
                    CreateModelObjectEditor(modelObject);
                }
                else if (modelObject != m_previousModelObject)
                {
                    DestroyImmediate(m_modelObjectEditor);
                    CreateModelObjectEditor(modelObject);
                }

                m_modelObjectEditor.OnPreviewGUI(previewRect, EditorStyles.whiteLabel);
            }
            else
            {
                label = "No model Loaded";
                GUI.Box(previewRect, "", UnitEditorHelpers.blackStyle);
            }
        }

        Rect labelRect = previewRect;
        labelRect.height = 40;
        Vector2 labelPos = labelRect.position;
        labelPos.y = previewRect.position.y + previewRect.height - labelRect.height * 2;
        labelRect.position = labelPos;

        GUI.Label(labelRect, label, UnitEditorHelpers.previewTextStyle);
    }

    protected void CreateModelObjectEditor(GameObject modelObject)
    {
        m_modelObjectEditor = Editor.CreateEditor(modelObject);
        m_previousModelObject = modelObject;
    }

    protected void DestroyModelObjectEditor()
    {
        if(m_modelObjectEditor != null)
        {
            DestroyImmediate(m_modelObjectEditor);
            m_modelObjectEditor = null;
        }
    }

    protected T CreateScriptableObject<T>(string path) where T : ScriptableObject
    {
        return UnitEditorHelpers.CreateScriptableObject<T>(path);
    }

    protected T CreateScriptableObject<T>(string path, UnitEditorHelpers.EditSourceFunc<T> editFunc, params object[] objects) where T : ScriptableObject
    {
        return UnitEditorHelpers.CreateScriptableObject<T>(path, editFunc, objects);
    }

    protected GameObject CreateObjectPrefab(GameObject sourceModelPrefab, string path)
    {
        return UnitEditorHelpers.CreateObjectPrefab(sourceModelPrefab, path);
    }

    protected GameObject CreateObjectPrefab(GameObject sourceModelPrefab, string path, UnitEditorHelpers.EditSourceFunc editFunc, params object[] objects)
    {
        return UnitEditorHelpers.CreateObjectPrefab(sourceModelPrefab, path, editFunc, objects);
    }

    protected virtual void PlayStateChanged(PlayModeStateChange playModeStateChange)
    {
        if (playModeStateChange == PlayModeStateChange.EnteredPlayMode)
        {
            // this will reset the preview window
            DestroyModelObjectEditor();
            m_isWaiting = true;
        }
        if (playModeStateChange == PlayModeStateChange.EnteredEditMode)
        {
            m_isWaiting = false;
        }
    }

    protected virtual void OnEnable()
    {
        EditorApplication.playModeStateChanged += PlayStateChanged;
    }

    protected virtual void OnDisable()
    {
        EditorApplication.playModeStateChanged -= PlayStateChanged;
    }
}
