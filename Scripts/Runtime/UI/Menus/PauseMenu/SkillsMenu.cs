using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class SkillsMenu : MonoBehaviour
    {
        public DetailedUnitsMenu detailedUnitsMenu;
        public PreviewCommandListHUD previewCommandListHUD;
        public DetailedUnitHUD selectedDetailedUnitHUD;
        public UIMenu uiMenu;
        
        [System.NonSerialized] public PartyMember selectedMember;


        public void OnOpenMenu()
        {
            detailedUnitsMenu.uiMenu = uiMenu;
            detailedUnitsMenu?.UpdateUnits();
            selectedMember = null;
        }
        public void DetailedUnitsOnCreate(UIButton button, PartyMember member)
        {
            button.onHighlight.AddListener(() => { HighlightMember(member); });
            button.menusToOpen.Add(previewCommandListHUD.uiMenu);
        }
        protected void HighlightMember(PartyMember member)
        {
            selectedMember = member;
            UpdateInfo();
        }
        public void UpdateInfo()
        {
            if (selectedMember == null) return;
            if (previewCommandListHUD)
                previewCommandListHUD.UpdateCommands(selectedMember);
            if (selectedDetailedUnitHUD)
                selectedDetailedUnitHUD.UpdateInfo(selectedMember, true);
        }
    }
}
