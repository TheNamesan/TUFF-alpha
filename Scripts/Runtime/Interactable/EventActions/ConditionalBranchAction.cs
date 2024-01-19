using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ConditionalBranchAction : EventAction
    {
        public List<BranchActionContent> branches = new List<BranchActionContent>();
        public ActionList elseActionList = new();
        public bool addBranchWhenNoConditionsApply = false;
        public ConditionalBranchAction()
        {
            eventName = "Conditional Branch";
            branches = new List<BranchActionContent>();
        }
        public override void Invoke()
        {
            if (branches.Count <= 0)
            {
                isFinished = true;
                Debug.Log("Count is 0");
                return;
            }
            int index = -1;
            for (int i = 0; i < branches.Count; i++)
            {
                if (branches[i].ValidateCondition())
                {
                    index = i;
                    break;
                }
            }
            if (index < 0)
            {
                if (addBranchWhenNoConditionsApply)
                {
                    GameManager.instance.StartCoroutine(TriggerEvents(elseActionList));
                }
                else
                {
                    isFinished = true;
                }
                return;
            }
            GameManager.instance.StartCoroutine(TriggerEvents(branches[index].actionList));
        }

        public IEnumerator TriggerEvents(ActionList actionList)
        {
            if (actionList == null) { Debug.LogWarning("ActionList is null!"); isFinished = false; yield break; }
            actionList.index = 0;
            yield return GameManager.instance.StartCoroutine(actionList.PlayActions());
            actionList.index = 0;
            isFinished = true;
        }
    }
}

