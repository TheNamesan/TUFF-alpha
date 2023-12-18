using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ChangeGameVariableAction : EventAction
    {
        public int variableIndex = 0;
        public GameVariableValueType valueType = GameVariableValueType.BoolValue;
        public bool boolValue = false;
        public float numberValue = 0;
        public string stringValue = "";
        public Vector2 vectorValue = new Vector2();
        public ChangeGameVariableAction()
        {
            eventName = "Change Game Variable";
            eventColor = EventGUIColors.gameProgression;
        }
        public override void Invoke()
        {
            PlayerData.instance.AssignGameVariableValue(variableIndex, GetValue(this));
            isFinished = true;
        }
        public static object GetValue(ChangeGameVariableAction action)
        {
            switch (action.valueType)
            {
                case GameVariableValueType.BoolValue:
                    return action.boolValue;
                case GameVariableValueType.NumberValue:
                    return action.numberValue;
                case GameVariableValueType.StringValue:
                    return action.stringValue;
                case GameVariableValueType.VectorValue:
                    return action.vectorValue;
            }
            return null;
        }
    }
}

