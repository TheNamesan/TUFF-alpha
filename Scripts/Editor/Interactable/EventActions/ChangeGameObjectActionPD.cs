using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ChangeGameObjectAction))]
    public class ChangeGameObjectActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            var transform = targetProperty.FindPropertyRelative("gameObject");
            EditorGUILayout.PropertyField(transform);

            // Active
            var keepActive = targetProperty.FindPropertyRelative("keepActive");
            EditorGUILayout.PropertyField(keepActive);
            if (!keepActive.boolValue)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("setActive"));
            }
            // Rotation
            var changeName = targetProperty.FindPropertyRelative("changeName");
            EditorGUILayout.PropertyField(changeName);
            if (changeName.boolValue)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("newName"));
            }
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ChangeGameObjectAction;
            if (action.gameObject == null) return "No Game Object set";
            string name = action.gameObject.name;
            string active = "";
            if (!action.keepActive)
            {
                active = $"[Active: {action.setActive}]";
            }
            string newName = "";
            if (action.changeName)
            {
                active = $"[Name: {action.newName}]";
            }
            return $"Set {name}: {active}{newName}";
        }
    }
}
