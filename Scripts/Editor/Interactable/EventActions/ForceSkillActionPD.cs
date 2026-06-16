using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ForceSkillAction))]
    public class ForceSkillActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(new GUIContent("Subject"), EditorStyles.boldLabel);
            SerializedProperty skillSubjectProp = targetProperty.FindPropertyRelative(nameof(ForceSkillAction.skillSubject));
            var subjectType = (ForceSkillAction.SkillSubject)skillSubjectProp.enumValueIndex;
            if (subjectType == ForceSkillAction.SkillSubject.Enemy)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative(nameof(ForceSkillAction.enemyIndex)));
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(new GUIContent("Skill"), EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative(nameof(ForceSkillAction.skill)));
            EditorGUILayout.EndVertical();
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ForceSkillAction;

            string subjectString = ObjectNames.NicifyVariableName(action.skillSubject.ToString());
            string skillName = (action.skill == null ? "null" : action.skill.GetName());

            string indexString = (action.enemyIndex.index.ToString());

            return $"Force Skill ({skillName}) on {subjectString} #{indexString}";
        }
    }

}
