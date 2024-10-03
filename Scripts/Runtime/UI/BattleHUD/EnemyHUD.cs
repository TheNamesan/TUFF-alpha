using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class EnemyHUD : MonoBehaviour
    {
        public ActiveStatesHUD statesHUD;
        public EnemyBarHandler hpBar;
        public EnemyBarHandler spBar;

        [System.NonSerialized] public EnemyInstance target;
        [HideInInspector] public BattleHUD hudCallback;
        private bool m_forceShow = false;
        private string m_format = null;

        private IEnumerator destroyCoroutine = null;
        private const float SECONDSTILDESTROY = 1f;

        public void Show(EnemyInstance target, BattleHUD hudCallback, bool infDisplayTime = false, string format = "F0")
        {
            this.target = target;
            this.hudCallback = hudCallback;
            m_forceShow = infDisplayTime;
            m_format = format;

            var rect = transform as RectTransform;
            rect.position = target.imageReference.GetOverlayPosition();
            rect.anchoredPosition -= Vector2.up * 250f;

            if (destroyCoroutine != null) StopCoroutine(destroyCoroutine);
            destroyCoroutine = WaitToDestroy();

            if (statesHUD)
            {
                if (!statesHUD.initialized) statesHUD.InitializeHUD();
                statesHUD?.UpdateStates(target.states);
            }
            hpBar?.ShowBar(target.HP, target.GetMaxHP(), target.prevHP, infDisplayTime, format, DestroyHUD);
            spBar?.ShowBar(target.SP, target.GetMaxSP(), target.prevSP, infDisplayTime, format);
        }
        public void QuickUpdate()
        {
            if (target == null) return;
            statesHUD?.UpdateStates(target.states);
        }
        private void DestroyHUD()
        {
            if (!m_forceShow) StartCoroutine(destroyCoroutine);
        }
        private IEnumerator WaitToDestroy()
        {
            yield return new WaitForSeconds(SECONDSTILDESTROY);
            hudCallback?.RemoveEnemyHUD(this);
        }
    }
}
