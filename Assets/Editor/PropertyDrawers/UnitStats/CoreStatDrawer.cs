using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NewUnitStats;

[CustomPropertyDrawer(typeof(CoreStat), true)]
public class CoreStatDrawer : PropertyDrawer
{
    const string VALUE_MEMBER_NAME = "m_value";

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property.FindPropertyRelative(VALUE_MEMBER_NAME), GUIContent.none, true);
    }
}
