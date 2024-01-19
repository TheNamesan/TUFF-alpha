using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ShowChoicesAction : EventAction
    {
        public List<string> choices = new();
        public ShowChoicesAction()
        {
            eventName = "Show Choices";
        }
        public override void Invoke()
        {
            UIController.instance.ShowChoices(this, choices);
        }
    }
}
