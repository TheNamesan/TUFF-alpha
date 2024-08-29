using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ActiveState))]
    public class ActiveStatePD : PropertyDrawer
    {
        private static string[] variableNames = new string[] 
        { "m_stateID", "remainingTurns", "startingTurns", "remainingSeconds", "isAutoState" };
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 20f;
            if (property.isExpanded)
            {
                for (int i = 0; i < variableNames.Length; i++)
                {
                    height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative(variableNames[i]))
                        + EditorGUIUtility.standardVerticalSpacing;
                }
            }
            return height;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var array = DatabaseLoader.states;
            var stateID = property.FindPropertyRelative(variableNames[0]);
            string stateName = "Null";
            if (stateID.intValue > 0 && stateID.intValue <= DatabaseLoader.states.Length)
                stateName = array[stateID.intValue].GetName();
            int remainingTurns = property.FindPropertyRelative(variableNames[1]).intValue;

            position.height = 20f;
            label.text += $" ({stateName}. Turns: {remainingTurns})";
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
            DrawIcon(position, stateID, array);
            position.y += 20f;


            if (property.isExpanded)
            {
                position.x += 15f;
                position.width -= 15f;
                for (int i = 0; i < variableNames.Length; i++)
                {
                    var prop = property.FindPropertyRelative(variableNames[i]);
                    if (variableNames[i] == "m_stateID")
                    { DrawStateIDField(position, prop, array); position.y += 20f; continue; }
                    EditorGUI.PropertyField(position, prop);
                    position.y += EditorGUI.GetPropertyHeight(prop) + EditorGUIUtility.standardVerticalSpacing;
                }
                position.x -= 15f;
                position.width += 15f;
            }
            property.serializedObject.ApplyModifiedProperties();
        }
        private void DrawIcon(Rect rect, SerializedProperty property, State[] array)
        {
            Sprite icon = null;
            Rect spriteRect = rect;
            spriteRect.x += EditorGUIUtility.labelWidth - 20f;
            spriteRect.width = 18f; spriteRect.height = 18f;

            if (property.intValue >= 0 && property.intValue < array.Length) icon = array[property.intValue].icon;
            LISAEditorUtility.DrawSprite(spriteRect, icon);
        }
        private void DrawStateIDField(Rect rect, SerializedProperty property, State[] array)
        {
            if (property == null) return;
            GUIContent label = new GUIContent(property.displayName, property.tooltip);

            int length = array.Length + 1;

            GUIContent[] options = new GUIContent[length];
            int[] values = new int[length];

            int index = -1;

            for (int i = 0; i < length; i++)
            {
                if (index < 0) options[0] = new GUIContent("None");
                else options[i] = new GUIContent($"{index}: {array[index].GetName()}");
                values[i] = index;
                index++;
            }
            float totalWidth = rect.width;
            rect.width *= 0.90f;

            property.intValue = EditorGUI.IntPopup(rect, label, property.intValue, options, values);
            rect.x += rect.width;
            rect.width = totalWidth * 0.1f;

            property.intValue = EditorGUI.IntField(rect, property.intValue);
        }
    }
}

