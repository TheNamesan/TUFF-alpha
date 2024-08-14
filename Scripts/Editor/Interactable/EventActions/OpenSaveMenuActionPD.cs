using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(OpenSaveMenuAction))]
    public class OpenSaveMenuActionPD : EventActionPD
    {
        private List<EventActionPD> m_savedDrawer = new List<EventActionPD>();
        private List<EventActionPD> m_unsavedDrawer = new List<EventActionPD>();
        private static bool queueReset = false;
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("addBranchesWhenGameIsSaved"));
        }
        public override void SummaryGUI(Rect position)
        {
            var action = targetObject as OpenSaveMenuAction;
            if (action == null) { Debug.LogWarning("Object is not OpenSaveMenuAction"); return; }

            EditorGUI.LabelField(position, GetSummaryText());
            position.y += 20f;
            DrawBranches(position);
        }
        private string GetSummaryText()
        {
            return "Open Save Menu";
        }
        public override float GetSummaryHeight()
        {
            var action = targetObject as OpenSaveMenuAction;
            if (action == null) { Debug.LogWarning("Object is not OpenSaveMenuAction"); return 20f; }

            return 20f + GetBranchesHeight();
        }
        // Branches
        private void DrawBranches(Rect position)
        {
            var action = targetObject as OpenSaveMenuAction;
            if (!action.addBranchesWhenGameIsSaved) return;

            if (m_savedDrawer == null || m_unsavedDrawer == null || queueReset)
            {
                ResetEventEditorsList(action);
                if (queueReset) queueReset = false;
            }

            SerializedProperty contentProp = targetProperty.FindPropertyRelative("savedActionList.content");
            string labelText = $"=== If Saved";
            string selectionPanelTitle = $"{action.eventName} If Saved";

            position = EventActionListWindow.DrawBranch(position, targetProperty.propertyPath, m_savedDrawer,
                labelText, selectionPanelTitle,
                contentProp, action.savedActionList);

            contentProp = targetProperty.FindPropertyRelative("unsavedActionList.content");
            labelText = $"=== If Unsaved";
            selectionPanelTitle = $"{action.eventName} If Unsaved";
            position = EventActionListWindow.DrawBranch(position, targetProperty.propertyPath, m_unsavedDrawer,
                labelText, selectionPanelTitle,
                contentProp, action.unsavedActionList);
        }
        private float GetBranchesHeight()
        {
            var action = targetObject as OpenSaveMenuAction;
            if (!action.addBranchesWhenGameIsSaved) return 0f;

            float height = 0;

            height += 20f;
            height += EventActionListWindow.GetListHeight(targetProperty.propertyPath, action.savedActionList.content);
            height += 20f;
            height += EventActionListWindow.GetListHeight(targetProperty.propertyPath, action.unsavedActionList.content);

            return height;
        }
        private void ResetEventEditorsList(OpenSaveMenuAction action)
        {
            m_savedDrawer = new List<EventActionPD>(action.savedActionList.content.Count);
            m_unsavedDrawer = new List<EventActionPD>(action.unsavedActionList.content.Count);
        }
    }
}
