using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NewUnitStats;

[CustomPropertyDrawer(typeof(ResourceStat), true)]
public class ResourceStatDrawer : PropertyDrawer
{
    const string VALUE_MEMBER_NAME = "m_value";
    const string MAX_VALUE_MEMBER_NAME = "m_maxValue";

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect rect = position;
        rect.width *= 0.5f;
        EditorGUI.PropertyField(rect, property.FindPropertyRelative(VALUE_MEMBER_NAME), GUIContent.none, true);
        rect.x += rect.width;
        EditorGUI.PropertyField(rect, property.FindPropertyRelative(MAX_VALUE_MEMBER_NAME), GUIContent.none, true);
    }
}
