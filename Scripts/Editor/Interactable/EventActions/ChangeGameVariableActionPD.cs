using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ChangeGameVariableAction))]
    public class ChangeGameVariableActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            var variableIndex = targetProperty.FindPropertyRelative("variableIndex");
            var variablesData = GameVariableList.GetList();
            string[] options = new string[variablesData.Length];
            int[] values = new int[variablesData.Length];
            for (int i = 0; i < variablesData.Length; i++)
            {
                options[i] = variablesData[i].name;
                values[i] = i;
            }
            variableIndex.intValue = EditorGUILayout.IntPopup(variableIndex.displayName, variableIndex.intValue, options, values);
            var valueType = targetProperty.FindPropertyRelative("valueType");
            EditorGUILayout.PropertyField(valueType);
            DrawOnType(valueType);

        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ChangeGameVariableAction;
            var list = GameVariableList.GetList();
            string name = "null";
            int variableIndex = action.variableIndex;
            if (variableIndex >= 0 && variableIndex < list.Length)
                name = list[variableIndex].name;
            string valueType = action.valueType.ToString();
            string value = ChangeGameVariableAction.GetValue(action).ToString();

            return $"Change Variable '{name}' {valueType} to '{value}'";
        }
        private void DrawOnType(SerializedProperty valueType)
        {
            var valueEnum = (GameVariableValueType)valueType.enumValueIndex;
            switch (valueEnum)
            {
                case GameVariableValueType.BoolValue:
                    EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("boolValue")); break;
                case GameVariableValueType.NumberValue:
                    EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("numberValue")); break;
                case GameVariableValueType.StringValue:
                    EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("stringValue")); break;
                case GameVariableValueType.VectorValue:
                    EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("vectorValue")); break;
            }
        }
    }
}
