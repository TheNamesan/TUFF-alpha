using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(StartBattleAction))]
    public class StartBattleActionPD : EventActionPD
    {
        int popupValue = 0;
        private List<EventActionPD> m_winDrawer = new List<EventActionPD>();
        private List<EventActionPD> m_escapeDrawer = new List<EventActionPD>();
        private List<EventActionPD> m_loseDrawer = new List<EventActionPD>();
        private static bool queueReset = false;
        public override void InspectorGUIContent()
        {
            var battle = targetObject as StartBattleAction;
            var battleTarget = targetProperty.FindPropertyRelative("battle");

            DatabaseDropdownDrawer.DrawBattlesDropdown(ref popupValue, battleTarget);
            GUIContent content = new GUIContent(battleTarget.displayName, battleTarget.tooltip);
            EditorGUILayout.PropertyField(battleTarget);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("canEscape"));
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("continueOnLose"));
        }
        public override void SummaryGUI(Rect position)
        {
            var action = targetObject as StartBattleAction;
            if (action == null) { Debug.LogWarning("Object is not StartBattleAction"); return; }

            EditorGUI.LabelField(position, GetSummaryText());
            position.y += 20f;
            DrawBranches(position);
        }
        private string GetSummaryText()
        {
            var action = targetObject as StartBattleAction;
            if (action.battle == null) return "No Battle set";
            string canEscape = "";
            if (action.canEscape) canEscape = " (Can Escape)";
            return $"Start '{action.battle.name}' Battle{canEscape}";
        }
        public override float GetSummaryHeight()
        {
            var action = targetObject as StartBattleAction;
            if (action == null) { Debug.LogWarning("Object is not StartBattleAction"); return 20f; }

            return 20f + GetBranchesHeight();
        }

        // Branch Stuff
        private void DrawBranches(Rect position)
        {
            var action = targetObject as StartBattleAction;
            if (!action.UsesBranches()) return;

            if (m_winDrawer == null || m_escapeDrawer == null || m_loseDrawer == null || queueReset)
            {
                ResetEventEditorsList(action);
                if (queueReset) queueReset = false;
            }

            
            SerializedProperty contentProp = targetProperty.FindPropertyRelative("winActionList.content");
            string labelText = $"=== If Win";
            string selectionPanelTitle = $"{action.eventName} If Win";

            position = EventActionListWindow.DrawBranch(position, targetProperty.propertyPath, m_winDrawer,
                labelText, selectionPanelTitle,
                contentProp, action.winActionList);
            
            if (action.canEscape)
            {
                labelText = $"=== If Escape";
                selectionPanelTitle = $"{action.eventName} If Escape";
                contentProp = targetProperty.FindPropertyRelative("escapeActionList.content");
                position = EventActionListWindow.DrawBranch(position, targetProperty.propertyPath, m_escapeDrawer,
                    labelText, selectionPanelTitle,
                    contentProp, action.escapeActionList);
            }
            if (action.continueOnLose)
            {
                labelText = $"=== If Lose";
                selectionPanelTitle = $"{action.eventName} If Lose";
                contentProp = targetProperty.FindPropertyRelative("loseActionList.content");
                position = EventActionListWindow.DrawBranch(position, targetProperty.propertyPath, m_loseDrawer,
                    labelText, selectionPanelTitle,
                    contentProp, action.loseActionList);
            }
        }
        private float GetBranchesHeight()
        {
            var action = targetObject as StartBattleAction;
            if (!action.UsesBranches()) return 0f;

            float height = 0;

            height += 20f;
            height += EventActionListWindow.GetListHeight(targetProperty.propertyPath, action.winActionList.content);

            if (action.canEscape)
            {
                height += 20f;
                height += EventActionListWindow.GetListHeight(targetProperty.propertyPath, action.escapeActionList.content);
            }
            if (action.continueOnLose)
            {
                height += 20f;
                height += EventActionListWindow.GetListHeight(targetProperty.propertyPath, action.loseActionList.content);
            }

            return height;
        }

        private void ResetEventEditorsList(StartBattleAction action)
        {
            m_winDrawer = new List<EventActionPD>(action.winActionList.content.Count);
            m_escapeDrawer = new List<EventActionPD>(action.escapeActionList.content.Count);
            m_loseDrawer = new List<EventActionPD>(action.escapeActionList.content.Count);
        }
    }
}

