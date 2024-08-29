using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(PartyMember), true)]
    public class PartyMemberPD : PropertyDrawer
    {
        private static string[] variableNames = new string[]
        { "HP", "prevHP", "SP", "TP", "isKOd", "states",
          "m_jobID", "prevExp", "exp", "prevLevel", "level",
          "m_primaryWeaponID", "m_secondaryWeaponID", "m_headID", "m_bodyID", "m_primaryAccessoryID", "m_secondaryAccessoryID",
          "learnedSkills"};
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
                height -= 60f;
            }
            return height;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();
            PartyMember member = LISAEditorUtility.GetTargetObjectOfProperty(property) as PartyMember;

            int index = LISAEditorUtility.GetArrayIndexFromPath(property.propertyPath);

            Unit unit = null;
            if (index >= 0 && index < DatabaseLoader.units.Length)
            {
                unit = DatabaseLoader.units[index];
            }
            if (unit) label.text = $"{index}: {unit.GetName()}";

            position.height = 20f;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
            position.y += 20f;

            if (property.isExpanded)
            {
                position.x += 15f;
                position.width -= 15f;

                float totalWidth = position.width;
                float orgX = position.x;

                DrawSpritePreview(position, member);
                DrawStatusInfo(position, property);
                position.y += (20f * 4f);

                position.width = totalWidth;
                position.x = orgX;

                var statesProp = property.FindPropertyRelative(variableNames[5]);
                EditorGUI.PropertyField(position, statesProp);
                position.y += EditorGUI.GetPropertyHeight(statesProp) + EditorGUIUtility.standardVerticalSpacing;

                DrawJobIDField(position, property.FindPropertyRelative(variableNames[6]), DatabaseLoader.jobs);
                position.y += 20f;

                DrawLevelInfo(position, property);
                position.y += (20f * 2f);

                DrawEquipmentInfo(position, property);
                position.y += (20f * 6f);

                var prop = property.FindPropertyRelative(variableNames[17]);
                EditorGUI.PropertyField(position, prop);
                position.y += EditorGUI.GetPropertyHeight(prop) + EditorGUIUtility.standardVerticalSpacing;

                position.width = totalWidth;
                position.x = orgX;

                position.x -= 15f;
                position.width += 15f;
            }
            property.serializedObject.ApplyModifiedProperties();
        }
        private void DrawSpritePreview(Rect rect, PartyMember member)
        {
            if (member == null) return;
            Sprite sprite = member.GetGraphic();
            if (sprite == null) return;
            rect.x += 10f;
            rect.y += 15f;
            rect.width = sprite.rect.width;
            rect.height = sprite.rect.height;
            LISAEditorUtility.DrawSprite(rect, sprite);
        }
        private void DrawStatusInfo(Rect position, SerializedProperty property)
        {
            position.x += position.width * 0.15f;
            position.width *= 0.85f;
            float orgWidth = position.width;
            float orgX = position.x;
            float orgLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 50f;

            position.width *= 0.5f;
            EditorGUI.PropertyField(position, property.FindPropertyRelative(variableNames[0]));
            position.x += position.width;
            EditorGUI.PropertyField(position, property.FindPropertyRelative(variableNames[1]));
            position.width = orgWidth;
            position.x = orgX;
            position.y += 20f;
            EditorGUI.PropertyField(position, property.FindPropertyRelative(variableNames[2]));
            position.y += 20f;
            EditorGUI.PropertyField(position, property.FindPropertyRelative(variableNames[3]));
            position.y += 20f;
            EditorGUI.PropertyField(position, property.FindPropertyRelative(variableNames[4]), new GUIContent("Is KO'd"));

            EditorGUIUtility.labelWidth = orgLabelWidth;

            //position.width *= 0.5f;
            //EditorGUI.PropertyField(position, property.FindPropertyRelative(variableNames[0]));
            //position.x += position.width;
            //EditorGUI.PropertyField(position, property.FindPropertyRelative(variableNames[1]));
            position.y += 20f;
        }
        private void DrawLevelInfo(Rect position, SerializedProperty property)
        {
            float orgWidth = position.width;
            float orgX = position.x;
            float orgLabelWidth = EditorGUIUtility.labelWidth;
            position.width *= 0.5f;
            EditorGUI.PropertyField(position, property.FindPropertyRelative(variableNames[8]));
            position.x += position.width;
            EditorGUI.PropertyField(position, property.FindPropertyRelative(variableNames[7]));
            position.width = orgWidth;
            position.x = orgX;
            position.y += 20f;
            position.width *= 0.5f;
            EditorGUI.PropertyField(position, property.FindPropertyRelative(variableNames[10]));
            position.x += position.width;
            EditorGUI.PropertyField(position, property.FindPropertyRelative(variableNames[9]));
        }
        private void DrawEquipmentInfo(Rect position, SerializedProperty property)
        {
            DrawEquipIDField(position, property.FindPropertyRelative(variableNames[11]), DatabaseLoader.weapons); position.y += 20f;
            DrawEquipIDField(position, property.FindPropertyRelative(variableNames[12]), DatabaseLoader.weapons); position.y += 20f;
            DrawEquipIDField(position, property.FindPropertyRelative(variableNames[13]), DatabaseLoader.armors); position.y += 20f;
            DrawEquipIDField(position, property.FindPropertyRelative(variableNames[14]), DatabaseLoader.armors); position.y += 20f;
            DrawEquipIDField(position, property.FindPropertyRelative(variableNames[15]), DatabaseLoader.armors); position.y += 20f;
            DrawEquipIDField(position, property.FindPropertyRelative(variableNames[16]), DatabaseLoader.armors); position.y += 20f;
        }
        private void DrawJobIDField(Rect rect, SerializedProperty property, DatabaseElement[] array)
        {
            if (property == null) return;
            GUIContent label = new GUIContent(property.displayName, property.tooltip);

            int length = array.Length;

            GUIContent[] options = new GUIContent[length];
            int[] values = new int[length];

            for (int i = 0; i < length; i++)
            {
                options[i] = new GUIContent($"{i}: {array[i].GetName()}");
                values[i] = i;
            }
            float totalWidth = rect.width;
            rect.width *= 0.90f;
            property.intValue = EditorGUI.IntPopup(rect, label, property.intValue, options, values);
            rect.x += rect.width;
            rect.width = totalWidth * 0.1f;
            property.intValue = EditorGUI.IntField(rect, property.intValue);
        }
        private void DrawEquipIDField(Rect rect, SerializedProperty property, InventoryItem[] array)
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

            Rect spriteRect = rect;
            spriteRect.x += EditorGUIUtility.labelWidth - 20f;
            spriteRect.width = 18f; spriteRect.height = 18f;
            Sprite icon = null;
            if (property.intValue >= 0 && property.intValue < array.Length) icon = array[property.intValue].icon;
            LISAEditorUtility.DrawSprite(spriteRect, icon);

            property.intValue = EditorGUI.IntPopup(rect, label, property.intValue, options, values);
            rect.x += rect.width;
            rect.width = totalWidth * 0.1f;

            property.intValue = EditorGUI.IntField(rect, property.intValue);
        }
    }

}
