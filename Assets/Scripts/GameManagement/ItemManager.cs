using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager
{
    ItemLibrary m_itemLibrary = null;

    public ItemManager(ItemLibrary itemLibrary)
    {
        SetItemLibrary(itemLibrary);
    }

    public void SetItemLibrary(ItemLibrary itemLibrary)
    {
        m_itemLibrary = itemLibrary;
    }

    public void AddItemToInventory(Inventory<Unit> inventory, TempItemID tempItemID)
    {
        inventory.AddItem(m_itemLibrary.GetConsumableItem(tempItemID));
    }
}
