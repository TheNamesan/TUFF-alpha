using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class Inventory
    {
        public const int INVENTORY_CAP = 99; //Move this to TUFF Settings
        public int[] items = new int[0];
        public int[] keyItems = new int[0];
        public int[] weapons = new int[0];
        public int[] armors = new int[0];

        public static Inventory instance
        {
            get
            {
                return (PlayerData.instance != null ?
                   PlayerData.instance.inventory : null);
            }
        }
        public void Initialize()
        {
            items = new int[DatabaseLoader.items.Length];
            keyItems = new int[DatabaseLoader.keyItems.Length];
            weapons = new int[DatabaseLoader.weapons.Length];
            armors = new int[DatabaseLoader.armors.Length];

            if (TUFFSettings.DebugStartWithMaxItems())
                SetAmountToAll(INVENTORY_CAP);
        }
        public void ValidateInventory()
        {
            System.Array.Resize(ref items, DatabaseLoader.items.Length);
            System.Array.Resize(ref keyItems, DatabaseLoader.keyItems.Length);
            System.Array.Resize(ref weapons, DatabaseLoader.weapons.Length);
            System.Array.Resize(ref armors, DatabaseLoader.armors.Length);

            if (TUFFSettings.DebugStartWithMaxItems())
                SetAmountToAll(INVENTORY_CAP);
        }

        public Dictionary<InventoryItem, int> GetItemsAndAmount(Dictionary<InventoryItem, int> baseDirectory = null, bool includeZero = false)
        {
            if (baseDirectory == null) baseDirectory = new Dictionary<InventoryItem, int>();
            for (int i = 0; i < items.Length; i++) {
                var item = DatabaseLoader.items[i];
                var amount = GetItemAmount(item);
                if (!includeZero && amount <= 0) continue;
                baseDirectory.Add(item, amount);
            }
            return baseDirectory;
        }
        public Dictionary<InventoryItem, int> GetKeyItemsAndAmount(Dictionary<InventoryItem, int> baseDirectory = null, bool includeZero = false)
        {
            if (baseDirectory == null) baseDirectory = new Dictionary<InventoryItem, int>();
            for (int i = 0; i < keyItems.Length; i++) {
                var item = DatabaseLoader.keyItems[i];
                var amount = GetItemAmount(item);
                if (!includeZero && amount <= 0) continue;
                baseDirectory.Add(item, amount);
            }
            return baseDirectory;
        }
        public Dictionary<InventoryItem, int> GetAllWeaponsAndAmount(Dictionary<InventoryItem, int> baseDirectory = null, bool includeZero = false)
        {
            if (baseDirectory == null) baseDirectory = new Dictionary<InventoryItem, int>();
            for (int i = 0; i < weapons.Length; i++) {
                var item = DatabaseLoader.weapons[i];
                var amount = GetItemAmount(item);
                if (!includeZero && amount <= 0) continue;
                baseDirectory.Add(item, amount);
            }
            return baseDirectory;
        }
        public Dictionary<InventoryItem, int> GetWeaponsAndAmountOfType(WeaponWieldType wieldType, Dictionary<InventoryItem, int> baseDirectory = null, bool includeZero = false)
        {
            if (baseDirectory == null) baseDirectory = new Dictionary<InventoryItem, int>();
            for (int i = 0; i < weapons.Length; i++)
            {
                var item = DatabaseLoader.weapons[i];
                if (!IsMatchingWeaponType(item, wieldType, null)) continue;
                var amount = GetItemAmount(item);
                if (!includeZero && amount <= 0) continue;
                baseDirectory.Add(item, amount);
            }
            return baseDirectory;
        }
        public Dictionary<InventoryItem, int> GetWeaponsAndAmountOfType(WeaponWieldType wieldType, List<int> weaponTypes, Dictionary<InventoryItem, int> baseDirectory = null, bool includeZero = false)
        {
            if (baseDirectory == null) baseDirectory = new Dictionary<InventoryItem, int>();
            for (int i = 0; i < weapons.Length; i++)
            {
                var item = DatabaseLoader.weapons[i];
                if (!IsMatchingWeaponType(item, wieldType, weaponTypes)) continue;
                var amount = GetItemAmount(item);
                if (!includeZero && amount <= 0) continue;
                baseDirectory.Add(item, amount);
            }
            return baseDirectory;
        }
        public IEquipable GetHighestStatsWeapon(WeaponWieldType wieldType, List<int> weaponTypes)
        {
            IEquipable bestItem = null;
            for (int i = 0; i < weapons.Length; i++)
            {
                Weapon item = DatabaseLoader.weapons[i];
                if (!IsMatchingWeaponType(item, wieldType, weaponTypes)) continue;
                var amount = GetItemAmount(item);
                if (amount <= 0) continue;
                if (bestItem != null)
                { 
                    if (((IEquipable)item).GetStatTotal() > bestItem.GetStatTotal()) 
                    {
                        bestItem = item;
                    }
                }
                else bestItem = item;
            }
            return bestItem;
        }
        protected bool IsMatchingWeaponType(Weapon item, WeaponWieldType wieldType, List<int> weaponTypes)
        {
            if (!item) return false;
            if (item.wieldType != wieldType && item.wieldType != WeaponWieldType.AnyWeaponSlot) return false;
            if (weaponTypes == null || !weaponTypes.Contains(item.weaponType)) return false;
            return true;
        }
        public Dictionary<InventoryItem, int> GetAllArmorsAndAmount(Dictionary<InventoryItem, int> baseDirectory = null, bool includeZero = false)
        {
            if (baseDirectory == null) baseDirectory = new Dictionary<InventoryItem, int>();
            for (int i = 0; i < armors.Length; i++) {
                var item = DatabaseLoader.armors[i];
                var amount = GetItemAmount(item);
                if (!includeZero && amount <= 0) continue;
                baseDirectory.Add(item, amount);
            }
            return baseDirectory;
        }
        public Dictionary<InventoryItem, int> GetArmorsAndAmountOfType(EquipType equipType, Dictionary<InventoryItem, int> baseDirectory = null, bool includeZero = false)
        {
            if (baseDirectory == null) baseDirectory = new Dictionary<InventoryItem, int>();
            for (int i = 0; i < armors.Length; i++)
            {
                var item = DatabaseLoader.armors[i];
                if (!IsMatchingArmorType(item, equipType, null)) continue;
                var amount = GetItemAmount(item);
                if (!includeZero && amount <= 0) continue;
                baseDirectory.Add(item, amount);
            }
            return baseDirectory;
        }
        public Dictionary<InventoryItem, int> GetArmorsAndAmountOfType(EquipType equipType, List<int> armorTypes, Dictionary<InventoryItem, int> baseDirectory = null, bool includeZero = false)
        {
            if (baseDirectory == null) baseDirectory = new Dictionary<InventoryItem, int>();
            for (int i = 0; i < armors.Length; i++)
            {
                var item = DatabaseLoader.armors[i];
                if (!IsMatchingArmorType(item, equipType, armorTypes)) continue;
                var amount = GetItemAmount(item);
                if (!includeZero && amount <= 0) continue;
                baseDirectory.Add(item, amount);
            }
            return baseDirectory;
        }
        public IEquipable GetHighestStatsArmor(EquipType equipType, List<int> armorTypes)
        {
            IEquipable bestItem = null;
            for (int i = 0; i < armors.Length; i++)
            {
                Armor item = DatabaseLoader.armors[i];
                if (!IsMatchingArmorType(item, equipType, armorTypes)) continue;
                var amount = GetItemAmount(item);
                if (amount <= 0) continue;
                if (bestItem != null)
                {
                    if (((IEquipable)item).GetStatTotal() > bestItem.GetStatTotal())
                    {
                        bestItem = item;
                    }
                }
                else bestItem = item;
            }
            return bestItem;
        }
        protected bool IsMatchingArmorType(Armor item, EquipType equipType, List<int> armorTypes)
        {
            if (!item) return false;
            if (item.equipType != equipType) return false;
            if (armorTypes != null && !armorTypes.Contains(item.armorType)) return false;
            return true;
        }
        public Dictionary<InventoryItem, int> GetEntireInventoryAndAmount(Dictionary<InventoryItem, int> baseDirectory = null, bool includeZero = false)
        {
            if (baseDirectory == null) baseDirectory = new Dictionary<InventoryItem, int>();
            GetItemsAndAmount(baseDirectory, includeZero);
            GetKeyItemsAndAmount(baseDirectory, includeZero);
            GetAllWeaponsAndAmount(baseDirectory, includeZero);
            GetAllArmorsAndAmount(baseDirectory, includeZero);
            return baseDirectory;
        }
        public int GetItemAmount(InventoryItem item, bool includeEquipment)
        {
            if (!item) return -1;
            if (item is Item) return items[item.id];
            if (item is KeyItem) return keyItems[item.id];
            if (item is Weapon) return weapons[item.id] + (includeEquipment ? GetItemAmountFromPartyEquipment(item as IEquipable) : 0);
            if (item is Armor) return armors[item.id] + (includeEquipment ? GetItemAmountFromPartyEquipment(item as IEquipable) : 0);
            return -1;
        }
        public int GetItemAmount(InventoryItem item) => GetItemAmount(item, false);
        public int GetItemAmountFromPartyEquipment(IEquipable equipable)
        {
            return PlayerData.instance.GetItemAmountFromPartyEquipment(equipable);
        }
        public bool HasItem(InventoryItem item, bool includeEquipment)
        {
            return GetItemAmount(item) >= 0;
        }
        public bool HasItem(InventoryItem item) => HasItem(item, false);
        public void AddToInventory(Item item, int amount)
        {
            if (item == null) return;
            items[item.id] = Mathf.Clamp(items[item.id] + amount, 0, Inventory.INVENTORY_CAP);
        }
        public void AddToInventory(KeyItem keyItem, int amount)
        {
            if (keyItem == null) return;
            keyItems[keyItem.id] = Mathf.Clamp(keyItems[keyItem.id] + amount, 0, Inventory.INVENTORY_CAP);
        }
        public void AddToInventory(Weapon weapon, int amount)
        {
            if (weapon == null) return;
            weapons[weapon.id] = Mathf.Clamp(weapons[weapon.id] + amount, 0, Inventory.INVENTORY_CAP);
        }
        public void AddToInventory(Armor armor, int amount)
        {
            if (armor == null) return;
            armors[armor.id] = Mathf.Clamp(armors[armor.id] + amount, 0, Inventory.INVENTORY_CAP);
        }
        /// <summary>
        /// Sets the amount of Items, Key Items, Weapons and Armors to the specified value.
        /// </summary>
        /// <param name="amount">Amount of items.</param>
        public void SetAmountToAll(int amount)
        {
            int value = Mathf.Clamp(amount, 0, INVENTORY_CAP);

            System.Array.Fill(items, value, 0, items.Length);
            System.Array.Fill(keyItems, value, 0, keyItems.Length);
            System.Array.Fill(weapons, value, 0, weapons.Length);
            System.Array.Fill(armors, value, 0, armors.Length);
        }
    }
}

