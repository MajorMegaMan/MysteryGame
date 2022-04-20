using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[System.Serializable]
public class EquipmentItem : BaseItem
{
    [SerializeField] EquipmentSlotKey m_slotKey = 0;
    [SerializeField] List<CoreStatModifier> m_coreStatModifiers = new List<CoreStatModifier>();

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

    public float GetCoreStatModifier(CoreStatKey coreStatKey)
    {
        // TODO: use dictionary for easily searching for the desired corestatmodifier
        // for now just search the list, but I would like to replace the list with a dictionary.
        // Dictionary isn't serialized in Unity but lists are.
        foreach(var modifier in m_coreStatModifiers)
        {
            if(modifier.key == coreStatKey)
            {
                return modifier.value;
            }
        }
        return 0.0f;
    }
}

[System.Serializable]
struct CoreStatModifier
{
    [SerializeField] CoreStatKey m_key;
    [SerializeField] float m_value;

    public CoreStatKey key { get { return m_key; } }
    public float value { get { return m_value; } }
}
*/