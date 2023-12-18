using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class CommonEventManager : MonoBehaviour
    {
        [SerializeField] private List<CommonEvent> queuedEvents = new List<CommonEvent>();
        public bool isRunning { get => m_isRunning; }
        private bool m_isRunning = false;
        #region Singleton
        public static CommonEventManager instance;
        private void Awake()
        {
            if (instance != null) Destroy(gameObject);
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }

        }
        #endregion

        public void QueueCommonEvent(CommonEvent commonEvent)
        {
            if (commonEvent == null) return;
            queuedEvents.Add(commonEvent);
        }
        public void RunEvents()
        {
            if (queuedEvents.Count <= 0) return;
            StartCoroutine(UnqueueEvents());
        }
        public IEnumerator UnqueueEvents()
        {
            if (isRunning) yield break;
            m_isRunning = true;
            while (queuedEvents.Count > 0)
            {
                yield return queuedEvents[0].actionList.PlayActions();
                queuedEvents.RemoveAt(0);
            }
            m_isRunning = false;
        }
    }
}
