using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(Inventory))]
    public class InventoryPD : PropertyDrawer
    {
        private static Texture2D[] itemIcons = null;
        private static Texture2D[] keyItemIcons = null;
        private static Texture2D[] weaponIcons = null;
        private static Texture2D[] armorIcons = null;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
            {
                return (EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing) +
                (EditorGUIUtility.standardVerticalSpacing) + 2f;
            }
            int lines = 4;
            var inventory = LISAEditorUtility.GetTargetObjectOfProperty(property) as Inventory;
            var items = property.FindPropertyRelative("items");
            var keyItems = property.FindPropertyRelative("keyItems");
            var weapons = property.FindPropertyRelative("weapons");
            var armors = property.FindPropertyRelative("armors");
            if (items.isExpanded) lines += items.arraySize;
            if (keyItems.isExpanded) lines += keyItems.arraySize;
            if (weapons.isExpanded) lines += weapons.arraySize;
            if (armors.isExpanded) lines += armors.arraySize;
            return (EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing) +
                ((EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * lines) + 
                (EditorGUIUtility.standardVerticalSpacing) + 2f;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            var orgX = position.x;

            var inventory = LISAEditorUtility.GetTargetObjectOfProperty(property) as Inventory;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true, EditorStyles.foldoutHeader);
            position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            if (property.isExpanded)
            {
                position.x += 15f;
                position.width -= 15f;

                GetIconTextures(property);

                var items = property.FindPropertyRelative("items");
                var orgColor = GUI.color;
                GUI.color = Color.grey;
                GUI.Box(new Rect(position.x - 12, position.y, position.width + 12, 20f + (items.isExpanded ? 20f * items.arraySize : 0f)), "");
                GUI.color = orgColor;

                items.isExpanded = EditorGUI.Foldout(position, items.isExpanded, items.displayName, true);
                position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                if (items.isExpanded)
                {
                    position.x += 15f;
                    position.width -= 15f;
                    for (int i = 0; i < items.arraySize; i++)
                    {
                        DrawIconPreview(position, itemIcons[i]);
                        string name = "???";
                        if (i < DatabaseLoader.items.Length) name = $"{i}: " + DatabaseLoader.items[i].GetName();
                        EditorGUI.PropertyField(position, items.GetArrayElementAtIndex(i), new GUIContent(name));
                        position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                    }
                    position.x -= 15f;
                    position.width += 15f;
                }

                var keyItems = property.FindPropertyRelative("keyItems");
                GUI.color = Color.grey;
                GUI.Box(new Rect(position.x - 12, position.y, position.width + 12, 20f + (keyItems.isExpanded ? 20f * keyItems.arraySize : 0f)), "");
                GUI.color = orgColor;

                keyItems.isExpanded = EditorGUI.Foldout(position, keyItems.isExpanded, keyItems.displayName, true);
                position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                if (keyItems.isExpanded)
                {
                    position.x += 15f;
                    position.width -= 15f;
                    for (int i = 0; i < keyItems.arraySize; i++)
                    {
                        DrawIconPreview(position, keyItemIcons[i]);
                        string name = "???";
                        if (i < DatabaseLoader.keyItems.Length) name = $"{i}: " + DatabaseLoader.keyItems[i].GetName();
                        EditorGUI.PropertyField(position, items.GetArrayElementAtIndex(i), new GUIContent(name));
                        position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                    }
                    position.x -= 15f;
                    position.width += 15f;
                }

                var weapons = property.FindPropertyRelative("weapons");
                GUI.color = Color.grey;
                GUI.Box(new Rect(position.x - 12, position.y, position.width + 12, 20f + (weapons.isExpanded ? 20f * weapons.arraySize : 0f)), "");
                GUI.color = orgColor;

                weapons.isExpanded = EditorGUI.Foldout(position, weapons.isExpanded, weapons.displayName, true);
                position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                if (weapons.isExpanded)
                {
                    position.x += 15f;
                    position.width -= 15f;
                    for (int i = 0; i < weapons.arraySize; i++)
                    {
                        DrawIconPreview(position, weaponIcons[i]);
                        string name = "???";
                        if (i < DatabaseLoader.weapons.Length) name = $"{i}: " + DatabaseLoader.weapons[i].GetName();
                        EditorGUI.PropertyField(position, items.GetArrayElementAtIndex(i), new GUIContent(name));
                        position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                    }
                    position.x -= 15f;
                    position.width += 15f;
                }

                var armors = property.FindPropertyRelative("armors");
                GUI.color = Color.grey;
                GUI.Box(new Rect(position.x - 12, position.y, position.width + 12, 20f + (armors.isExpanded ? 20f * armors.arraySize : 0f)), "");
                GUI.color = orgColor;

                armors.isExpanded = EditorGUI.Foldout(position, armors.isExpanded, armors.displayName, true);
                position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                if (armors.isExpanded)
                {
                    position.x += 15f;
                    position.width -= 15f;
                    for (int i = 0; i < armors.arraySize; i++)
                    {
                        DrawIconPreview(position, armorIcons[i]);
                        string name = "???";
                        if (i < DatabaseLoader.armors.Length) name = $"{i}: " + DatabaseLoader.armors[i].GetName();
                        EditorGUI.PropertyField(position, items.GetArrayElementAtIndex(i), new GUIContent(name));
                        position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                    }
                    position.x -= 15f;
                    position.width += 15f;
                }
                position.width += 15f;
            }
            position.x = orgX;
            property.serializedObject.ApplyModifiedProperties();
        }

        private static void GetIconTextures(SerializedProperty property)
        {
            if (itemIcons != null && keyItemIcons != null && weaponIcons != null && armorIcons != null) return;
            var items = property.FindPropertyRelative("items");
            var keyItems = property.FindPropertyRelative("keyItems");
            var weapons = property.FindPropertyRelative("weapons");
            var armors = property.FindPropertyRelative("armors");
            itemIcons = new Texture2D[items.arraySize];
            keyItemIcons = new Texture2D[keyItems.arraySize];
            weaponIcons = new Texture2D[weapons.arraySize];
            armorIcons = new Texture2D[armors.arraySize];

            for (int i = 0; i < itemIcons.Length; i++)
            {
                if (i >= DatabaseLoader.items.Length) itemIcons[i] = null;
                itemIcons[i] = AssetPreview.GetAssetPreview(DatabaseLoader.items[i].icon);
            }
            for (int i = 0; i < keyItemIcons.Length; i++)
            {
                if (i >= DatabaseLoader.keyItems.Length) keyItemIcons[i] = null;
                keyItemIcons[i] = AssetPreview.GetAssetPreview(DatabaseLoader.keyItems[i].icon);
            }
            for (int i = 0; i < weaponIcons.Length; i++)
            {
                if (i >= DatabaseLoader.weapons.Length) weaponIcons[i] = null;
                weaponIcons[i] = AssetPreview.GetAssetPreview(DatabaseLoader.weapons[i].icon);
            }
            for (int i = 0; i < armorIcons.Length; i++)
            {
                if (i >= DatabaseLoader.armors.Length) armorIcons[i] = null;
                armorIcons[i] = AssetPreview.GetAssetPreview(DatabaseLoader.armors[i].icon);
            }
        }

        private void DrawIconPreview(Rect position, Texture2D sprite)
        {
            var preview = sprite;
            if (preview != null) GUI.DrawTexture(new Rect(position.x - 2, position.y + 2, 18f, 18f), preview);
        }
    }
}

