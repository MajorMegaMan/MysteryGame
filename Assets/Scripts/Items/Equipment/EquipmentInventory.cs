using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventory : InventoryBase<Unit>
{
    Dictionary<EquipmentSlotKey, InventorySlot<Unit>> m_equipmentSlots;

    int m_slotCount;

    public int slotCount { get { return m_slotCount; } }

    public EquipmentInventory(Unit owner) : base(owner)
    {
        System.Array enumValues = System.Enum.GetValues(typeof(EquipmentSlotKey));
        m_slotCount = enumValues.Length;

        m_equipmentSlots = new Dictionary<EquipmentSlotKey, InventorySlot<Unit>>(m_slotCount);
        for(int i = 0; i < m_slotCount; i++)
        {
            EquipmentSlotKey key = (EquipmentSlotKey)enumValues.GetValue(i);
            m_equipmentSlots.Add(key, new InventorySlot<Unit>(this, i));
        }
    }

    // returns -1 if there is no available space in the unit's inventory
    // returns 0 if there was no item to unequip
    // returns 1 if the item was unequipped and moved to the inventory
    public int Unequip(EquipmentSlotKey slotKey)
    {
        IInventoryItem<Unit> alreadyEquippedItem = m_equipmentSlots[slotKey].GetItemType();
        if(alreadyEquippedItem != null)
        {
            if (unitOwner.inventory.AddItem(alreadyEquippedItem))
            {
                // successfully added the equipment to the inventory
                // safe to remove from equipment slot
                m_equipmentSlots[slotKey].SetItemType(null, 0);
                return 1;
            }
            else
            {
                // no available space for the equipment in the inventory
                // unsafe to remove equipment
                return -1;
            }
        }
        else
        {
            // there is no item to unequip
            return 0;
        }
    }

    public bool EquipItem(EquipmentItem equipmentItem)
    {
        int unequipResult = Unequip(equipmentItem.slotKey);
        if(unequipResult < 0)
        {
            // unequip Failed.
            return false;
        }

        m_equipmentSlots[equipmentItem.slotKey].SetItemType(equipmentItem);
        return true;
    }

    public InventorySlot<Unit> GetEquipSlot(EquipmentSlotKey slotKey)
    {
        return m_equipmentSlots[slotKey];
    }
}
