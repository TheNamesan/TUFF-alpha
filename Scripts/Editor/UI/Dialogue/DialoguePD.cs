using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(Dialogue))]
    public class DialoguePD : PropertyDrawer
    {
        int lines = 0;
        float additionalHeight = 0;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (20f) +
                (EditorGUIUtility.singleLineHeight * lines) + 
                (EditorGUIUtility.standardVerticalSpacing) + additionalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var target = LISAEditorUtility.GetTargetObjectOfProperty(prop) as Dialogue;
            lines = 0;
            additionalHeight = 0;
            label = EditorGUI.BeginProperty(position, label, prop);
            position.height = 20f;
            prop.isExpanded = EditorGUI.Foldout(position, prop.isExpanded, label, true);
            if (prop.isExpanded)
            {
                EditorGUI.indentLevel++;
                Rect rect = EditorGUI.IndentedRect(position);
                AddLine(ref rect);
                var textboxType = prop.FindPropertyRelative("textboxType");
                EditorGUI.PropertyField(rect, textboxType);

                if ((TextboxType)textboxType.enumValueIndex == TextboxType.Normal)
                {
                    var originType = prop.FindPropertyRelative("originType");
                    AddLine(ref rect);
                
                    EditorGUI.PropertyField(rect, originType);
                
                    if ((FieldOriginType)originType.enumValueIndex == FieldOriginType.FromScene)
                    {
                        var origin = prop.FindPropertyRelative("origin");
                        AddLine(ref rect);
                        EditorGUI.PropertyField(rect, origin);
                        if (LISAUtility.IsPersistentInstance(target.origin))
                        {
                            target.originType = FieldOriginType.FromPersistentInstance;
                            target.persistentOrigin = LISAUtility.GetPersistentOriginType(target.origin);
                            target.origin = null;
                            Debug.LogWarning("Dialogue Origin is a Persistent Instance, changing OriginType to Persistent.");
                        }
                    }
                    else if((FieldOriginType)originType.enumValueIndex == FieldOriginType.FromPersistentInstance)
                    {
                        AddLine(ref rect);
                        EditorGUI.PropertyField(rect, prop.FindPropertyRelative("persistentOrigin"));
                    }
                    AddLine(ref rect);
                    EditorGUI.PropertyField(rect, prop.FindPropertyRelative("positionOffset"));
                    AddLine(ref rect);
                }
                else if ((TextboxType)textboxType.enumValueIndex == TextboxType.Fixed)
                {
                    AddLine(ref rect);
                    EditorGUI.PropertyField(rect, prop.FindPropertyRelative("fixedTextboxPosition"));
                }
                
                AddLine(ref rect);
                var overrideTextbox = prop.FindPropertyRelative("overrideTextbox");
                EditorGUI.PropertyField(rect, overrideTextbox);
                if (overrideTextbox.boolValue)
                {
                    AddLine(ref rect);
                    var customTextbox = prop.FindPropertyRelative("customTextbox");
                    customTextbox.objectReferenceValue = 
                        (GameObject)EditorGUI.ObjectField(rect, "Custom Textbox", customTextbox.objectReferenceValue, typeof(GameObject), false);
                }
                AddLine(ref rect);
                EditorGUI.PropertyField(rect, prop.FindPropertyRelative("baseVoicebank"));
                AddLine(ref rect);
                EditorGUI.PropertyField(rect, prop.FindPropertyRelative("baseTextSpeed"));
                AddLine(ref rect);
                var overrideColor = prop.FindPropertyRelative("overrideTextColor");
                EditorGUI.PropertyField(rect, overrideColor);
                AddLine(ref rect);
                if (overrideColor.boolValue)
                {
                    EditorGUI.PropertyField(rect, prop.FindPropertyRelative("customColor"));
                    AddLine(ref rect);
                }


                    var sentences = prop.FindPropertyRelative("sentences");
                EditorGUI.PropertyField(rect, sentences);
                float sentenceHeight = EditorGUI.GetPropertyHeight(sentences, true) + EditorGUIUtility.standardVerticalSpacing;
                additionalHeight += sentenceHeight;
                rect.y += sentenceHeight;

                var onDialogueEnd = prop.FindPropertyRelative("onDialogueEnd");
                EditorGUI.PropertyField(rect, onDialogueEnd);
                float endHeight = EditorGUI.GetPropertyHeight(onDialogueEnd, true);
                additionalHeight += endHeight + EditorGUIUtility.standardVerticalSpacing;
                rect.y += endHeight;
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndProperty();
            prop.serializedObject.ApplyModifiedProperties();
        }

        private void AddLine(ref Rect position, float spaceMult = 1)
        {
            lines++;
            position.y += (20f * spaceMult);
        }
    }
}
