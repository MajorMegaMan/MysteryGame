using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ModelObjectWindow : MysteryWindow
{
    GameObject m_assetObject = null;
    RuntimeAnimatorController m_animController = null;

    // Add menu named "My Window" to the Window menu
    [MenuItem("MysteryEditors/Model Object")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        ModelObjectWindow window = (ModelObjectWindow)EditorWindow.GetWindow(typeof(ModelObjectWindow));
        window.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        Rect previewRect = GUILayoutUtility.GetRect(position.width - MysteryEditorSettings.minDataWidth, position.height);

        EditorGUILayout.BeginVertical();
        m_assetObject = EditorGUILayout.ObjectField("Object", m_assetObject, typeof(GameObject), false) as GameObject;
        m_animController = EditorGUILayout.ObjectField("Animator Controller", m_animController, typeof(RuntimeAnimatorController), false) as RuntimeAnimatorController;

        if (m_assetObject != null)
        {
            PrefabAssetType assetType = PrefabUtility.GetPrefabAssetType(m_assetObject);

            if(assetType == PrefabAssetType.Model)
            {
                if (GUILayout.Button("Create Model Object Asset"))
                {
                    string path = AssetDatabase.GenerateUniqueAssetPath(MysteryEditorSettings.modelObjectsPath + m_assetObject.name + "_ModelObject.prefab");
                    MysteryEditorUtility.Creators.CreateObjectPrefab(m_assetObject, path, EditPrefabSourceFunc, m_animController);
                }
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.HelpBox("Targeted Prefab needs to be a model.", MessageType.Warning);
                EditorGUILayout.EndVertical();
            }
        }
        else
        {
            EditorGUI.BeginDisabledGroup(true);
            GUILayout.Button("Create Model Object Asset");
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }

        ShowModelPreview(previewRect, m_assetObject);

        EditorGUILayout.EndHorizontal();
    }

    void EditPrefabSourceFunc(GameObject objSource, object[] additionalObjects)
    {
        RuntimeAnimatorController animatorController = additionalObjects[0] as RuntimeAnimatorController;

        // add desired components to object in scene
        objSource.AddComponent<ModelObject>();
        Animator objectAnim = objSource.GetComponent<Animator>();
        objectAnim.applyRootMotion = false;
        objectAnim.runtimeAnimatorController = animatorController;
    }
}
