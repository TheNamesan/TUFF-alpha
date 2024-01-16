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
        public List<UIButton> elements = new();
        [System.NonSerialized]
        public EventAction actionCallback = null;
        protected bool initialized = false;
        protected List<string> choices = new();
        private void Awake()
        {
            Initialize();
        }
        private void Initialize()
        {
            if (initialized) return;
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
            HideAll();
            SetupUIMenu();
            initialized = true;
        }
        private void InitialValues(EventAction callback, List<string> options)
        {
            actionCallback = callback;
            choices = options;
            UpdateElements();
            uiMenu?.OpenMenu();
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

        public void DisplayChoices(EventAction callback, List<string> options)
        {
            Initialize();
            InitialValues(callback, options);
        }
        protected void HideAll()
        {
            for (int i = 0; i < elements.Count; i++) // Hide leftover elements
            {
                elements[i].gameObject.SetActive(false);
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
                var newArray = new UIElementContainer[rows];
                System.Array.Copy(uiMenu.UIElementContainers, newArray, uiMenu.UIElementContainers.Length);
                uiMenu.UIElementContainers = newArray;
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
        protected void PickOption(int index)
        {
            Debug.Log($"Selected: {index}");
        }
    }
}
