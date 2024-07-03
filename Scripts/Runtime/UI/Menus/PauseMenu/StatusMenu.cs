using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TUFF
{
    public class StatusMenu : MonoBehaviour
    {
        public DetailedUnitsMenu detailedUnitsMenu;
        public UIMenu uiMenu;
        [System.NonSerialized] public PartyMember selectedMember;

        [Header("Info References")]
        public TMP_Text fullNameText;
        public TMP_Text jobNameText;
        public Image portrait;
        public TMP_Text descriptionText;

        public void OnOpenMenu()
        {
            detailedUnitsMenu.uiMenu = uiMenu;
            detailedUnitsMenu?.UpdateUnits();
            selectedMember = null;
        }
        public void DetailedUnitsOnCreate(UIButton button, PartyMember member)
        {
            button.onHighlight.AddListener(() => { HighlightMember(member); });
            //button.menusToOpen.Add(previewCommandListHUD.uiMenu);
        }
        protected void HighlightMember(PartyMember member)
        {
            selectedMember = member;
            UpdateInfo();
        }
        public void UpdateInfo()
        {
            if (selectedMember == null) return;
            if (fullNameText) fullNameText.text = selectedMember.GetFullName();
            if (jobNameText) jobNameText.text = selectedMember.GetJob().GetName();
            if (portrait) portrait.sprite = selectedMember.GetPortraitSprite();
            if (descriptionText) descriptionText.text = selectedMember.GetJob().GetDescription();
        }
    }
}
