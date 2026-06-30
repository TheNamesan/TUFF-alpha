using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ShowBattleAnimationAction))]
    public class ShowBattleAnimationActionPD : EventActionPD
    {
        private int m_popupValue = 0;
        public override void InspectorGUIContent()
        {
            //EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("enemyIndex"));
            SerializedProperty posTypeProp = targetProperty.FindPropertyRelative(nameof(ShowBattleAnimationAction.animationPositionType));
            EditorGUILayout.PropertyField(posTypeProp);
            var posType = (ShowBattleAnimationAction.ShowAnimationPositionType)posTypeProp.enumValueIndex;
            if (posType == ShowBattleAnimationAction.ShowAnimationPositionType.RelativeToEnemy)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative(nameof(ShowBattleAnimationAction.enemyIndex)));
            }
            else if (posType == ShowBattleAnimationAction.ShowAnimationPositionType.RelativeToActiveMemberInParty)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative(nameof(ShowBattleAnimationAction.partyIndex)));
            }
            else if (posType == ShowBattleAnimationAction.ShowAnimationPositionType.RelativeToUnit)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative(nameof(ShowBattleAnimationAction.unit)));
            }
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative(nameof(ShowBattleAnimationAction.offset)));
            if (posType != ShowBattleAnimationAction.ShowAnimationPositionType.Screen)
            {
                var overridePivotProp = targetProperty.FindPropertyRelative(nameof(ShowBattleAnimationAction.overridePivot));
                EditorGUILayout.PropertyField(overridePivotProp);
                if (overridePivotProp.boolValue)
                {
                    EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative(nameof(ShowBattleAnimationAction.newPivot)));
                }
            }
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative(nameof(ShowBattleAnimationAction.displayWindowsUI)));
            SerializedProperty animation = targetProperty.FindPropertyRelative(nameof(ShowBattleAnimationAction.animation));
            
            DatabaseDropdownDrawer.DrawAnimationsDropdown(ref m_popupValue, animation);
            EditorGUILayout.PropertyField(animation);
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ShowBattleAnimationAction;
            var anim = action.animation;
            ShowBattleAnimationAction.ShowAnimationPositionType posTypeEnum = action.animationPositionType;

            string posType = ObjectNames.NicifyVariableName(posTypeEnum.ToString());
            string animName = (anim != null ? anim.name : "null");

            string target = "";
            if (posTypeEnum == ShowBattleAnimationAction.ShowAnimationPositionType.RelativeToEnemy)
                target = $" [{action.enemyIndex.index}]";
            else if (posTypeEnum == ShowBattleAnimationAction.ShowAnimationPositionType.RelativeToActiveMemberInParty)
                target = $" [{action.partyIndex.index}]";
            else if (posTypeEnum == ShowBattleAnimationAction.ShowAnimationPositionType.RelativeToUnit)
            {
                string unitName = "null";
                if (action.unit != null) unitName = action.unit.GetName();
                target = $" [{unitName}]";
            }
            
            return $"Show Battle Animation: {animName} [{posType}]{target}";
        }
    }

}
