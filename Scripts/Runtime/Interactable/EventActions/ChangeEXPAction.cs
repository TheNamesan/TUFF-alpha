using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class ChangeEXPAction : EventAction
    {
        [Tooltip("Specifies the Unit's HP to change.")]
        public PartyScope scope = PartyScope.EntireParty;
        [Tooltip("Unit to add EXP to.")]
        public Unit unit;
        public AddSetOperationType operation = AddSetOperationType.Add;

        [Header("Operand")]
        [Tooltip("Amount of EXP to change.")]
        public NumberOperand operand = new();

        public ChangeEXPAction()
        {
            eventName = "Change EXP";
            eventColor = EventGUIColors.unit;
        }
        public override void Invoke()
        {
            int value = (int)operand.GetNumber();
            if (scope == PartyScope.EntireParty)
            {
                PlayerData.instance.AddEXPToParty(value);
            }
            else if (scope == PartyScope.OnePartyMember)
            {
                if (unit == null) { isFinished = true; return; }
                var member = PlayerData.instance.GetPartyMember(unit);
                CalculateValue(member, value);
            }
            isFinished = true;
        }
        private void CalculateValue(PartyMember member, int value)
        {
            if (member == null) return;
            
            if (operation == AddSetOperationType.Add)
                member.AddEXP(value);
            if (operation == AddSetOperationType.Set)
                member.SetEXP(value);
        }
    }
}
