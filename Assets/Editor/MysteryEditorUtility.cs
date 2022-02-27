﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MysteryEditor.Helpers;

public static class MysteryEditorUtility
{
    public static class Styles
    {
        public static GUIStyle previewTextStyle { get { return StyleHelpers.previewTextStyle; } }
        public static GUIStyle blackStyle { get { return StyleHelpers.blackStyle; } }
    }

    public delegate void EditSourceFunc(GameObject sourceObject, object[] additionalObjects);
    public delegate void EditSourceFunc<T>(T sourceObject, object[] additionalObjects);

    public static class Creators
    {
        public static UnitProfile CreateNewUnitProfile(string unitName)
        {
            if (string.IsNullOrEmpty(unitName))
            {
                unitName = "NewUnit";
            }
            return CreateScriptableObject<UnitProfile>(MysteryEditorSettings.unitProfilesPath + unitName + "_Profile.asset", EditUnitProfile, unitName);
        }

        static void EditUnitProfile<T>(T scriptableSource, object[] additionalObjects) where T : UnitProfile
        {
            UnitProfile.Edit.SetUnitName(scriptableSource, additionalObjects[0] as string);
        }

        public static T CreateScriptableObject<T>(string path) where T : ScriptableObject
        {
            return ObjectCreatorHelpers.CreateScriptableObject<T>(path);
        }

        public static T CreateScriptableObject<T>(string path, EditSourceFunc<T> editFunc, params object[] objects) where T : ScriptableObject
        {
            return ObjectCreatorHelpers.CreateScriptableObject<T>(path, editFunc, objects);
        }

        public static GameObject CreateObjectPrefab(GameObject sourceModelPrefab, string path)
        {
            return ObjectCreatorHelpers.CreateObjectPrefab(sourceModelPrefab, path);
        }

        public static GameObject CreateObjectPrefab(GameObject sourceModelPrefab, string path, EditSourceFunc editFunc, params object[] objects)
        {
            return ObjectCreatorHelpers.CreateObjectPrefab(sourceModelPrefab, path, editFunc, objects);
        }
    }

    public static class GUI
    {
        public delegate void MouseAction(Vector2 mousePosition);

        public static void DrawField(Rect rect, SerializedProperty prop, bool drawChildren = true)
        {
            if (prop != null)
            {
                EditorGUI.PropertyField(rect, prop, drawChildren);
            }
        }

        public static void DrawFieldLayout(SerializedProperty prop, bool drawChildren = true)
        {
            if (prop != null)
            {
                EditorGUILayout.PropertyField(prop, drawChildren);
            }
        }

        // Makes a rect smaller by the control buffer size
        public static Rect RemoveControlBuffer(Rect rect, bool removeWidth = true, bool removeHeight = true)
        {
            Rect result = rect;
            if (removeWidth)
            {
                result.x += MysteryEditorSettings.windowControlSize;
                result.width -= MysteryEditorSettings.windowControlSize * 2;
            }
            if (removeHeight)
            {
                result.y += MysteryEditorSettings.windowControlSize;
                result.height -= MysteryEditorSettings.windowControlSize * 2;
            }
            return result;
        }

        public static void ControlRect(EditorWindow window, Rect controlRect, MouseCursor mouseCursor, MouseAction mouseAction)
        {
            GUIHelpers.MouseControlRect.Instance.ControlRect(window, controlRect, mouseCursor, mouseAction);
        }
    }
}

namespace MysteryEditor.Helpers
{
    public static class StyleHelpers
    {
        private static GUIStyle _previewTextStyle = null;
        private static GUIStyle _blackStyle = null;

        public static GUIStyle previewTextStyle { get { return _styleGetter.Invoke(_previewTextStyle); } }
        public static GUIStyle blackStyle { get { return _styleGetter.Invoke(_blackStyle); } }

        delegate GUIStyle GUIStyleGetter(GUIStyle style);
        static GUIStyleGetter _styleGetter = GetFirstGUIStyle;

        static void Initialise()
        {
            _previewTextStyle = new GUIStyle(EditorStyles.label);
            _previewTextStyle.normal.textColor = Color.white;
            _previewTextStyle.alignment = TextAnchor.MiddleCenter;

            _blackStyle = new GUIStyle();
            _blackStyle.normal.background = MakeSolidTexture(2, 2, Color.black);
        }

        static GUIStyle GetFirstGUIStyle(GUIStyle style)
        {
            if (style == null)
            {
                Initialise();
            }
            _styleGetter = GetGUIStyle;
            return style;
        }

        static GUIStyle GetGUIStyle(GUIStyle style)
        {
            return style;
        }

        public static Texture2D MakeSolidTexture(int width, int height, Color colour)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = colour;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }

    public static class ObjectCreatorHelpers
    {
        public static T CreateScriptableObject<T>(string path) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(path);

            AssetDatabase.CreateAsset(asset, uniquePath);
            AssetDatabase.SaveAssets();

            return asset;
        }

        public static T CreateScriptableObject<T>(string path, MysteryEditorUtility.EditSourceFunc<T> editFunc, params object[] objects) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            editFunc.Invoke(asset, objects);

            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(path);

            AssetDatabase.CreateAsset(asset, uniquePath);
            AssetDatabase.SaveAssets();

            return asset;
        }

        public static GameObject CreateObjectPrefab(GameObject sourceModelPrefab, string path)
        {
            // create copy in scene
            GameObject objSource = PrefabUtility.InstantiatePrefab(sourceModelPrefab) as GameObject;

            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(path);

            // create prefab from scene object
            GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(objSource, uniquePath);

            // destroy scene object
            Object.DestroyImmediate(objSource);

            return newPrefab;
        }

        public static GameObject CreateObjectPrefab(GameObject sourceModelPrefab, string path, MysteryEditorUtility.EditSourceFunc editFunc, params object[] objects)
        {
            // create copy in scene
            GameObject objSource = PrefabUtility.InstantiatePrefab(sourceModelPrefab) as GameObject;

            // add desired components to object in scene
            editFunc.Invoke(objSource, objects);

            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(path);

            // create prefab from scene object
            GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(objSource, uniquePath);

            // destroy scene object
            Object.DestroyImmediate(objSource);

            return newPrefab;
        }
    }

    public static class GUIHelpers
    {
        public class MouseControlRect : SingletonBase<MouseControlRect>
        {
            MysteryEditorUtility.GUI.MouseAction m_mouseAction;

            private delegate void MouseControlAction(EditorWindow window, Rect controlRect, MouseCursor mouseCursor, MysteryEditorUtility.GUI.MouseAction mouseAction);
            MouseControlAction m_mouseControlAction;

            MouseControlRect()
            {
                m_mouseControlAction = WaitForMouseDown;
            }

            // This is a function that will handle mouseInput for the supplied Rect.
            // Example creating an area to for resizing shapes in the window, such as a sidebar
            public void ControlRect(EditorWindow window, Rect controlRect, MouseCursor mouseCursor, MysteryEditorUtility.GUI.MouseAction mouseAction)
            {
                m_mouseControlAction.Invoke(window, controlRect, mouseCursor, mouseAction);
            }

            // Action for when the mouse is hovering over the supplied Rect
            void WaitForMouseDown(EditorWindow window, Rect controlRect, MouseCursor mouseCursor, MysteryEditorUtility.GUI.MouseAction mouseAction)
            {
                EditorGUIUtility.AddCursorRect(controlRect, mouseCursor);
                if (controlRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
                {
                    mouseAction.Invoke(Event.current.mousePosition);
                    m_mouseControlAction = WaitforMouseUp;
                    m_mouseAction = mouseAction;
                    EditorApplication.update += window.Repaint;
                }
            }

            // Action to invoke while the mouse button is being held down.
            void WaitforMouseUp(EditorWindow window, Rect controlRect, MouseCursor mouseCursor, MysteryEditorUtility.GUI.MouseAction mouseAction)
            {
                if (Event.current.type == EventType.MouseUp)
                {
                    // mouse was released
                    m_mouseControlAction = WaitForMouseDown;
                    EditorApplication.update -= window.Repaint;
                    return;
                }
                // do action test until mouse release
                // Still have the mouse cursor as the desired mouseCursor
                EditorGUIUtility.AddCursorRect(controlRect, mouseCursor);
                // do the action
                m_mouseAction.Invoke(Event.current.mousePosition);
            }
        }
    }
}
