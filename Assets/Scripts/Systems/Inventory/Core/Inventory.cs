using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory<TOwnerClass> : InventoryBase<TOwnerClass>
{
    List<InventorySlot<TOwnerClass>> m_items;
    int m_maxCapacity = 5;

    public int maxCapacity { get { return m_maxCapacity; } }

    public Inventory(TOwnerClass unitOwner, int maxCapacity = 5) : base(unitOwner)
    {
        m_maxCapacity = maxCapacity;
        m_items = new List<InventorySlot<TOwnerClass>>(maxCapacity);
        for(int i = 0; i < maxCapacity; i++)
        {
            m_items.Add(new InventorySlot<TOwnerClass>(this, i));
        }
    }

    public bool AddItem(IInventoryItem<TOwnerClass> newItem)
    {
        for(int i = 0; i < m_items.Count; i++)
        {
            int slotID = m_items[i].GetItemID();
            if(slotID == -1)
            {
                // slot is empty
                m_items[i].SetItemType(newItem);
                return true;
            }
            else if (slotID == newItem.GetUniqueItemID())
            {
                // slot is the desired itemType
                if(m_items[i].TryAddItemCount())
                {
                    return true;
                }
            }
        }

        return false;
    }

    public InventorySlot<TOwnerClass> GetSlot(int index)
    {
        return m_items[index];
    }
}
