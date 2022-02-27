using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;


[CustomEditor(typeof(UnitProfile))]
public class UnitProfileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Open Editor Window"))
        {
            UnitProfileEditorWindow.OpenWindow(target as UnitProfile);
        }
    }
}

public class AssetHandler
{
    [OnOpenAsset()]
    public static bool OpenEditor(int insatnceId, int line)
    {
        UnitProfile obj = EditorUtility.InstanceIDToObject(insatnceId) as UnitProfile;
        if(obj != null)
        {
            UnitProfileEditorWindow.OpenWindow(obj);
            return true;
        }
        return false;
    }
}