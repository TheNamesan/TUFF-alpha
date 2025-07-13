using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class MemberEquipmentMenu : MonoBehaviour
    {
        public InventoryItemViewer inventoryItemViewer;
        [Header("References")]
        public GeneralInfoDisplay primaryWeaponInfo;
        public GeneralInfoDisplay secondaryWeaponInfo;
        public GeneralInfoDisplay headInfo;
        public GeneralInfoDisplay bodyInfo;
        public GeneralInfoDisplay capeInfo;
        public GeneralInfoDisplay primaryAccessoryInfo;
        public GeneralInfoDisplay secondaryAccessoryInfo;
        public UIMenu uiMenu;

        [System.NonSerialized] public PartyMember memberRef;

        public virtual void UpdateInfo(PartyMember member)
        {
            memberRef = member;
            UpdateEquipInfo(primaryWeaponInfo, EquipmentSlotType.PrimaryWeapon);
            UpdateEquipInfo(secondaryWeaponInfo, EquipmentSlotType.SecondaryWeapon);
            UpdateEquipInfo(headInfo, EquipmentSlotType.Head);
            UpdateEquipInfo(bodyInfo, EquipmentSlotType.Body);
            UpdateEquipInfo(capeInfo, EquipmentSlotType.Cape);
            UpdateEquipInfo(primaryAccessoryInfo, EquipmentSlotType.PrimaryAccessory);
            UpdateEquipInfo(secondaryAccessoryInfo, EquipmentSlotType.SecondaryAccessory);
        }
        protected virtual void UpdateEquipInfo(GeneralInfoDisplay infoDisplay, EquipmentSlotType slotType)
        {
            if (infoDisplay == null) return;
            if (memberRef == null) return;
            infoDisplay.gameObject.SetActive(memberRef.CanEquipInSlot(slotType));
            InventoryItem invItem = (InventoryItem)memberRef.GetEquipmentFromUserSlot(slotType);
            if (invItem == null) { infoDisplay.DisplayEmpty(); UpdateSlotDescription(infoDisplay, ""); return; }
            infoDisplay.DisplayInfo(invItem.icon, invItem.GetName());
            UpdateSlotDescription(infoDisplay, invItem.GetDescription());
        }
        protected virtual void UpdateSlotDescription(GeneralInfoDisplay infoDisplay, string description)
        {
            if (infoDisplay.uiElement == null) return;
            infoDisplay.uiElement.highlightDisplayText = description;
        }
    }
}

