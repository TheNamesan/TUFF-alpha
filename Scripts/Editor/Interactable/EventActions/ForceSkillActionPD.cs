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
            EditorGUILayout.PropertyField(skillSubjectProp);
            var subjectType = (ForceSkillAction.SkillSubject)skillSubjectProp.enumValueIndex;
            if (subjectType == ForceSkillAction.SkillSubject.Enemy)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative(nameof(ForceSkillAction.enemyIndex)));
            }
            else if (subjectType == ForceSkillAction.SkillSubject.ActivePartyMember)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative(nameof(ForceSkillAction.partyIndex)));
            }
            else if (subjectType == ForceSkillAction.SkillSubject.SpecificPartyMember)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative(nameof(ForceSkillAction.unit)));
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

            string indexString = "";
            if (action.skillSubject == ForceSkillAction.SkillSubject.Enemy) indexString = $"#{(action.enemyIndex.index.ToString())}";
            else if (action.skillSubject == ForceSkillAction.SkillSubject.ActivePartyMember) indexString = $"#{(action.partyIndex.index.ToString())}";
            else if (action.skillSubject == ForceSkillAction.SkillSubject.SpecificPartyMember) indexString = $"({(action.unit == null ? "null" : action.unit.GetName())})";

            return $"Force Skill ({skillName}) on {subjectString} {indexString}";
        }
    }

}
