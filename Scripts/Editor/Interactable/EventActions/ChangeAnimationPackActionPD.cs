using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ChangeAnimationPackAction))]
    public class ChangeAnimationPackActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            var originType = targetProperty.FindPropertyRelative("originType");
            EditorGUILayout.PropertyField(originType);
            if ((FieldOriginType)originType.enumValueIndex == FieldOriginType.FromScene)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("targetAnimationHandler"));
            }
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("pack"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ChangeAnimationPackAction;

            string origin = (action.targetAnimationHandler != null && action.originType == FieldOriginType.FromScene ?
                action.targetAnimationHandler.name : "(No Handler Set)");
            if (action.originType == FieldOriginType.FromPersistentInstance) { origin = "Follower Instance"; }
            string pack = (action.pack != null ? action.pack.name : "null");

            return $"Set {origin} Animation Pack to {pack}";
        }
    }
}
