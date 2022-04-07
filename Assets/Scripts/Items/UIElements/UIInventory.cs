using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    Inventory<Unit> m_inventory = null;
    [SerializeField] RectTransform m_contentWindow = null;
    [SerializeField] UIInventorySlot m_UIInventorySlotPrefab = null;
    List<UIInventorySlot> m_UIInventorySlotList = null;

    public void SetInventory(Inventory<Unit> inventory)
    {
        m_inventory = inventory;
    }

    public void InstantiateUIInventorySlots()
    {
        RectTransform originalTransform = m_UIInventorySlotPrefab.GetComponent<RectTransform>();
        m_UIInventorySlotList = new List<UIInventorySlot>(m_inventory.maxCapacity);
        for (int i = 0; i < m_inventory.maxCapacity; i++)
        {
            var newUISlot = InstantiateUIInventorySlot(m_inventory.GetSlot(i));

            Vector3 localPosition = originalTransform.localPosition;
            localPosition.y -= originalTransform.rect.height * i;
            localPosition.x += m_contentWindow.rect.width * 0.5f;
            newUISlot.transform.localPosition = localPosition;
        }
        Vector2 sizeDelta = m_contentWindow.sizeDelta;
        sizeDelta.y = originalTransform.rect.height * m_inventory.maxCapacity;
        m_contentWindow.sizeDelta = sizeDelta;
    }

    UIInventorySlot InstantiateUIInventorySlot(InventorySlot<Unit> inventorySlot)
    {
        UIInventorySlot newUISlot = UIInventorySlot.InstantiateUIInventorySlot(m_UIInventorySlotPrefab, inventorySlot, m_contentWindow, UpdateLabel);
        m_UIInventorySlotList.Add(newUISlot);
        return newUISlot;
    }

    public void DestroyUIInventorySlots()
    {
        for (int i = 0; i < m_UIInventorySlotList.Count; i++)
        {
            Destroy(m_UIInventorySlotList[i]);
        }
        m_UIInventorySlotList.Clear();
        m_UIInventorySlotList = null;
    }

    public static string UpdateLabel(InventorySlot<Unit> inventorySlot)
    {
        string labelText;
        if (inventorySlot.GetItemType() != null)
        {
            string numText = inventorySlot.count.ToString();
            if (numText.Length < 2)
            {
                numText = "0" + numText;
            }
            labelText = numText + "x " + inventorySlot.GetItemName() + "\n";
        }
        else
        {
            labelText = "--NULL--\n";
        }
        return labelText;
    }
}
