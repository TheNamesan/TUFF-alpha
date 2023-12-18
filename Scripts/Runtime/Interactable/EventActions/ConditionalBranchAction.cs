using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ConditionalBranchAction : EventAction
    {
        public List<BranchActionContent> branches = new List<BranchActionContent>();
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
                if (branches[i].VerifyCondition())
                {
                    index = i;
                    break;
                }
            }
            if (index < 0)
            {
                isFinished = true;
                return;
            }
            GameManager.instance.StartCoroutine(TriggerEvents(index));
        }

        public IEnumerator TriggerEvents(int index)
        {
            ActionList actionList = branches[index].actionList;
            actionList.index = 0;
            yield return GameManager.instance.StartCoroutine(actionList.PlayActions());
            actionList.index = 0;
            isFinished = true;
        }
    }
}

