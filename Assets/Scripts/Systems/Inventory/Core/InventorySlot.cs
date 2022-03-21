using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot<T>
{
    Inventory<T> m_owner = null;
    IInventoryItem<T> m_itemType = null;
    int m_count = 0;

    public delegate void ItemAction();
    ItemAction m_onItemTypeChange = null;
    ItemAction m_onCountChange = null;
    ItemAction m_onUse = null;

    // getters
    public int count { get { return m_count; } }

    public InventorySlot(Inventory<T> owner)
    {
        m_owner = owner;
    }

    public IInventoryItem<T> GetItemType()
    {
        return m_itemType;
    }

    public void SetItemType(IInventoryItem<T> itemType, int targetCount = 1)
    {
        m_itemType = itemType;
        m_count = targetCount;
        m_onItemTypeChange?.Invoke();
    }

    public string GetItemName()
    {
        return m_itemType.itemName;
    }

    public int GetItemID()
    {
        if(m_itemType == null)
        {
            return -1;
        }
        return m_itemType.GetID();
    }

    public void UseItem()
    {
        m_count--;
        m_onCountChange?.Invoke();
        UseItemNoCountChange();
        if (m_count <= 0)
        {
            SetItemType(null, 0);
        }
    }

    public void UseItemNoCountChange()
    {
        m_itemType.Use(m_owner.unitOwner);
        m_onUse?.Invoke();
    }

    public bool TryAddItemCount(int toAddCount = 1)
    {
        if(m_count + toAddCount <= m_itemType.GetMaxStackCount())
        {
            m_count += toAddCount;
            m_onCountChange?.Invoke();
            return true;
        }
        return false;
    }

    public void AddOnItemTypeChange(ItemAction useAction)
    {
        m_onItemTypeChange += useAction;
    }

    public void RemoveOnItemTypeChange(ItemAction useAction)
    {
        m_onItemTypeChange -= useAction;
    }

    public void AddOnCountChange(ItemAction useAction)
    {
        m_onCountChange += useAction;
    }

    public void RemoveOnCountChange(ItemAction useAction)
    {
        m_onCountChange -= useAction;
    }

    public void AddOnUse(ItemAction useAction)
    {
        m_onUse += useAction;
    }

    public void RemoveOnUse(ItemAction useAction)
    {
        m_onUse -= useAction;
    }
}
