using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TUFF
{
    public class UIButton : UIElement
    {
        public Image fill;

        [Tooltip("If value is more than 0, this button will be selected after holding the Select button for the especified seconds.")]
        public float holdTimeToSelect = 0f;
        private float m_holdTime = 0f;
        private bool m_held = false;

        [Header("OnSelect Actions")]
        [Tooltip("Menus to close when pressing this button.")]
        public List<UIMenu> menusToClose = new List<UIMenu>();
        [Tooltip("Menus to open when pressing this button.")]
        public List<UIMenu> menusToOpen = new List<UIMenu>();

        public override void Highlight()
        {
            ResetHold();
            base.Highlight();
        }
        public override void Unhighlight()
        {
            ResetHold();
            base.Unhighlight();
        }
        public override void Select(InputAction.CallbackContext context)
        {
            if (GameManager.disableUIInput) return;
            if (context.performed)
            {
                if (disabled)
                {
                    PlaySound(DisabledClip());
                    return;
                }
                if (holdTimeToSelect > 0)
                {
                    m_held = true;
                    return;
                }
                OnSelect();
            }
            if (context.canceled)
            {
                ResetHold();
                onSelectCanceled?.Invoke();
            }
        }
        private void Update()
        {
            if (m_held)
            {
                m_holdTime += Time.deltaTime;
                UpdateFill();
                if (m_holdTime >= holdTimeToSelect)
                    OnSelect();
            }
        }

        private void OnSelect()
        {
            PlaySound(SelectClip());
            ResetHold();
            onSelect?.Invoke();
            ChangeMenus();
        }

        private void ChangeMenus()
        {
            CloseMenus();
            OpenMenus();
        }

        private void CloseMenus()
        {
            for(int i = 0; i < menusToClose.Count; i++)
            {
                menusToClose[i]?.CloseMenu();
            }
        }

        private void OpenMenus()
        {
            for (int i = 0; i < menusToOpen.Count; i++)
            {
                menusToOpen[i].OpenMenu();
            }
        }
        private void ResetHold()
        {
            m_holdTime = 0f;
            m_held = false;
            UpdateFill();
        }
        private void UpdateFill()
        {
            if (!fill) return;
            float fillAmount = 0f;
            if (holdTimeToSelect > 0f) fillAmount = m_holdTime / holdTimeToSelect;
            fill.fillAmount = fillAmount;
        }
    }
}
