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
        public DetailedUnitHUD selectedDetailedUnitHUD;
        public MemberEquipmentMenu memberEquipment;
        public MemberBioHUD memberBioHUD;
        public UIMenu uiMenu;
        [System.NonSerialized] public PartyMember selectedMember;

        [Header("Info References")]
        public TMP_Text fullNameText;
        public TMP_Text jobNameText;
        public Image portrait;

        [Header("Stats")]
        public StatChangeElement ATKElement;
        public StatChangeElement DEFElement;
        public StatChangeElement SATKElement;
        public StatChangeElement SDEFElement;
        public StatChangeElement AGIElement;
        public StatChangeElement LUCKElement;

        public void OnOpenMenu()
        {
            detailedUnitsMenu.uiMenu = uiMenu;
            detailedUnitsMenu?.UpdateUnits();
            selectedMember = null;
            UpdateLabels();
            memberBioHUD?.UpdateLabels();
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
            if (selectedDetailedUnitHUD)
                selectedDetailedUnitHUD.UpdateInfo(selectedMember, true);
            if (memberEquipment) memberEquipment.UpdateInfo(selectedMember);

            if (ATKElement) ATKElement.UpdateInfo(selectedMember.GetATK());
            if (DEFElement) DEFElement.UpdateInfo(selectedMember.GetDEF());
            if (SATKElement) SATKElement.UpdateInfo(selectedMember.GetSATK());
            if (SDEFElement) SDEFElement.UpdateInfo(selectedMember.GetSDEF());
            if (AGIElement) AGIElement.UpdateInfo(selectedMember.GetAGI());
            if (LUCKElement) LUCKElement.UpdateInfo(selectedMember.GetLUK());

            if (memberBioHUD) memberBioHUD.UpdateInfo(selectedMember);
        }

        public void UpdateLabels()
        {
            ATKElement?.UpdateLabel(TUFFSettings.ATKShortText);
            DEFElement?.UpdateLabel(TUFFSettings.DEFShortText);
            SATKElement?.UpdateLabel(TUFFSettings.SATKShortText);
            SDEFElement?.UpdateLabel(TUFFSettings.SDEFShortText);
            AGIElement?.UpdateLabel(TUFFSettings.AGIShortText);
            LUCKElement?.UpdateLabel(TUFFSettings.LUKShortText);
        }
    }
}
