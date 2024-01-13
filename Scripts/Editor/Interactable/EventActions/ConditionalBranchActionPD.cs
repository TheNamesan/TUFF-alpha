using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ConditionalBranchAction))]
    public class ConditionalBranchActionPD : EventActionPD
    {
        private ReorderableList branchesList;
        private SerializedProperty branches;
        private List<EventActionPD> curDrawers = new();
        private List<List<EventActionPD>> m_branchesDrawers = new List<List<EventActionPD>>(); // Class where drawers will be stored for each branch
        private static bool queueReset = false;
        private void GetList()
        {
            branches = targetProperty.FindPropertyRelative("branches");
            branchesList = new ReorderableList(targetProperty.serializedObject, branches, true, true, true, true);
            branchesList.drawElementCallback = DrawListItems;
            branchesList.drawHeaderCallback = DrawHeader;
            branchesList.elementHeightCallback = GetElementHeight;
            branchesList.onChangedCallback = (ReorderableList l) => {
                Debug.Log("Moved in branch");
                EventActionListWindow.ForceResetList();
            };
        }
        public override void InspectorGUIContent()
        {
            if (branchesList == null)
            {
                GetList();
            }
            var eventCommand = targetObject as ConditionalBranchAction;
            EditorGUILayout.LabelField("WIP. Do not use.");
            EditorGUI.BeginChangeCheck();
            branchesList.DoLayoutList();
            
            if (EditorGUI.EndChangeCheck())
            {
                queueReset = true;
            }
        }
        void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.height = 20f;
            SerializedProperty prop = branchesList.serializedProperty.GetArrayElementAtIndex(index);
            var condition = prop.FindPropertyRelative("condition");
            var actionList = prop.FindPropertyRelative("actionList");
            var content = actionList.FindPropertyRelative("content");
            EditorGUI.PropertyField(rect, condition, new GUIContent($"Condition | Event Count: {content.arraySize}"));
            /*if(condition.boolValue)
            {
                AddLine(ref rect);
                EditorGUI.PropertyField(rect, prop.FindPropertyRelative("condition"));
            }*/
        }
        float GetElementHeight(int index)
        {
            var target = (targetObject as ConditionalBranchAction).branches;
            if (target.Count == 0) return 0f;
            var element = branchesList.serializedProperty.GetArrayElementAtIndex(index);
            var elementHeight = EditorGUI.GetPropertyHeight(element);
            /*if (target[index].condition)
            {
                elementHeight += EditorGUI.GetPropertyHeight(element); //same as EditorGUIUtility.singleLineHeight
            }*/
            return elementHeight + EditorGUIUtility.standardVerticalSpacing;
        }
        private void AddLine(ref Rect position)
        {
            position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        }
        void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Branches");
        }
        public override void SummaryGUI(Rect position)
        {
            var conditionalBranchAction = targetObject as ConditionalBranchAction;
            if (conditionalBranchAction == null) { Debug.LogWarning("Object is not Conditional Branch (Summary)"); return; }
            branches = targetProperty.FindPropertyRelative("branches");
            //if (curDrawers == null || curDrawers.Count != conditionalBranchAction.branches.Count || queueReset)
            //{
            //    ResetEventEditorsList(conditionalBranchAction);
            //    if (queueReset) queueReset = false;
            //}
            if (m_branchesDrawers == null || m_branchesDrawers.Count != conditionalBranchAction.branches.Count || queueReset)
            {
                ResetEventEditorsList(conditionalBranchAction);
                if (queueReset) queueReset = false;
            }

            EditorGUI.LabelField(position, "Conditional");
            position.y += 20f;

            GUILayout.BeginVertical();
            for (int i = 0; i < branches.arraySize; i++)
            {
                if (i >= conditionalBranchAction.branches.Count)
                {
                    Debug.Log($"{i}/{conditionalBranchAction.branches.Count}");
                    continue;
                }
                SerializedProperty elementProp = branches.GetArrayElementAtIndex(i);
                var actionListContentProp = elementProp.FindPropertyRelative("actionList.content");
                ActionList list = conditionalBranchAction.branches[i].actionList;

                if (m_branchesDrawers[i] == null || m_branchesDrawers[i].Count != list.content.Count)
                {
                    m_branchesDrawers[i] = new List<EventActionPD>();
                    EventActionListWindow.UpdatePDs(actionListContentProp, list.content, m_branchesDrawers[i]);
                }
                //EditorGUILayout.BeginVertical("box");
                EditorGUI.LabelField(position, $"Branch #{i}: if =conditional= is {conditionalBranchAction.branches[i].condition}");
                position.y += 20f;
                position.x += 10;

                position.width -= 10f;
                EventActionListWindow.DisplayEventListContent(position, list.content, m_branchesDrawers[i], $"{conditionalBranchAction.eventName} Branch #{i}", actionListContentProp, targetProperty.propertyPath);
                float height = EventActionListWindow.GetListHeight(targetProperty.propertyPath, list.content);
                position.y += height;
                //position.y += 200f;//EventActionListWindow.GetDisplayEventListContentHeight();

                position.x -= 10;
                position.width += 10f;
                //EditorGUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }
        public override float GetSummaryHeight()
        {
            if (EventActionListWindow.eventDeleted) { Debug.LogWarning("Event Deleted!"); return 20f; };
            var conditionalBranchAction = targetObject as ConditionalBranchAction;
            if (conditionalBranchAction == null) { Debug.LogWarning("Object is not Conditional Branch"); return 20f; }
            
            float height = 0;
            branches = targetProperty.FindPropertyRelative("branches");
            if (branches != null)
            {
                for (int i = 0; i < branches.arraySize; i++)
                {
                    if (conditionalBranchAction == null) continue;
                    if (conditionalBranchAction.branches == null) continue;
                    if (conditionalBranchAction.branches.Count <= 0) continue;
                    if (conditionalBranchAction.branches[i] == null) continue;
                    ActionList list = conditionalBranchAction.branches[i].actionList;
                    if (list == null) continue;

                    //height += EditorGUI.GetPropertyHeight(element);
                    height += 20f;
                    height += EventActionListWindow.GetListHeight(targetProperty.propertyPath, list.content);
                    //height += EventActionListWindow.GetDisplayEventListContentHeight();
                }
            }
            return 20f + height;
        }
        private void ResetEventEditorsList(ConditionalBranchAction action)
        {
            //curDrawers = new List<EventActionPD>();
            //for (int i = 0; i < action.branches.Count; i++)
            //{
            //    curDrawers.Add(null);
            //}
            m_branchesDrawers.Clear();
            for (int i = 0; i < action.branches.Count; i++)
            {
                m_branchesDrawers.Add(new List<EventActionPD>(action.branches[i].actionList.content.Count));
            }
        }
    }
}
