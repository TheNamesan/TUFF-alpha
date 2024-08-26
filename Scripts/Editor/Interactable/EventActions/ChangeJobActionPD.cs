using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ChangeJobAction))]
    public class ChangeJobActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("unit"));
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("job"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ChangeJobAction;
            string unitName = (action.unit == null ? "null" : action.unit.GetName());
            string jobName = (action.job == null ? "null" : action.job.GetName());

            return $"Set {unitName}'s job to {jobName}";
        }
    }

}
