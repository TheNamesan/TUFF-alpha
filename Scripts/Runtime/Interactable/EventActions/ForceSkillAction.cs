using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TUFF
{
    public class ForceSkillAction : EventAction
    {
        public enum SkillSubject { Enemy = 0, ActivePartyMember = 1, SpecificPartyMember = 2 }
        public SkillSubject skillSubject = SkillSubject.Enemy;
        public EnemyIndex enemyIndex = new();
        public PartyIndex partyIndex = new();
        [Tooltip("Reference to the Unit.")]
        public Unit unit;
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

            Targetable user = null;
            if (skillSubject == SkillSubject.Enemy)
            {
                user = enemyIndex.GetEnemyInstance();
            }
            else if (skillSubject == SkillSubject.ActivePartyMember)
            {
                user = partyIndex.GetPartyMember();
            }
            else if (skillSubject == SkillSubject.SpecificPartyMember)
            {
                PartyMember pm = PlayerData.instance.GetPartyMember(unit);
                if (pm != null && pm.IsInActiveParty()) user = pm;
            }

            if (user == null) { EndEvent(); return; }
            BattleManager.instance.QueueForcedCommand(GetTargetedSkill(skill, user));
            BattleManager.instance.RunForcedSkills(this);
        }

        private TargetedSkill GetTargetedSkill(Skill baseSkill, Targetable user)
        {
            List<Targetable> targets = new();

            var validTargets = BattleManager.instance.GetInvocationValidTargets(user, skill.scopeData);
            targets = BattleLogic.GetDefaultTargets(validTargets, skill.scopeData); // Tmp, should add target field for ForceSkillAction
            
            return new TargetedSkill(baseSkill, targets, user);
        }
    }
}
