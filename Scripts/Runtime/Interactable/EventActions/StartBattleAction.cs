using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class StartBattleAction : EventAction
    {
        [Tooltip("Battle prefab to trigger.")]
        public Battle battle;

        public bool canEscape = false;

        public ActionList winActionList = new();
        public ActionList escapeActionList = new();
        public StartBattleAction()
        {
            eventName = "Start Battle";
            eventColor = EventGUIColors.scene;
        }
        public override void Invoke()
        {
            if (battle != null) BattleManager.instance.InitiateBattle(battle, canEscape, this);
            else
            {
                Debug.LogWarning($"No Battle set for {eventName} event.");
                isFinished = true;
            }
        }
        public void OnBattleEnd(BattleState endBattleState)
        {
            if (!UsesBranches()) { isFinished = true; return; }
            if (endBattleState == BattleState.ESCAPED)
            {
                CommonEventManager.instance.TriggerEventActionBranch(this, escapeActionList);
            }
            else
                CommonEventManager.instance.TriggerEventActionBranch(this, winActionList);
        }
        public bool UsesBranches()
        {
            return canEscape;
        }
    }
}

