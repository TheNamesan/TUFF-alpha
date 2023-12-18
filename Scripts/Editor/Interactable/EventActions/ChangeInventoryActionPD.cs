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
            string drop = "";

            if (action.dropType == DropType.Item) drop = action.item?.GetName();
            if (action.dropType == DropType.KeyItem) drop = action.keyItem?.GetName();
            if (action.dropType == DropType.Weapon) drop = action.weapon?.GetName();
            if (action.dropType == DropType.Armor) drop = action.armor?.GetName();

            int amount = action.constant;
            return $"{drop} ({action.dropType}) {(amount > 0 ? $"+{amount}" : amount)}";
        }
    }
}

