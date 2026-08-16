using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TUFF
{
    // TODO
    // Add SpecificPartyMember to TargetType
    // Add LastTarget functionality to TargetType
    public class ForceSkillAction : EventAction
    {
        public enum SkillSubject { Enemy = 0, ActivePartyMember = 1, SpecificPartyMember = 2 }
        public enum TargetType { Random = 0, LastTarget = 1, SpecificIndex = 2 }
        public SkillSubject skillSubject = SkillSubject.Enemy;
        public EnemyIndex enemyIndex = new();
        public PartyIndex partyIndex = new();
        [Tooltip("Reference to the Unit.")]
        public Unit unit;
        [Tooltip("Reference to the Skill.")]
        public Skill skill;
        public TargetType target = TargetType.Random;
        public int targetIndex = 0;

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
            TargetedSkill targetedSkill = GetTargetedSkill(skill, user);
            BattleManager.instance.QueueForcedCommand(targetedSkill);
            BattleManager.instance.RunForcedSkills(this);
        }

        private TargetedSkill GetTargetedSkill(Skill baseSkill, Targetable user)
        {
            List<Targetable> targets = new();
            ScopeData scopeData = skill.scopeData;

            var validTargets = BattleManager.instance.GetInvocationValidTargets(user, scopeData);

            switch (target)
            {
                case TargetType.Random:
                    targets = BattleLogic.GetDefaultTargets(validTargets, scopeData);
                    break;
                case TargetType.LastTarget:
                    // Get last target from user => Use target[0] if CURRENT SKILL is single target, use all if multiple target 
                    break;
                case TargetType.SpecificIndex:
                { 
                    if (BattleLogic.IsSingleScope(scopeData.scopeType))
                    {
                        if (user is PartyMember)
                        {
                            var expectedTarget = BattleManager.instance.GetEnemyInstanceAtIndex(targetIndex);
                            if (validTargets.Contains(expectedTarget)) targets.Add(expectedTarget);
                        }
                        else if (user is EnemyInstance)
                        {
                            var expectedTarget = BattleManager.instance.GetPartyMemberAtIndex(targetIndex);
                            if (validTargets.Contains(expectedTarget)) targets.Add(expectedTarget);
                        }
                    }
                    else
                    {
                        targets = BattleLogic.GetDefaultTargets(validTargets, scopeData);
                    }
                    break;
                }  
            }

            return new TargetedSkill(baseSkill, targets, user);
        }
    }
}
