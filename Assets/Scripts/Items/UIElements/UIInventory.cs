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
            var newUISlot = Instantiate(m_UIInventorySlotPrefab, m_contentWindow);
            Vector3 localPosition = originalTransform.localPosition;
            localPosition.y -= originalTransform.rect.height * i;
            newUISlot.transform.localPosition = localPosition;
            m_UIInventorySlotList.Add(newUISlot);
            m_UIInventorySlotList[i].SetInventorySlot(m_inventory.GetSlot(i));
        }
        Vector2 sizeDelta = m_contentWindow.sizeDelta;
        sizeDelta.y = originalTransform.rect.height * m_inventory.maxCapacity;
        m_contentWindow.sizeDelta = sizeDelta;
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
}
