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
        private List<EventActionPD> eventListEditorsList;
        private List<EventActionPD> test = new List<EventActionPD>(); // CHANGE, ONLY WORKS WITH 1 BRANCH
        private static bool queueReset = false;
        private void GetList()
        {
            branches = targetProperty.FindPropertyRelative("branches");
            branchesList = new ReorderableList(targetProperty.serializedObject, branches, true, true, true, true);
            branchesList.drawElementCallback = DrawListItems;
            branchesList.drawHeaderCallback = DrawHeader;
            branchesList.elementHeightCallback = GetElementHeight;
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
            EditorGUI.PropertyField(rect, condition);
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
            var eventAction = targetObject as ConditionalBranchAction;
            branches = targetProperty.FindPropertyRelative("branches");
            if (eventListEditorsList == null || queueReset)
            {
                ResetEventEditorsList(eventAction);
                if (queueReset) queueReset = false;
            }
            
            EditorGUI.LabelField(position, "Conditional");
            position.y += 20f;

            GUILayout.BeginVertical();
            for (int i = 0; i < branches.arraySize; i++)
            {
                SerializedProperty elementProp = branches.GetArrayElementAtIndex(i);
                
                var actionListContentProp = elementProp.FindPropertyRelative("actionList.content");

                
                ActionList list = eventAction.branches[i].actionList;
                if (eventListEditorsList[i] == null)
                {
                    eventListEditorsList[i] = new EventActionPD();
                    EventActionListWindow.UpdatePDs(actionListContentProp, list.content, test);
                    //GetEditorsFromEventList(list, eventListEditorsList[i]);
                }
                //EditorGUILayout.BeginVertical("box");
                EditorGUI.LabelField(position, $"Branch #{i}: if =conditional= is {eventAction.branches[i].condition}");
                position.y += 20f;
                position.x += 10;

                position.width -= 10f;
                EventActionListWindow.DisplayEventListContent(position, list.content, test, $"{eventAction.eventName} Branch #{i}", actionListContentProp, targetProperty.propertyPath);
                float height = EventActionListWindow.GetListHeight(targetProperty.propertyPath);
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
            float height = 0;
            branches = targetProperty.FindPropertyRelative("branches");
            if (branches != null)
            {
                for (int i = 0; i < branches.arraySize; i++)
                {
                    //height += EditorGUI.GetPropertyHeight(element);
                    height += 20f;
                    height += EventActionListWindow.GetListHeight(targetProperty.propertyPath);
                    //height += EventActionListWindow.GetDisplayEventListContentHeight();
                }
            }
            return 20f + height;
        }
        private void ResetEventEditorsList(ConditionalBranchAction action)
        {
            eventListEditorsList = new List<EventActionPD>();
            for (int i = 0; i < action.branches.Count; i++)
            {
                eventListEditorsList.Add(null);
            }
        }
    }

}