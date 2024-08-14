using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class OpenSaveMenuAction : EventAction
    {
        [Tooltip("If true, adds two branches that check if the player saved or not.")]
        public bool addBranchesWhenGameIsSaved = false;
        public ActionList savedActionList = new();
        public ActionList unsavedActionList = new();
        public OpenSaveMenuAction()
        {
            eventName = "Open Save Menu";
            eventColor = EventGUIColors.scene;
        }
        public override void Invoke()
        {
            UIController.instance.OpenFileSelectMenu(FileSelectMenuMode.SaveFile, this);
        }
        public override void EndEvent(params object[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is bool saved && addBranchesWhenGameIsSaved)
                {
                    OnFileSaved(saved);
                    return;
                }
            }
            isFinished = true;
        }
        private void OnFileSaved(bool saved)
        {
            if (!addBranchesWhenGameIsSaved) { isFinished = true; return; }
            if (saved)
            {
                CommonEventManager.instance.TriggerEventActionBranch(this, savedActionList);
            }
            else
            {
                CommonEventManager.instance.TriggerEventActionBranch(this, unsavedActionList);
            }
        }
        
    }
}

