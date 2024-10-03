using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TUFF
{
    public class EnemyBarHandler : BarHandler
    {
        private const float depleteDuration = 0.5f;
        public Tween tween;
        private float m_currentValue;
        private float m_targetValue;
        private float m_targetMaxValue;
        private string format = null;
        public void ShowBar(float targetValue, float targetMaxValue, float targetPrevValue, bool infDisplayTime = false, string format = "F0", System.Action onFinished = null)
        {
            this.format = format;

            m_targetValue = targetValue;
            m_targetMaxValue = targetMaxValue;
            float prevValue = targetPrevValue;
            
            if (tween != null)
            {
                prevValue = m_currentValue;
                tween?.Kill();
                tween = null;
            }
            if (infDisplayTime)
            {
                OnUpdate(m_targetValue);
            }
            else
            {
                OnUpdate(prevValue);
                tween = DOTween
                    .To(val => OnUpdate(val), prevValue, m_targetValue, depleteDuration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => {
                        onFinished?.Invoke();
                    });
            }    
        }
        private void OnUpdate(float currentValue)
        {
            fill.fillAmount = Mathf.Clamp(currentValue / m_targetMaxValue, 0f, 1f);
            valueText.text = LISAUtility.FloatToString(currentValue, format);
            UpdateFillColor();
            this.m_currentValue = currentValue;
        }
        private void OnDestroy()
        {
            tween?.Kill();
            tween = null;
        }
    }
}
