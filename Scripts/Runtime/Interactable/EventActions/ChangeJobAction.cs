using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ChangeJobAction : EventAction
    {
        [Tooltip("Reference to the Unit.")]
        public Unit unit;
        [Tooltip("Reference to the Job to change to.")]
        public Job job;
        public ChangeJobAction()
        {
            eventName = "Change Job";
            eventColor = EventGUIColors.unit;
        }
        public override void Invoke()
        {
            if (unit != null)
            {
                var member = PlayerData.instance.GetPartyMember(unit);
                member.SetJob(job);
            }
            EndEvent();
        }
    }

}
