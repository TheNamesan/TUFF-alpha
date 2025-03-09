using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ChangeBattleBGMAction : EventAction
    {
        [Tooltip("BGM to play.")]
        public BGMPlayData bgmPlayData;
        public ChangeBattleBGMAction() 
        {
            eventName = "Change Battle BGM";
            eventColor = EventGUIColors.sound;
        }
        public override void Invoke()
        {
            AudioManager.instance.SetBattleBGM(bgmPlayData);
            EndEvent();
        }
    }

}
