using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnitProfileSet))]
public class UnitProfileSetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Open Editor Window"))
        {
            UnitStatsEditorWindow.OpenWindow(target as UnitProfileSet, null);
        }
    }

    public static ScriptableUnitProfile CreateUnitProfile(string profileName)
    {
        return MysteryEditorSettings.currentProfileSet.CreateUnitProfile(profileName);
    }

    public static void RemoveUnitProfile(ScriptableUnitProfile target)
    {
        MysteryEditorSettings.currentProfileSet.DeleteUnitProfile(target);
    }
}

public static class UnitProfileSetExtensions
{
    public static ScriptableUnitProfile CreateUnitProfile(this UnitProfileSet unitProfileSet, string profileName)
    {
        ScriptableUnitProfile item = MysteryEditorUtility.Creators.CreateScriptableObject<ScriptableUnitProfile>(MysteryEditorSettings.unitProfilesPath + profileName + ".asset");
        item.Init();
        EditorUtility.SetDirty(item);
        unitProfileSet.AddUnitProfile(item);
        EditorUtility.SetDirty(unitProfileSet);
        AssetDatabase.SaveAssets();
        return item;
    }

    // Removes target and saves the asset
    public static void DeleteUnitProfile(this UnitProfileSet unitProfileSet, ScriptableUnitProfile target)
    {
        unitProfileSet.RemoveUnitProfile(target);
        EditorUtility.SetDirty(unitProfileSet);
        AssetDatabase.SaveAssets();
    }
}

class ProfileAssetModificationProcessor : UnityEditor.AssetModificationProcessor
{
    static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions opt)
    {
        System.Type assetType = AssetDatabase.GetMainAssetTypeAtPath(path);
        if (assetType == typeof(ScriptableUnitProfile))
        {
            UnitProfileSetEditor.RemoveUnitProfile(AssetDatabase.LoadAssetAtPath<ScriptableUnitProfile>(path));
        }
        else if (assetType == typeof(UnitProfileSet))
        {
            //UnitProfileSet.CleanInstance();
        }
        return AssetDeleteResult.DidNotDelete;
    }
}
