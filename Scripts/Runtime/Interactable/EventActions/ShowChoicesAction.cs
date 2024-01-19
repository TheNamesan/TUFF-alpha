using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ShowChoicesAction : EventAction
    {
        public List<ShowChoicesBranch> choices = new();

        public ShowChoicesAction()
        {
            eventName = "Show Choices";
        }
        public override void Invoke()
        {
            if (choices == null || choices.Count <= 0) { 
                Debug.LogWarning("No choices assigned!"); 
                isFinished = true; 
                return; 
            }

            List<string> texts = new();
            for (int i = 0; i < choices.Count; i++)
            {
                texts.Add(choices[i].choice);
            }
            UIController.instance.ShowChoices(this, texts);
        }
        public void PickOption(int index)
        {
            Debug.Log("Picked option: " + index);
            if (index >= 0 && index < choices.Count)
            {
                CommonEventManager.instance.TriggerEventActionBranch(this, choices[index].actionList);
            }
            else {
                Debug.LogWarning($"Index {index} is out of bounds!");
                isFinished = true;
            }
        }
    }

    [System.Serializable]
    public class ShowChoicesBranch
    {
        public string choice = "";
        public ActionList actionList = new();
        public string ParsedChoiceText()
        {
            return TUFFTextParser.ParseText(choice);
        }
    }

}
