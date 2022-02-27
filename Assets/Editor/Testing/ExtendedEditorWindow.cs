using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExtendedEditorWindow : EditorWindow
{
    protected delegate void MouseAction(Vector2 mousePosition);
    MouseAction m_mouseAction;

    private delegate void MouseControlAction(Rect controlRect, MouseCursor mouseCursor, MouseAction mouseAction);
    MouseControlAction m_mouseControlAction;

    public ExtendedEditorWindow() : base()
    {
        m_mouseControlAction = WaitForMouseDown;
    }

    // This is a function that will handle mouseInput for the supplied Rect.
    // Example creating an area to for resizing shapes in the window, such as a sidebar
    protected void ControlRect(Rect controlRect, MouseCursor mouseCursor, MouseAction mouseAction)
    {
        m_mouseControlAction.Invoke(controlRect, mouseCursor, mouseAction);
    }

    // Action for when the mouse is hovering over the supplied Rect
    void WaitForMouseDown(Rect controlRect, MouseCursor mouseCursor, MouseAction mouseAction)
    {
        EditorGUIUtility.AddCursorRect(controlRect, mouseCursor);
        if (controlRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
        {
            mouseAction.Invoke(Event.current.mousePosition);
            m_mouseControlAction = WaitforMouseUp;
            m_mouseAction = mouseAction;
            EditorApplication.update += Repaint;
        }
    }

    // Action to invoke while the mouse button is being held down.
    void WaitforMouseUp(Rect controlRect, MouseCursor mouseCursor, MouseAction mouseAction)
    {
        if(Event.current.type == EventType.MouseUp)
        {
            // mouse was released
            m_mouseControlAction = WaitForMouseDown;
            EditorApplication.update -= Repaint;
            return;
        }
        // do action test until mouse release
        // Still have the mouse cursor as the desired mouseCursor
        EditorGUIUtility.AddCursorRect(controlRect, mouseCursor);
        // do the action
        m_mouseAction.Invoke(Event.current.mousePosition);
    }
}
