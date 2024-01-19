using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TUFF
{
    public class ChoicesMenu : MonoBehaviour
    {
        // TODO: Implementation for adding new elements
        public UIMenu uiMenu;
        public RectTransform content;
        public CanvasGroup contentCanvasGroup;
        public List<UIButton> elements = new();

        [System.NonSerialized]
        public EventAction actionCallback = null;
        protected bool initialized = false;
        protected List<string> choices = new();

        private bool m_inUse = false;
        public bool InUse { get => m_inUse; }

        private RectTransform rect { get => transform as RectTransform; }
        private RectTransform parentRect { get => transform.parent as RectTransform; }
        private void Awake()
        {
            Initialize();
        }
        private void Initialize()
        {
            if (initialized) return;
            VerifyMenuArrays(0);
            int index = 0;
            foreach (Transform child in content) // Add existing GameObjects to list
            {
                if (child.TryGetComponent(out UIButton existing))
                {
                    elements.Add(existing);
                    AddToMenu(index, existing);
                }
                index++;
            }
            ToggleAll(false);
            SetupUIMenu();
            if (uiMenu.transitionHandler)
                uiMenu.transitionHandler.onTransitionChange.AddListener(UpdateOnState);
            initialized = true;
        }
        private void InitialValues(EventAction callback, List<string> options)
        {
            m_inUse = true;
            actionCallback = callback;
            choices = options;
            if (choices == null || choices.Count <= 0)
            {
                EndChoices(-1);
                return;
            }
            UpdateElements();
            SetPosition();
            ShowContent(false);
            uiMenu?.OpenMenu();
        }
        private void SetPosition()
        {
            LISAUtility.SetPivot(rect, new Vector2(rect.pivot.x, 0.5f));
            rect.localPosition = new Vector2(0, 0); //Tmp
        }
        private void UpdateElements()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (i >= choices.Count)
                {
                    elements[i].gameObject.SetActive(false);
                    continue;
                }
                UIButton button = elements[i];
                button.text.text = TUFFTextParser.ParseText(choices[i]);
                button.gameObject.SetActive(true);
            }
            SetupUIMenu();
        }
        private void ShowContent(bool show)
        {
            if (!contentCanvasGroup) return;
            contentCanvasGroup.alpha = (show ? 1 : 0);
        }

        public void DisplayChoices(EventAction callback, List<string> options)
        {
            Initialize();
            StartCoroutine(InitialValuesCoroutine(callback, options));
        }
        protected IEnumerator InitialValuesCoroutine(EventAction callback, List<string> options)
        {
            while (m_inUse) yield return null;
            InitialValues(callback, options);
        }
        protected void UpdateOnState(BoxTransitionState state)
        {
            if (state == BoxTransitionState.Visible) ShowContent(true);
        }
        protected void ToggleAll(bool enabled)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].gameObject.SetActive(enabled);
            }
        }
        private void VerifyMenuArrays(int rows)
        {
            if (uiMenu == null) return;
            if (uiMenu.UIElementContainers == null)
            {
                uiMenu.UIElementContainers = new UIElementContainer[rows];
            }
            if (rows >= uiMenu.UIElementContainers.Length) // If row is out of row count
            {
                var newArray = new UIElementContainer[rows + 1];
                System.Array.Copy(uiMenu.UIElementContainers, newArray, uiMenu.UIElementContainers.Length);
                uiMenu.UIElementContainers = newArray;
                Debug.Log("Resize: " + uiMenu.UIElementContainers.Length);
            }
            if (uiMenu.UIElementContainers[0] == null) // Check Index 0
                uiMenu.UIElementContainers[0] = new UIElementContainer();
        }
        protected void SetupUIMenu()
        {
            if (uiMenu == null) return;
            uiMenu.SetupUIElements();
        }
        protected void AddToMenu(int index, UIButton element)
        {
            if (uiMenu == null) return;
            if (index >= uiMenu.UIElementContainers.Length)
            {
                VerifyMenuArrays(index);
            }
            //Debug.Log($"{uiMenu.UIElementContainers.Length} VS {index}");
            if (uiMenu.UIElementContainers[index] == null)
                uiMenu.UIElementContainers[index] = new UIElementContainer();
            uiMenu.UIElementContainers[index].UIElements.Add(element);
            element.onSelect.AddListener(() => PickOption(index));
        }
        private void PickOption(int index)
        {
            Debug.Log($"Selected: {index}");
            StartCoroutine(EndChoices(index));
        }
        private IEnumerator EndChoices(int index)
        {
            ShowContent(false);
            CloseTextbox();
            if (uiMenu.IsOpen) uiMenu.CloseMenu();
            m_inUse = false;

            yield return new WaitForEndOfFrame(); // Little hack so it doesn't automatically skip text when this ends, fix later?
            if (actionCallback is ShowChoicesAction showChoices)
            {
                showChoices.PickOption(index);
            }
            else if (actionCallback != null) actionCallback.isFinished = true;
            yield break;
        }

        private static void CloseTextbox()
        {
            // If open textbox exists.
            if (DialogueManager.openBoxes.Count > 0 && DialogueManager.openBoxes[0])
            {
                DialogueManager.openBoxes[0].CloseTextbox();
            }
        }
    }
}
