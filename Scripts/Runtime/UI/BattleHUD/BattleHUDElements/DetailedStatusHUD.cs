using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

namespace TUFF
{
    public class DetailedStatusHUD : MonoBehaviour
    {
        public TMP_Text nameText;
        public TMP_Text hpText;
        public TMP_Text stateText;
        public ActiveStatesHUDElement activeStateHUDElement;
        public RectTransform detailedTextContent;
        public AdjustToPreferredTextSize adjustToPreferredTextSize;
        public TMP_Text detailedText;

        [Header("Scroll")]
        public float scrollSpeed = 102;
        public float fastScrollSpeedMult = 8;

        public float timeUntilScroll = 1f;
        private float m_scrollTimer = 0;
        public bool IsScrolling { get => m_scrollTimer <= 0; }
        public void ToggleStatus(bool enable)
        {
            gameObject.SetActive(enable);
            if (enable)
            {
                m_scrollTimer = timeUntilScroll;
                SetDetailedTextPosition(0);
            }
        }
        private void Update()
        {
            if (m_scrollTimer > 0)
            {
                m_scrollTimer -= Time.deltaTime * GetSkipMult();
                if (m_scrollTimer < 0) m_scrollTimer = 0;
            }
            if (IsScrolling)
            {
                AddDetailedTextPosition(GetScrollSpeed());
            }
        }
        private float GetScrollSpeed()
        {
            return Time.deltaTime * scrollSpeed * GetSkipMult();
        }
        private float GetSkipMult()
        {
            return (UIController.instance.skipButtonHold ? fastScrollSpeedMult : 1f);
        }
        public void UpdateStatus(ActiveState activeState)
        {
            if (activeState == null) { return; }
            if (activeState.state == null) { Debug.LogWarning("ActiveState has no State!"); return; }
            if (activeState.user == null) { Debug.LogWarning("ActiveState has no user!"); return; }
            var user = activeState.user;
            string HP = $"{(user.CanShowStatus() ? user.HP : "???")}";
            string maxHP = $"{(user.CanShowStatus() ? user.GetMaxHP() : "???")}";
            if (nameText) nameText.text = $"{user.GetName()}";
            if (hpText) hpText.text = $"{TUFFSettings.HPShortText}: {HP}/{maxHP}";
            if (stateText) stateText.text = $"{activeState.state.GetName()}";
            if (activeStateHUDElement) activeStateHUDElement.UpdateStateInfo(activeState);
            if (detailedText) { detailedText.text = GetDetailedText(activeState.state); }
            SetDetailedTextPosition(0);
            m_scrollTimer = timeUntilScroll;
        }
        public string GetDetailedText(State state)
        {
            if (!state) return "ERROR";
            string text = "";
            if (state.useCustomDetailedDescription)
            {
                return state.GetCustomDetailedDescription();
            }
            for (int i = 0; i < state.features.Count; i++)
            {
                bool valid = false;
                var feat = state.features[i];
                string nextText = GetNextText(ref valid, feat);
                text += nextText;
                if (valid && i + 1 < state.features.Count)
                    text += "\n";
            }
            return text;
        }

        private string GetNextText(ref bool valid, Feature feat)
        {
            var featType = feat.featureType;
            string nextText = "";
            float value = 0;
            switch (featType)
            {
                case FeatureType.StatChange:
                    nextText += $"- {TUFFFeatureToText.GetStatChangeText(feat.statChange)} ";
                    value = LISAUtility.PercentageToValue(feat.statChangeValue);
                    nextText += $"x{GetValue(value)}";
                    valid = true;
                    break;
                case FeatureType.ExtraRateChange:
                    nextText += $"- {feat.extraRateChange} "; // Change later
                    value = feat.extraRateChangeValue;
                    nextText += $"{(value >= 0 ? "+" : "")}{GetValue(value)}%";
                    valid = true;
                    break;
                case FeatureType.SpecialRateChange:
                    nextText += $"- {feat.specialRateChange}"; // Change later
                    value = feat.specialRateChangeValue;
                    nextText += $"x{GetValue(value)}%";
                    valid = true;
                    break;
                case FeatureType.ElementPotency:
                    nextText += $"- {feat.featureType}"; // Change later
                    nextText += $" - {TUFFSettings.elements[feat.element].GetName()} ";
                    value = LISAUtility.PercentageToValue(feat.elementValue);
                    nextText += $"x{GetValue(value)}";
                    valid = true;
                    break;
                case FeatureType.ElementVulnerability:
                    nextText += $"- {feat.featureType}"; // Change later
                    nextText += $" - {TUFFSettings.elements[feat.element].GetName()} ";
                    value = LISAUtility.PercentageToValue(feat.elementValue);
                    nextText += $"x{GetValue(value)}";
                    valid = true;
                    break;
                case FeatureType.StatePotency:
                    nextText += $"- {feat.featureType}"; // Change later
                    State targetState = feat.state;
                    if (targetState == null) { nextText = ""; break; }
                    nextText += $" - {targetState.GetName()} ";
                    value = LISAUtility.PercentageToValue(feat.stateValue);
                    nextText += $"x{GetValue(value)}";
                    valid = true;
                    break;
                case FeatureType.StateVulnerability:
                    nextText += $"- {feat.featureType}"; // Change later
                    targetState = feat.state;
                    if (targetState == null) { nextText = ""; break; }
                    nextText += $" - {targetState.GetName()} ";
                    value = LISAUtility.PercentageToValue(feat.stateValue);
                    nextText += $"x{GetValue(value)}";
                    valid = true;
                    break;
                case FeatureType.StateImmunity:
                    nextText += $"- {feat.featureType}"; // Change later
                    targetState = feat.state;
                    if (targetState == null) { nextText = ""; break; }
                    nextText += $" - {targetState.GetName()} ";
                    valid = true;
                    break;
                // Add Hit Element
                // Add State Element
                case FeatureType.AttackSpeed:
                    nextText += $"- {feat.featureType} "; // Change later
                    value = feat.attackSpeed;
                    nextText += $"{(value >= 0 ? "+" : "")}{GetValue(value)}";
                    valid = true;
                    break;
                //case FeatureType.AttackTimes:
                //    valid = true;
                //    break;
            }
            return nextText;
        }

        private string GetValue(float value)
        {
            return LISAUtility.FloatToString(value);
        }

        public void AddDetailedTextPosition(float deltaY)
        {
            if (!detailedText) return;
            RectTransform rectTransform = detailedText.rectTransform;
            if (!rectTransform) return;
            if (adjustToPreferredTextSize) adjustToPreferredTextSize.Adjust();
            //detailedText.ForceMeshUpdate();
            SetDetailedTextPosition(rectTransform.anchoredPosition.y + deltaY);
        }
        public void SetDetailedTextPosition(float posY)
        {
            if (!detailedText) return;
            RectTransform rectTransform = detailedText.rectTransform;
            if (!rectTransform) return;
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, posY);
            CapDetailedTextPosition(rectTransform);
        }
        private void CapDetailedTextPosition(RectTransform rectTransform)
        {
            if (!rectTransform) return;
            if (!detailedTextContent) return;

            float newAnchorPosY = rectTransform.anchoredPosition.y;
            if (newAnchorPosY < 0) newAnchorPosY = 0;
            float max = rectTransform.sizeDelta.y - detailedTextContent.sizeDelta.y;
            if (newAnchorPosY > max) 
            {
                if (max > 0) newAnchorPosY = max;
                else newAnchorPosY = 0;
            }
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, newAnchorPosY);
        }
    }
}
