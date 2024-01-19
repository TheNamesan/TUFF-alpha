using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ShowChoicesAction))]
    public class ShowChoicesActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("choices"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ShowChoicesAction;

            return $"Show Choices";
        }
    }
}

