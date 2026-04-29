using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(PartyIndex))]
    public class PartyIndexPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 20f;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;

            var enemyIdx = property.FindPropertyRelative("index");
            List<PartyMember> members = new List<PartyMember>();
            if (BattleManager.instance != null)
            {
                members.AddRange(BattleManager.instance.activeParty);
            }
            int idx = enemyIdx.intValue;
            if (idx >= 0 && idx < members.Count)
            {
                string name = (members[idx] != null ? members[idx].GetName() : "");
                label.text += $" ({name})";
            }

            EditorGUI.PropertyField(position, enemyIdx, label);

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}

