using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class ChangeGameObjectAction : EventAction
    {
        public GameObject gameObject;

        [Header("Active")]
        public bool keepActive = false;
        public bool setActive = false;

        [Header("Name")]
        public bool changeName = false;
        public string newName = "";

        public ChangeGameObjectAction()
        {
            eventName = "Change Game Object";
            eventColor = EventGUIColors.character;
        }
        public override void Invoke()
        {
            if (gameObject)
            {
                if (!keepActive)
                {
                    gameObject.SetActive(setActive);
                }
                if (changeName)
                {
                    gameObject.name = newName;
                }
            }
            isFinished = true;
        }
    }

}
