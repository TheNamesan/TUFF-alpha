using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(PlayerData))]
    public class PlayerDataPD : PropertyDrawer
    {
        private static string[] variableNames = new string[] 
            { "playtime", "party", "partyOrder", "inventory", "mags", "battleData",
              "sceneData", "charProperties", "gameVariables", "persistentInteractableIDs"};

        private ReorderableList partyOrderList;
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
                    if (variableNames[i] == "partyOrder") 
                        { position.y += DrawPartyOrder(position, property, prop); continue; }
                    EditorGUI.PropertyField(position, prop);
                    position.y += EditorGUI.GetPropertyHeight(prop) + EditorGUIUtility.standardVerticalSpacing;
                }
                position.x -= 15f;
                position.width += 15f;
            }
            property.serializedObject.ApplyModifiedProperties();
        }

        private float DrawPartyOrder(Rect position, SerializedProperty property, SerializedProperty elements)
        {
            if (partyOrderList == null)
            {
                partyOrderList = new ReorderableList(property.serializedObject, elements, true, false, true, true);
                partyOrderList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => 
                {
                    rect.height = 16f;
                    SerializedProperty prop = partyOrderList.serializedProperty.GetArrayElementAtIndex(index);
                    int id = prop.intValue;
                    int length = DatabaseLoader.units.Length;
                    string[] options = new string[length];
                    int[] values = new int[length];
                    for (int i = 0; i < length; i++)
                    {
                        options[i] = $"{i}: {DatabaseLoader.units[i].GetName()}";
                        values[i] = i;
                    }
                    float totalWidth = rect.width;
                    rect.width *= 0.90f;
                    prop.intValue = EditorGUI.IntPopup(rect, $"Element {index}", id, options, values);
                    rect.x += rect.width;
                    rect.width = totalWidth * 0.1f;
                    prop.intValue = EditorGUI.IntField(rect, prop.intValue);
                };
                partyOrderList.elementHeightCallback = (int i) => { return 18f; };
            }
            elements.isExpanded = EditorGUI.Foldout(position, elements.isExpanded, elements.displayName, true, EditorStyles.foldoutHeader);
            position.y += 20f;
            if (elements.isExpanded) partyOrderList.DoList(position);
            return (elements.isExpanded ? partyOrderList.GetHeight() : 0f) + 20f;
        }
    }
}
