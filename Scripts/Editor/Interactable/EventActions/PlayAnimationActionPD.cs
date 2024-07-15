using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(PlayAnimationAction))]
    public class PlayAnimationActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("animator"));

            DrawPopup();
        }
        private void DrawPopup()
        {
            var action = targetObject as PlayAnimationAction;
            if (!action.animator) return;
            if (!action.animator.runtimeAnimatorController)
            { EditorGUILayout.LabelField("No Animator Controller assigned!"); return; }
            var controller = action.animator.runtimeAnimatorController as AnimatorController;

            var animationNameProp = targetProperty.FindPropertyRelative("animationName");

            List<string> names = new();
            List<int> values = new();
            int tmp = 0;
            for (int i = 0; i < controller.layers.Length; i++)
            {
                var states = controller.layers[i].stateMachine.states;
                for (int j = 0; j < states.Length; j++)
                {
                    names.Add(states[j].state.name);
                    values.Add(tmp);
                    tmp++;
                }
            }
            if (names.Count <= 0) return;

            int nameIndex = names.IndexOf(action.animationName);
            if (nameIndex < 0) nameIndex = 0;

            EditorGUI.BeginChangeCheck();
            nameIndex = EditorGUILayout.IntPopup("Animations", nameIndex, names.ToArray(), values.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                //
            }
            animationNameProp.stringValue = names[nameIndex];
            //EditorGUILayout.PropertyField(animationNameProp);
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as PlayAnimationAction;
            if (action.animator == null) return "No Animator set";
            string animationName = action.animationName;
            return $"Play Animation '{animationName}'";
        }
    }
}

