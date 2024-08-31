using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public static class BattleLogic
    {
        public static bool IsHit(BattleAnimationEvent hitInfo, int targetIndex)
        {
            if (hitInfo.certainHit) return true; //if certainHit, always hit

            float userHitChance = hitInfo.skillOrigin.user.GetHitRate(); //get user's hit rate
            userHitChance *= (hitInfo.successPercent * 0.01f); //Multiply user's hit rate by successPercent
            float targetEvadeChance = hitInfo.skillOrigin.targets[targetIndex].GetEvasionRate(); //get target's evasion
            int hitChance = LISAUtility.Truncate(Mathf.Clamp(userHitChance - targetEvadeChance, 0f, 1f) * 100); //Limit hit between 0 and 100%.
            if (hitChance <= 0) return false; //if hitChance is 0, cant hit.
            //int random = Random.Range(0, 100); 
            //bool trigger = random < hitChance; 
            if (RollChance(hitChance)) return true; //if random number(between 0 and 99) is lower than hitChance, it's a hit
            return false;
        }
        public static bool IsCriticalHit(BattleAnimationEvent hitInfo, int targetIndex, bool ignoreCritEvade = false)
        {
            if (!hitInfo.canCrit) return false;
            float userCritChance = hitInfo.skillOrigin.user.GetCritRate();
            float targetCritEvade = 0;
            if (!ignoreCritEvade) targetCritEvade = hitInfo.skillOrigin.targets[targetIndex].GetCritEvasionRate();
            int critChance = LISAUtility.Truncate(Mathf.Clamp(userCritChance - targetCritEvade, 0f, 1f) * 100);
            if (critChance <= 0) return false;
            //int random = Random.Range(0, 100);
            //bool trigger = random < critChance;
            if (RollChance(critChance)) return true;
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chance"></param>
        /// <param name="randomMinInclusive"></param>
        /// <param name="randomMaxExclusive"></param>
        /// <returns>Returns true if chance was triggered.</returns>
        public static bool RollChance(int chance, int randomMinInclusive = 0, int randomMaxExclusive = 100)
        {
            // If random number (between min and max - 1) is lower than chance, return true
            bool triggeredChance = (Random.Range(randomMinInclusive, randomMaxExclusive) < chance);
            return triggeredChance;
        }
        public static float GetStatChange(List<Feature> features, StatChangeType changeType, float initialValue = 1)
        {
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].featureType != FeatureType.StatChange) continue;
                if (features[i].statChange != changeType) continue;
                initialValue *= features[i].statChangeValue * 0.01f;
            }
            return Mathf.Clamp(initialValue, 0, 10);
        }
        public static float GetExtraRateChange(List<Feature> features, ExtraRateChangeType changeType, float initialValue = 0)
        {
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].featureType != FeatureType.ExtraRateChange) continue;
                if (features[i].extraRateChange != changeType) continue;
                initialValue += features[i].extraRateChangeValue * 0.01f;
            }
            return initialValue;
        }
        public static float GetSpecialRateChange(List<Feature> features, SpecialRateChangeType changeType, float initialValue = 1)
        {
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].featureType != FeatureType.SpecialRateChange) continue;
                if (features[i].specialRateChange != changeType) continue;
                initialValue *= features[i].specialRateChangeValue * 0.01f;
            }
            return Mathf.Clamp(initialValue, 0, 10);
        }
        /// <summary>
        /// Rolls for a state to be applied to a target considering vulnerabilities, potencies and immunities.
        /// </summary>
        /// <param name="state">State to apply</param>
        /// <param name="user">Targetable casting the state. If null, potency of the user will not be considered</param>
        /// <param name="target">Targetable to apply the state to.</param>
        /// <param name="baseTriggerChance">Base percentage chance of state triggering before vulnerabilities and potencies</param>
        /// <returns>Returns true if state was applied.</returns>
        public static bool RollForStateApply(State state, Targetable user, Targetable target, int baseTriggerChance)
        {
            if (state == null) return false;
            if (target == null) return false;
            if (target.IsImmuneToState(state)) return false;
            float totalMultiplier = GetTotalStateVulnerability(state, user, target);
            totalMultiplier *= GetLuckEffectRate(user, target);
            int chance = LISAUtility.Truncate(baseTriggerChance * totalMultiplier);
            Debug.Log("Chance: " + chance);
            if (RollChance(chance)) target.ApplyState(state);
            return true;
        }
        public static List<Targetable> RollForTargets(List<Targetable> validTargets, int numberOfTargets = 1)
        {
            List<Targetable> targets = new List<Targetable>();

            int totalValue = 0;
            int[] targetRates = new int[validTargets.Count];
            for (int i = 0; i < validTargets.Count; i++)
            {
                targetRates[i] = LISAUtility.Truncate(validTargets[i].GetTargetRate() * 100f);
                //Debug.Log(targetRates[i]);
                if (targetRates[i] <= 0) targetRates[i] = 1;
                totalValue += targetRates[i];
            }
            if (numberOfTargets < 0) numberOfTargets = 0;
            int[] targetValue = new int[numberOfTargets];
            for (int i = 0; i < numberOfTargets; i++)
            {
                int minValue = 1;
                int maxValue = 0;
                targetValue[i] = Random.Range(1, totalValue + 1);
                Debug.Log($"Rolled: {targetValue[i]}. Total: {totalValue}");
                for (int j = 0; j < validTargets.Count; j++)
                {
                    maxValue += targetRates[j];
                    if (targetValue[i] >= minValue && targetValue[i] <= maxValue)
                    {
                        targets.Add(validTargets[j]);
                        break;
                    }
                }
            }
            return targets;
        }
        public static List<Targetable> GetDefaultTargets(List<Targetable> validTargets, ScopeData scopeData)
        {
            if (validTargets.Count == 0) return null;
            var scope = scopeData.scopeType;
            if (scope == ScopeType.TheUser) // if TheUser, pick itself
            {
                return new List<Targetable>() { validTargets[0] };
            }
            if (IsSingleScope(scope))
            {
                return RollForTargets(validTargets);
            }
            if (IsGroupScope(scope) || IsRandomScope(scope))
            {
                return validTargets;
            }

            return null;
        }
        /// <summary>
        /// Returns the luck effect bonus based on the user and target's LUK value difference.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static float GetLuckEffectRate(Targetable user, Targetable target)
        {
            if (user == null) return 1f;
            if (target == null) return 1f;
            float value = Mathf.Max(((user.GetLUK() - target.GetLUK()) * 0.001f) + 1f, 0f);
            return value;
        }
        public static bool IsValidTargetFromConditionScope(ConditionScopeType conditionScope, Targetable targetable)
        {
            var isKOd = targetable.isKOd;
            switch (conditionScope)
            {
                case ConditionScopeType.OnlyAlive:
                    if (!isKOd) return true;
                    break;
                case ConditionScopeType.OnlyKO:
                    if (isKOd) return true;
                    break;
                case ConditionScopeType.AliveAndKO:
                    return true;
                default:
                    return false;
            }
            return false;
        }
        public static List<Targetable> CheckValidTargetFromConditionScope(ConditionScopeType conditionScope, Targetable targetable, List<Targetable> targetListToAddTo = null)
        {
            if (targetable == null) return null;
            if (targetListToAddTo == null) targetListToAddTo = new List<Targetable>();
            if (IsValidTargetFromConditionScope(conditionScope, targetable))
                targetListToAddTo.Add(targetable);
            return targetListToAddTo;
        }
        public static float GetTotalElementVulnerability(int elementIndex, Targetable user, Targetable target)
        {
            if (elementIndex < 0) return 1f;
            float potMult = (user != null ? user.GetElementPotency(elementIndex) : 1f);
            float vulMult = target.GetElementVulnerability(elementIndex);
            float totalMult = vulMult * potMult;
            Debug.Log("elementIndex: " + elementIndex + " , Total: " + totalMult);
            return totalMult;
        }
        public static float GetTotalStateVulnerability(State state, Targetable user, Targetable target)
        {
            float potMult = (user != null ? user.GetStatePotency(state) : 1f);
            float vulMult = target.GetStateVulnerability(state);
            return vulMult * potMult;
        }
        public static float GetTotalHitTypeVulnerability(HitType hitType, Targetable user, Targetable target)
        {
            float potMult = hitType == HitType.PhysicalAttack ? user.GetPhysicalDamagePotency() : user.GetSpecialDamagePotency();
            float vulMult = hitType == HitType.PhysicalAttack ? target.GetPhysicalDamageVulnerability() : target.GetSpecialDamageVulnerability();
            return vulMult * potMult;
        }
        public static float GetTotalHealingPotency(Targetable user, Targetable target)
        {
            float dealtMult = user.GetHealingDealtPotency();
            float receivedMult = target.GetHealingReceivedPotency();
            return dealtMult * receivedMult;
        }
        public static float GetElementVulnerability(List<Feature> features, int elementIndex, float initialValue = 1)
        {
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].featureType != FeatureType.ElementVulnerability) continue;
                if (features[i].element != elementIndex) continue;
                initialValue *= features[i].elementValue * 0.01f;
            }
            return Mathf.Clamp(initialValue, 0, 10);
        }
        public static float GetElementPotency(List<Feature> features, int elementIndex, float initialValue = 1)
        {
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].featureType != FeatureType.ElementPotency) continue;
                if (features[i].element != elementIndex) continue;
                initialValue *= features[i].elementValue * 0.01f;
            }
            return Mathf.Clamp(initialValue, 0, 10);
        }
        public static float GetTotalItemPotency(Targetable user, Targetable target)
        {
            float dealtMult = user.GetItemDealtPotency();
            float receivedMult = target.GetItemReceivedPotency();
            return dealtMult * receivedMult;
        }
        public static float GetStateVulnerability(List<Feature> features, State state, float initialValue = 1)
        {
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].featureType != FeatureType.StateVulnerability) continue;
                if (features[i].state != state) continue;
                initialValue *= features[i].stateValue * 0.01f;
            }
            return Mathf.Clamp(initialValue, 0f, 100f);
        }
        public static float GetStatePotency(List<Feature> features, State state, float initialValue = 1)
        {
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].featureType != FeatureType.StatePotency) continue;
                if (features[i].state != state) continue;
                initialValue *= features[i].stateValue * 0.01f;
            }
            return Mathf.Clamp(initialValue, 0f, 100f);
        }
        public static VulnerabilityType GetVulnerabilityType(float multiplier)
        {
            if (multiplier <= 0) return VulnerabilityType.Immune;
            else if (multiplier < 1) return VulnerabilityType.Resist;
            else if (multiplier > 1) return VulnerabilityType.Weakpoint;
            return VulnerabilityType.Normal;
        }
        public static VulnerabilityType GetVulnerabilityTypeFromSkill(IBattleInvocation invocation, Targetable user, Targetable target, ref int elementIndexFound) // Remove out
        {
            var evts = invocation.GetAllEvents();
            for (int i = 0; i < evts.Count; i++)
            {
                if (evts[i].ElementIndex >= 0)
                {
                    elementIndexFound = evts[i].ElementIndex;
                    float mult = GetTotalElementVulnerability(evts[i].ElementIndex, user, target);
                    var vulType = GetVulnerabilityType(mult);
                    if (vulType != VulnerabilityType.Normal)
                        return vulType;
                }
            }
            return VulnerabilityType.Normal;
        }
        public static int GetAttackSpeed(List<Feature> features, int initialValue = 0)
        {
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].featureType != FeatureType.AttackSpeed) continue;
                initialValue += features[i].attackSpeed;
            }
            return initialValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The index of the special feature.</returns>
        public static int GetSpecialFeatureIndex(List<Feature> features, SpecialFeatureType featureType)
        {
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].featureType != FeatureType.SpecialFeature) continue;
                if (features[i].specialFeature == featureType) return i;
            }
            return -1;
        }
        public static bool IsSingleScope(ScopeType scope)
        {
            return scope == ScopeType.OneEnemy || scope == ScopeType.OneAlly || scope == ScopeType.Anyone;
        }
        public static bool IsGroupScope(ScopeType scope)
        {
            return scope == ScopeType.AllEnemies || scope == ScopeType.AllAllies || scope == ScopeType.Everyone;
        }
        public static bool IsRandomScope(ScopeType scope)
        {
            return scope == ScopeType.RandomEnemies || scope == ScopeType.RandomAllies || scope == ScopeType.RandomAnyone;
        }
        public static bool IsEnemyScope(ScopeType scope)
        {
            return scope == ScopeType.OneEnemy || scope == ScopeType.AllEnemies || scope == ScopeType.RandomEnemies;
        }
        public static bool IsAllyScope(ScopeType scope)
        {
            return scope == ScopeType.OneAlly || scope == ScopeType.AllAllies || scope == ScopeType.RandomAllies;
        }
        public static bool IsAnyoneScope(ScopeType scope)
        {
            return scope == ScopeType.Anyone || scope == ScopeType.Everyone || scope == ScopeType.RandomAnyone;
        }
    }
}

