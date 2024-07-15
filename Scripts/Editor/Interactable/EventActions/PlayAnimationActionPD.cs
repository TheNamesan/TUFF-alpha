using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(PlayAnimationAction))]
    public class PlayAnimationActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("animator"));
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("animationName"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as PlayAnimationAction;
            if (action.animator == null) return "No Animator set";
            string animationName = action.animationName;
            return $"Play Animation '{animationName}'";
        }
    }
}

