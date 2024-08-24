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
                string path = EditorUtility.OpenFilePanel("Select a valid save file", "", "sav");
                if (!string.IsNullOrEmpty(path)) ImportFile(path);
            }
            if (GUILayout.Button("Export File"))
            {
                string path = EditorUtility.SaveFilePanel("Save file as", "", "exportedsave", "sav");
                if (!string.IsNullOrEmpty(path)) ExportFile(path);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(serializedData.FindProperty("data"));
            
            EditorGUILayout.EndScrollView();
            serializedData.ApplyModifiedProperties();
        }
        private void ImportFile(string path)
        {
            PlayerData load = SaveDataConverter.LoadPlayerDataFromPath(path);
            data = load;
            serializedData.Update();
        }
        private void ExportFile(string path)
        {
            SaveDataConverter.SavePlayerDataToPath(data, path);
            serializedData.Update();
        }
    }
}

