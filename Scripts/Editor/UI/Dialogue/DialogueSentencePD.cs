using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Localization.Settings;
using UnityEditor.Localization.UI;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(DialogueSentence))]
    public class DialogueSentencePD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lines = 0f;
            float additionalHeight = 0;
            if (property.isExpanded)
            {
                lines = 1f;
                var sentenceTextType = property.FindPropertyRelative("sentenceTextType");
                if ((SentenceTextType)sentenceTextType.enumValueIndex == SentenceTextType.Localized)
                {
                    lines += 2f;
                    additionalHeight = (EditorGUI.GetPropertyHeight(property.FindPropertyRelative("text")) + EditorGUIUtility.standardVerticalSpacing) * 1f;
                    additionalHeight += 6f;
                }
                else if ((SentenceTextType)sentenceTextType.enumValueIndex == SentenceTextType.Simple)
                {
                    lines += 3f;
                    additionalHeight = (EditorGUI.GetPropertyHeight(property.FindPropertyRelative("text")) + EditorGUIUtility.standardVerticalSpacing) * 1f;
                    additionalHeight -= 6f;
                }
                if(property.FindPropertyRelative("voicebank").isExpanded)
                {
                    lines += 3;
                    var overrideColor = property.FindPropertyRelative("overrideTextColor");
                    if (overrideColor.boolValue) lines += 1;
                }
            }
            return (20f) + (20f * lines) + additionalHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            position.height = 20f;
            label = EditorGUI.BeginProperty(position, label, prop);
            prop.isExpanded = EditorGUI.Foldout(position, prop.isExpanded, label, true);
            if (prop.isExpanded)
            {
                Rect rect = EditorGUI.IndentedRect(position);
                AddLine(ref rect);
                var sentenceTextType = prop.FindPropertyRelative("sentenceTextType");
                EditorGUI.PropertyField(rect, sentenceTextType);
                if ((SentenceTextType)sentenceTextType.enumValueIndex == SentenceTextType.Localized)
                {
                    AddLine(ref rect);
                    var key = prop.FindPropertyRelative("key");
                    key.stringValue = EditorGUI.TextField(rect, "Key", key.stringValue);
                    AddLine(ref rect);
                    rect = DrawLocalizationButtons(rect);
                    var localizedText = LISAUtility.GetLocalizedDialogueText(key.stringValue);
                    AddLine(ref rect);
                    rect.height = 50f;
                    GUI.enabled = false;
                    EditorGUI.TextArea(rect, localizedText);
                    GUI.enabled = true;
                    rect.y += 50f;
                    rect.height = 20f;
                }
                else if ((SentenceTextType)sentenceTextType.enumValueIndex == SentenceTextType.Simple)
                {
                    AddLine(ref rect);
                    rect = DrawLocalizationButtons(rect);
                    AddLine(ref rect);
                    var text = prop.FindPropertyRelative("text");
                    float textHeight = EditorGUI.GetPropertyHeight(text) + EditorGUIUtility.standardVerticalSpacing;
                    rect.height = textHeight;
                    float width = rect.width;
                    Rect halfWidthRect = rect;
                    halfWidthRect.width = width * 0.495f;
                    EditorGUI.PropertyField(halfWidthRect, text);

                    // Preview
                    AddLine(ref halfWidthRect);
                    halfWidthRect.height -= 20f;
                    halfWidthRect.x += width * 0.5f;
                    GUI.enabled = false;
                    var parsedText = TUFFTextParser.ParseText(text.stringValue);
                    EditorGUI.TextArea(halfWidthRect, parsedText);
                    GUI.enabled = true;
                    rect.y += textHeight;
                    // End

                    rect.height = 20f;
                }
                //AddLine(ref rect, 2);
                var voicebank = prop.FindPropertyRelative("voicebank");
                voicebank.isExpanded = EditorGUI.Foldout(rect, voicebank.isExpanded, "Override Properties", true);
                AddLine(ref rect, 1);
                if(voicebank.isExpanded)
                {
                    rect.x += 15f;
                    EditorGUI.PropertyField(rect, voicebank);
                    AddLine(ref rect, 1);
                    EditorGUI.PropertyField(rect, prop.FindPropertyRelative("textSpeed"));
                    AddLine(ref rect, 1);
                    var overrideColor = prop.FindPropertyRelative("overrideTextColor");
                    EditorGUI.PropertyField(rect, overrideColor);
                    AddLine(ref rect);
                    if (overrideColor.boolValue)
                    {
                        EditorGUI.PropertyField(rect, prop.FindPropertyRelative("customColor"));
                        AddLine(ref rect);
                    }

                    rect.x -= 15f;
                }
            }
            EditorGUI.EndProperty();
            prop.serializedObject.ApplyModifiedProperties();
        }

        private static Rect DrawLocalizationButtons(Rect rect)
        {
            var locale = LocalizationSettings.SelectedLocale;
            EditorGUI.LabelField(rect, $"Preview Localization [{(locale != null ? locale.LocaleName : "No Active Locale")}]");
            float layoutX = rect.x + rect.width * 0.6f;
            float layoutWidth = rect.width * 0.4f;
            string[] options = new string[LocalizationSettings.AvailableLocales.Locales.Count];
            int[] values = new int[LocalizationSettings.AvailableLocales.Locales.Count];
            for (int i = 0; i < options.Length; i++)
            {
                options[i] = LocalizationSettings.AvailableLocales.Locales[i].LocaleName;
                values[i] = i;
            }
            LISAUtility.CheckLocaleIsNotNull();
            if (LISAUtility.GetSelectedLocaleIndex() >= 0)
                LISAUtility.SelectLocale(EditorGUI.IntPopup(new Rect(layoutX, rect.y, layoutWidth * 0.5f, 20), LISAUtility.GetSelectedLocaleIndex(), options, values));
            if (GUI.Button(new Rect(layoutX + layoutWidth * 0.5f, rect.y, layoutWidth * 0.5f, 20), "Open Tables", EditorStyles.miniButton))
            {
                LocalizationTablesWindow.ShowWindow();
            }

            return rect;
        }

        private void AddLine(ref Rect position, float spaceMult = 1)
        {
            position.y += (20f * spaceMult);
        }
        
    }
}

