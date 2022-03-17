using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIInventorySlot : MonoBehaviour
{
    InventorySlot<Unit> m_inventorySlot = null;
    [SerializeField] TMP_Text m_itemLabel = null;
    [SerializeField] Button m_useButton = null;

    public void SetInventorySlot(InventorySlot<Unit> inventorySlot)
    {
        if(m_inventorySlot != null)
        {
            m_useButton.onClick.RemoveListener(m_inventorySlot.UseItem);
            m_inventorySlot.RemoveOnCountChange(UpdateLabel);
            m_inventorySlot.RemoveOnItemTypeChange(UpdateLabel);
        }

        m_inventorySlot = inventorySlot;
        if (inventorySlot != null)
        {
            m_useButton.onClick.AddListener(m_inventorySlot.UseItem);
            m_inventorySlot.AddOnCountChange(UpdateLabel);
            m_inventorySlot.AddOnItemTypeChange(UpdateLabel);
        }

        UpdateLabel();
    }

    public void UpdateLabel()
    {
        string labelText;
        if (m_inventorySlot.GetItemType() != null)
        {
            string numText = m_inventorySlot.count.ToString();
            if (numText.Length < 2)
            {
                numText = "0" + numText;
            }
            labelText = numText + "x " + m_inventorySlot.GetItemName() + "\n";
        }
        else
        {
            labelText = "--NULL--\n";
        }
        m_itemLabel.text = labelText;
    }
}
