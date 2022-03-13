using System.Collections;
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
        public static GUIStyle[] panelStyles { get { return StyleHelpers.panelStyles; } }
        public static Texture2D[] solidTextures { get { return StyleHelpers.solidTextures; } }
    }

    public delegate void EditSourceFunc(GameObject sourceObject, object[] additionalObjects);
    public delegate void EditSourceFunc<T>(T sourceObject, object[] additionalObjects);

    public static class Creators
    {
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
            GUIHelpers.MouseControlRect.instance.ControlRect(window, controlRect, mouseCursor, mouseAction);
        }
    }
}

namespace MysteryEditor.Helpers
{
    public static class StyleHelpers
    {
        private static GUIStyle _previewTextStyle = null;
        private static GUIStyle _blackStyle = null;

        enum PanelIndex
        {
            baseValues,
            growthValues
        }

        private static GUIStyle[] _panelStyles = null;
        private static Texture2D[] _solidTextures = null;

        public static GUIStyle previewTextStyle { get { return _styleGetter.Invoke(ref _previewTextStyle); } }
        public static GUIStyle blackStyle { get { return _styleGetter.Invoke(ref _blackStyle); } }

        public static GUIStyle[] panelStyles { get { return _styleArrayGetter.Invoke(ref _panelStyles); } }
        public static Texture2D[] solidTextures { get { return _textureArrayGetter.Invoke(ref _solidTextures); } }

        delegate T ObjectGetter<T>(ref T obj);
        static ObjectGetter<GUIStyle> _styleGetter = GetFirstObject;
        static ObjectGetter<GUIStyle[]> _styleArrayGetter = GetFirstObject;
        static ObjectGetter<Texture2D[]> _textureArrayGetter = GetFirstObject;


        static void Initialise()
        {            
            _previewTextStyle = new GUIStyle(EditorStyles.label);
            _previewTextStyle.normal.textColor = Color.white;
            _previewTextStyle.alignment = TextAnchor.MiddleCenter;

            _blackStyle = new GUIStyle();
            _blackStyle.normal.background = MakeSolidTexture(2, 2, Color.black);

            InitialisePanelStyles();
        }

        static void InitialisePanelStyles()
        {
            System.Array values = System.Enum.GetValues(typeof(PanelIndex));
            _panelStyles = new GUIStyle[values.Length];
            _solidTextures = new Texture2D[_panelStyles.Length];

            for (int i = 0; i < _panelStyles.Length; i++)
            {
                _panelStyles[i] = new GUIStyle(GUI.skin.button);
            }

            Color colour = Color.green * 0.2f;
            colour.a = 1.0f;
            _solidTextures[0] = MakeSolidTexture(2, 2, colour);
            _panelStyles[0].normal.background = _solidTextures[0];
            _panelStyles[0].active.background = _solidTextures[0];
            _panelStyles[0].hover.background = _solidTextures[0];

            colour = Color.red * 0.2f;
            colour.a = 1.0f;
            _solidTextures[1] = MakeSolidTexture(2, 2, colour);
            _panelStyles[1].normal.background = _solidTextures[1];
            _panelStyles[1].active.background = _solidTextures[1];
            _panelStyles[1].hover.background = _solidTextures[1];
        }

        static T GetFirstObject<T>(ref T obj)
        {
            if (obj == null)
            {
                Initialise();
            }
            _styleGetter = GetObject;
            _styleArrayGetter = GetObject;
            _textureArrayGetter = GetObject;
            return obj;
        }

        static T GetObject<T>(ref T obj)
        {
            return obj;
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
