using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIInventorySlot : MonoBehaviour
{
    InventorySlot<Unit> m_inventorySlot = null;
    [SerializeField] TMP_Text m_itemLabel = null;
    [SerializeField] Button m_useButton = null;

    public delegate string LabelUpdateMethod(InventorySlot<Unit> inventorySlot);
    LabelUpdateMethod m_labelUpdateDelegate = DefaultLabelSetter;

    UnityAction<InventorySlot<Unit>> m_buttonAction = null;

    public static UIInventorySlot InstantiateUIInventorySlot(UIInventorySlot UIInventorySlotPrefab, InventorySlot<Unit> inventorySlotTarget, Transform parent, LabelUpdateMethod labelUpdateMethod)
    {
        UIInventorySlot newUISlot = Instantiate(UIInventorySlotPrefab, parent);
        newUISlot.SetLabelUpdateMethod(labelUpdateMethod);
        newUISlot.SetInventorySlot(inventorySlotTarget, UseSlotItem);
        return newUISlot;
    }

    public void SetInventorySlot(InventorySlot<Unit> inventorySlot, UnityAction<InventorySlot<Unit>> buttonAction)
    {
        if(m_inventorySlot != null)
        {
            m_useButton.onClick.RemoveListener(UseButton);
            m_inventorySlot.RemoveOnCountChange(UpdateLabel);
            m_inventorySlot.RemoveOnItemTypeChange(UpdateLabel);
        }

        m_inventorySlot = inventorySlot;
        if (inventorySlot != null)
        {
            m_buttonAction = buttonAction;
            m_useButton.onClick.AddListener(UseButton);
            m_inventorySlot.AddOnCountChange(UpdateLabel);
            m_inventorySlot.AddOnItemTypeChange(UpdateLabel);
        }

        UpdateLabel();
    }

    public static string DefaultLabelSetter(InventorySlot<Unit> inventorySlot)
    {
        return "LabelSetMethod is not set";
    }

    public void SetLabelUpdateMethod(LabelUpdateMethod labelUpdateMethod)
    {
        if(labelUpdateMethod == null)
        {
            m_labelUpdateDelegate = DefaultLabelSetter;
            return;
        }
        m_labelUpdateDelegate = labelUpdateMethod;
    }

    public void UpdateLabel()
    {
        m_itemLabel.text = m_labelUpdateDelegate.Invoke(m_inventorySlot);
    }

    void UseButton()
    {
        m_buttonAction.Invoke(m_inventorySlot);
    }

    static void UseSlotItem(InventorySlot<Unit> inventorySlot)
    {
        inventorySlot.UseItem();
    }
}
