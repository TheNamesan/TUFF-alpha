using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ChangeBattleBGMAction))]
    public class ChangeBattleBGMActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative(nameof(ChangeBattleBGMAction.bgmPlayData)), new GUIContent("BGM Play Data"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }

        private string GetSummaryText()
        {
            var action = targetObject as ChangeBattleBGMAction;
            if (action.bgmPlayData == null) return "No BGM set";
            if (action.bgmPlayData.bgm == null) return "No BGM set";
            //float dur = action.fadeInDuration;
            return $"Change Battle BGM to '{action.bgmPlayData.bgm.songName}', at Volume {action.bgmPlayData.volume}, Pitch {action.bgmPlayData.pitch}";
                //$"{(dur > 0 ? $" with {dur} second{(dur == 1 ? "" : "s")} Fade In" : "")}";
        }
    }
}

