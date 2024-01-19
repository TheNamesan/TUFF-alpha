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
        public void TriggerEventActionBranch(EventAction parentEventAction, ActionList actionList)
        {
            StartCoroutine(TriggerEventActionBranchCoroutine(parentEventAction, actionList));
        }
        private IEnumerator TriggerEventActionBranchCoroutine(EventAction eventAction, ActionList actionList)
        {
            if (eventAction == null) yield break;
            if (actionList == null) { Debug.LogWarning("ActionList is null!"); eventAction.isFinished = false; yield break; }
            actionList.index = 0;
            yield return GameManager.instance.StartCoroutine(actionList.PlayActions());
            actionList.index = 0;
            eventAction.isFinished = true;
        }
    }
}
