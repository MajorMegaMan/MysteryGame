using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class UIEquipmentInventory : MonoBehaviour
{
    EquipmentInventory m_equipmentInventory = null;
    [SerializeField] List<UIEquipmentSlot> m_UIInventorySlotList = null;

    public void SetInventory(EquipmentInventory inventory)
    {
        m_equipmentInventory = inventory;

        System.Array slotKeyValues = System.Enum.GetValues(typeof(EquipmentSlotKey));
        for(EquipmentSlotKey key = 0; (int)key < slotKeyValues.Length && (int)key < m_UIInventorySlotList.Count; key++)
        {
            m_UIInventorySlotList[(int)key].SetLabelUpdateMethod(UpdateLabel);
            InventorySlot<Unit, EquipmentItem> equipSlot = m_equipmentInventory.GetEquipSlot(key);
            m_UIInventorySlotList[(int)key].SetInventorySlot(equipSlot, UnequipSlotItem);
        }
    }

    public static string UpdateLabel(InventorySlot<Unit, EquipmentItem> inventorySlot)
    {
        string labelText;
        if (inventorySlot.GetItemType() != null)
        {
            labelText = inventorySlot.GetItemName() + "\n";
        }
        else
        {
            labelText = "--NULL--\n";
        }
        return labelText;
    }

    static void UnequipSlotItem(InventorySlot<Unit, EquipmentItem> inventorySlot)
    {
        inventorySlot.GetInventoryOwner<EquipmentInventory>().Unequip((EquipmentSlotKey)inventorySlot.inventoryPosition);
    }
}
*/