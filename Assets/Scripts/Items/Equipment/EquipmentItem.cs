using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentItem : BaseItem
{
    [SerializeField] EquipmentSlotKey m_slotKey = 0;

    public EquipmentSlotKey slotKey { get { return m_slotKey; } }

    public override void UseAction(Unit targetUnit)
    {
        if (targetUnit.equipment.EquipItem(this))
        {
            // success in equipping the item
            Debug.Log("Equipped " + itemName);
        }
        else
        {
            // failed equipping, nothing happened
            Debug.Log("Failed to Equip");
        }
    }

    public override int GetMaxStackCount()
    {
        // equipment won't stack in inventorys
        return 1;
    }
}
