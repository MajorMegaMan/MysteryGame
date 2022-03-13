using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MysterySystems.UnitStats;
using MysterySystems.UnitStats.Serialised;

[CustomEditor(typeof(ScriptableUnitProfile))]
public class SerialisationTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Editor Window"))
        {
            UnitStatsEditorWindow.OpenWindow(target);
        }
        base.OnInspectorGUI();
    }
}
