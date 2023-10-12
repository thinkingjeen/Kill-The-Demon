using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MobileConsole.Editor
{
    [CustomEditor(typeof(MobileConsole))]
    public class MobileConsoleEditor : UnityEditor.Editor
    {
        private SerializedProperty skin;
        private SerializedProperty showOnError;
        private SerializedProperty tapCorner;
        private SerializedProperty disableInReleaseBuild;
        private SerializedProperty maxLogEntries;
        
        void OnEnable()
        {
            skin = serializedObject.FindProperty("skin");
            showOnError = serializedObject.FindProperty("showOnError");
            tapCorner = serializedObject.FindProperty("tapCorner");
            disableInReleaseBuild = serializedObject.FindProperty("disableInReleaseBuild");
            maxLogEntries = serializedObject.FindProperty("maxLogEntries");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(skin);
            EditorGUILayout.PropertyField(showOnError);
            EditorGUILayout.PropertyField(tapCorner);
            EditorGUILayout.PropertyField(disableInReleaseBuild);
            EditorGUILayout.PropertyField(maxLogEntries);
            
            serializedObject.ApplyModifiedProperties();
            
            if (GUI.changed)
            {
                var mobileConsole = (MobileConsole) target;
                EditorUtility.SetDirty(mobileConsole);
                EditorSceneManager.MarkSceneDirty(mobileConsole.gameObject.scene);
            }
        }
    }
}