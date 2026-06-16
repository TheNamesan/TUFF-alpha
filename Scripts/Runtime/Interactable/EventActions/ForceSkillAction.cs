using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TUFF
{
    public class ForceSkillAction : EventAction
    {
        public enum SkillSubject { Enemy = 0, PartyMember = 1 }
        public SkillSubject skillSubject = SkillSubject.Enemy;
        public EnemyIndex enemyIndex;
        [Tooltip("Reference to the Skill.")]
        public Skill skill;
        public ForceSkillAction()
        {
            eventName = "Force Skill";
            eventColor = EventGUIColors.battle;
        }
        public override void Invoke()
        {
            if (skill == null) { Debug.LogWarning("No skill assigned"); EndEvent(); return; }
            if (!BattleManager.instance) { EndEvent(); return; }
            if (!BattleManager.instance.InBattle) { EndEvent(); return; }

            EnemyInstance enemy = enemyIndex.GetEnemyInstance();
            if (enemy == null) { EndEvent(); return; }
            BattleManager.instance.QueueForcedCommand(GetTargetedSkill(skill, enemy));
            BattleManager.instance.RunForcedSkills(this);
        }

        private TargetedSkill GetTargetedSkill(Skill baseSkill, Targetable user)
        {
            List<Targetable> targets = new();
            if (user is EnemyInstance)
            {
                var validTargets = BattleManager.instance.GetInvocationValidTargets(user, skill.scopeData);
                targets = BattleLogic.GetDefaultTargets(validTargets, skill.scopeData); // Tmp, should add target field for ForceSkillAction
            }
            return new TargetedSkill(baseSkill, targets, user);
        }
    }
}
