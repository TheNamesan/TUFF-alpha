using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(CharacterProperties))]
    public class CharacterPropertiesPD : PropertyDrawer
    {
        private static string[] variableNames = new string[] 
        { "playerPosition", "playerFacing", "disableRun", "disableRopeJump", "disableMenuAccess" };
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
            position.height = 20f;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
            position.y += 20f;

            if (property.isExpanded)
            {
                position.x += 15f;
                position.width -= 15f;
                for (int i = 0; i < variableNames.Length; i++)
                {
                    var prop = property.FindPropertyRelative(variableNames[i]);
                    EditorGUI.PropertyField(position, prop);
                    position.y += EditorGUI.GetPropertyHeight(prop) + EditorGUIUtility.standardVerticalSpacing;
                }
                position.x -= 15f;
                position.width += 15f;
            }
            property.serializedObject.ApplyModifiedProperties();
        }
    }

}
