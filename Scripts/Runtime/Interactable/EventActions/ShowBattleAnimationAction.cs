using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TUFF
{
    public class ShowBattleAnimationAction : EventAction
    {
        public enum ShowAnimationPositionType { Screen = 0, RelativeToEnemy = 1, RelativeToActiveMemberInParty = 2, RelativeToUnit = 3, }
        [Tooltip("Screen: Displays at the center of the screen." +
            "\nRelativeToEnemy: Display on top of an enemy´s sprite of the given index." +
            "\nRelativeToActiveMemberInParty: Display on top of the party member in the index slot." +
            "\nRelativeToUnit: If the Unit is active in the current battle, display on top of the party member.")]
        public ShowAnimationPositionType animationPositionType = ShowAnimationPositionType.Screen;
        public PartyIndex partyIndex = new();
        public EnemyIndex enemyIndex = new();
        [Tooltip("Reference to the Unit.")]
        public Unit unit;
        public Vector2 offset = new Vector2();
        public bool overridePivot = false;
        public AnimationPivotType newPivot = AnimationPivotType.Center;
        public bool displayWindowsUI = false;
        public BattleAnimation animation = null;

        public ShowBattleAnimationAction()
        {
            eventName = "Show Battle Animation";
            eventColor = EventGUIColors.battle;
        }
        public override void Invoke()
        {
            if (animation == null) { EndEvent(); return; }
            GetPositionAndPivot(out Vector3 pos, out AnimationPivotType pivot);
            GameManager.instance.StartCoroutine(CheckForAnimationEndCoroutine(animation, pos, pivot,displayWindowsUI));
        }

        private void GetPositionAndPivot(out Vector3 pos, out AnimationPivotType pivot)
        {
            pos = Vector3.zero;
            pivot = AnimationPivotType.Screen;
            if (animationPositionType == ShowAnimationPositionType.RelativeToEnemy)
            {
                pivot = animation.targetEnemyPivot;
                EnemyInstance enemyInstance = enemyIndex.GetEnemyInstance();
                if (enemyInstance != null)
                {
                    GraphicHandler handler = enemyInstance.imageReference;
                    if (handler) pos = handler.GetCameraPosition();
                }
            }
            else if (animationPositionType == ShowAnimationPositionType.RelativeToActiveMemberInParty)
            {
                pivot = animation.targetPartyPivot;
                PartyMember member = partyIndex.GetPartyMember();
                if (member != null)
                {
                    GraphicHandler handler = member.imageReference;
                    if (handler) pos = handler.GetCameraPosition();
                }
            }
            else if (animationPositionType == ShowAnimationPositionType.RelativeToUnit)
            {
                pivot = animation.targetPartyPivot;
                PartyMember member = PlayerData.instance.GetPartyMember(unit);
                if (member != null)
                {
                    GraphicHandler handler = member.imageReference;
                    if (handler) pos = handler.GetCameraPosition();
                }
            }
            pos = pos + (Vector3)offset;
            if (overridePivot && animationPositionType != ShowAnimationPositionType.Screen) pivot = newPivot;
        }

        private IEnumerator CheckForAnimationEndCoroutine(BattleAnimation battleAnim, Vector3 pos, AnimationPivotType pivot, bool displayUI)
        {
            if (!battleAnim) yield break;
            BattleManager.instance.hud.QueueShowWindows(displayUI);
            var battleAnimInstance = BattleManager.instance.PlayAnimation(battleAnim, pos, pivot);
            yield return new WaitUntil(() => battleAnimInstance.isFinished);
            BattleManager.instance.hud.QueueShowWindows(true);
            EndEvent();
        }
    }

}
