using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    public class SaveEditorWindow : EditorWindow
    {
        public static SaveEditorWindow instance;
        private static readonly Vector2 windowMinSize = new Vector2(100f, 200f);
        private Vector2 scrollPos = new Vector2();

        public PlayerData data = new PlayerData();
        private static SerializedObject serializedData;
        

        [MenuItem("TUFF/Save Editor Window")]
        public static void ShowWindow()
        {
            instance = GetWindow<SaveEditorWindow>("Save Editor");
            instance.AdjustSize();
            
        }
        private void OnEnable()
        {
            serializedData = new SerializedObject(this);
        }
        private void AdjustSize()
        {
            minSize = windowMinSize;
        }
        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(position.height));
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Import File"))
            {
                if (LISAEditorUtility.ImportFileTo(ref data)) serializedData.Update();
            }
            if (GUILayout.Button("Export File"))
            {
                if (LISAEditorUtility.ExportFileTo(data)) serializedData.Update();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(serializedData.FindProperty("data"));
            
            EditorGUILayout.EndScrollView();
            serializedData.ApplyModifiedProperties();
        }
    }
}

