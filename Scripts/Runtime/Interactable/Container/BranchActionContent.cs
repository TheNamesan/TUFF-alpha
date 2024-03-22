using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class BranchActionContent
    {
        public List<BranchActionContentElement> conditionList = new List<BranchActionContentElement>();
        public bool condition = false;
        
        public ActionList actionList = new ActionList();

        public bool ValidateCondition()
        {
            for (int i = 0; i < conditionList.Count; i++)
            {
                var condition = conditionList[i];
                if (condition == null) continue;
                if (!condition.ValidateCondition())
                    return false;
            }
            return true;
        }
    }
    [System.Serializable]
    public class BranchActionContentElement
    {
        [Tooltip("If true, this branch will trigger when the condtion is not met.")]
        public bool not = false;
        public BranchConditionType conditionType;

        public GameVariableComparator variableComparator;
        public InteractableObject targetInteractable;
        public int targetSwitch;

        public UnitStatusComparator unitComparator;
        public bool ValidateCondition()
        {
            bool valid = false;
            switch (conditionType)
            {
                case BranchConditionType.GameVariable:
                    valid = variableComparator.ValidateGameVariable(); break;
                case BranchConditionType.InteractableSwitch:
                    if (!targetInteractable) valid = true;
                    else valid = targetInteractable.currentSwitch == targetSwitch;
                    break;
                case BranchConditionType.Timer:
                    break;
                case BranchConditionType.Unit:
                    valid = unitComparator.ValidateUnit(); break;//ValidateUnit(targetUnit, unitCondition);
                default: break;

            }
            if (not) valid = !valid;
            return valid;
        }
    }
}