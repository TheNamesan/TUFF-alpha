using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ChangeInventoryAction))]
    public class ChangeInventoryActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            var dropType = targetProperty.FindPropertyRelative("dropType");
            EditorGUILayout.PropertyField(dropType);

            if (dropType.enumValueIndex == (int)DropType.Item)
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("item"));
            if (dropType.enumValueIndex == (int)DropType.KeyItem)
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("keyItem"));
            if (dropType.enumValueIndex == (int)DropType.Weapon)
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("weapon"));
            if (dropType.enumValueIndex == (int)DropType.Armor)
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("armor"));

            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("constant"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ChangeInventoryAction;
            string drop = "null";
            InventoryItem invItem = null;
            if (action.dropType == DropType.Item) invItem = action.item;
            if (action.dropType == DropType.KeyItem) invItem = action.keyItem;
            if (action.dropType == DropType.Weapon) invItem = action.weapon;
            if (action.dropType == DropType.Armor) invItem = action.armor;
            if (invItem == null) drop = "null";
            else drop = invItem.GetName();

            int amount = action.constant;
            return $"{drop} ({action.dropType}) {(amount >= 0 ? $"+{amount}" : amount)}";
        }
    }
}

