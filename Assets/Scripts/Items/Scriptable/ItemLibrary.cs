using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemLibrary", menuName = "ItemLibrary")]
public class ItemLibrary : ScriptableObject
{
    [SerializeField] List<ConsumableItem> m_consumables = new List<ConsumableItem>();
    [SerializeField] List<EquipmentItem> m_equipment = new List<EquipmentItem>();

    public IInventoryItem<Unit> GetItem(TempItemID tempItemID)
    {
        int index = (int)tempItemID;
        if(index < m_consumables.Count)
        {
            return m_consumables[index];
        }
        else
        {
            index -= m_consumables.Count;
        }

        if (index < m_equipment.Count)
        {
            return m_equipment[index];
        }
        else
        {
            index -= m_equipment.Count;
        }
        return null;
    }
}
