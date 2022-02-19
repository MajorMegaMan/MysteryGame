using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class UnitEditorHelpers
{
    private static GUIStyle _previewTextStyle = null;
    private static GUIStyle _blackStyle = null;

    public static GUIStyle previewTextStyle { get { return _styleGetter.Invoke(_previewTextStyle); } }
    public static GUIStyle blackStyle { get { return _styleGetter.Invoke(_blackStyle); } }

    delegate GUIStyle GUIStyleGetter(GUIStyle style);
    static GUIStyleGetter _styleGetter = GetFirstGUIStyle;

    public delegate void EditSourceFunc(GameObject sourceObject, object[] additionalObjects);
    public delegate void EditSourceFunc<T>(T sourceObject, object[] additionalObjects);

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
        if(style == null)
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

    public static  T CreateScriptableObject<T>(string path) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        return asset;
    }

    public static T CreateScriptableObject<T>(string path, EditSourceFunc<T> editFunc, params object[] objects) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        editFunc.Invoke(asset, objects);

        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        return asset;
    }

    public static GameObject CreateObjectPrefab(GameObject sourceModelPrefab, string path)
    {
        // create copy in scene
        GameObject objSource = PrefabUtility.InstantiatePrefab(sourceModelPrefab) as GameObject;

        // create prefab from scene object
        GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(objSource, path);

        // destroy scene object
        Object.DestroyImmediate(objSource);

        return newPrefab;
    }

    public static GameObject CreateObjectPrefab(GameObject sourceModelPrefab, string path, EditSourceFunc editFunc, params object[] objects)
    {
        // create copy in scene
        GameObject objSource = PrefabUtility.InstantiatePrefab(sourceModelPrefab) as GameObject;

        // add desired components to object in scene
        editFunc.Invoke(objSource, objects);

        // create prefab from scene object
        GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(objSource, path);

        // destroy scene object
        Object.DestroyImmediate(objSource);

        return newPrefab;
    }
}
