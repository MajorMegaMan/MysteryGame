using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class EditorDisplayBuffer
{
    SerializedObject m_serialisedObject;

    Rect m_currentPosition;

    Rect m_nextDataFieldRect = Rect.zero;

    public SerializedObject serialisedTarget { get { return m_serialisedObject; } }
    public bool serialisedObjectIsDestroyed { get { return m_serialisedObject != null && m_serialisedObject.targetObject == null; } }
    public Rect currentPosition { get { return m_currentPosition; } }

    protected Rect currentDataRect { get { return m_nextDataFieldRect; } }

    public void SetSerialisedObject(Object targetObj)
    {
        if (targetObj != null)
        {
            m_serialisedObject = new SerializedObject(targetObj);
        }
        else
        {
            m_serialisedObject = null;
        }
    }

    public void DrawGUI(Rect position)
    {
        m_serialisedObject?.Update();

        m_currentPosition = position;
        m_nextDataFieldRect = position;
        m_nextDataFieldRect.height = EditorGUIUtility.singleLineHeight;

        OnGUI();
    }

    protected abstract void OnGUI();

    protected Rect GetDrawRect()
    {
        return m_nextDataFieldRect;
    }

    // draw target property
    protected void DrawNextField(SerializedProperty prop, bool drawChildren = true)
    {
        EditorGUI.PropertyField(GetDrawRect(), prop, drawChildren);
        int rectAdvanceCount = 1;
        if (prop.isExpanded)
        {
            var cpy = prop.Copy();
            rectAdvanceCount += cpy.CountInProperty();
        }
        AdvanceRect(rectAdvanceCount);
    }

    protected void DrawNextField(GUIContent label, SerializedProperty prop, bool drawChildren = true)
    {
        EditorGUI.PropertyField(GetDrawRect(), prop, label, drawChildren);
        int rectAdvanceCount = 1;
        if (prop.isExpanded)
        {
            var cpy = prop.Copy();
            rectAdvanceCount += cpy.CountInProperty();
        }
        AdvanceRect(rectAdvanceCount);
    }

    protected void DrawLabel(string text)
    {
        GUI.Label(GetDrawRect(), text);
        AdvanceRect();
    }

    protected bool DrawButton(string text)
    {
        bool press = GUI.Button(GetDrawRect(), text);
        AdvanceRect();
        return press;
    }

    protected void AdvanceRect(int count = 1)
    {
        m_nextDataFieldRect.y += m_nextDataFieldRect.height * count;
    }

    // apply property field changes.
    // returns true if a change was made.
    protected bool Apply()
    {
        return m_serialisedObject.ApplyModifiedProperties();
    }
}
