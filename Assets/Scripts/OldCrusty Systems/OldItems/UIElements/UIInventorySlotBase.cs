using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using NewUnitStats.Inventory;

// This is an abstract class that is inherited purely just so the UI functionality can match the inventory slot type
public abstract class UIInventorySlotBase<TOwnerClass, TItemClass> : MonoBehaviour where TItemClass : IInventoryItem<TOwnerClass>
{
    InventorySlot<TOwnerClass, TItemClass> m_inventorySlot = default;
    [SerializeField] TMP_Text m_itemLabel = null;
    [SerializeField] Button m_useButton = null;

    public delegate string LabelUpdateMethod(InventorySlot<TOwnerClass, TItemClass> inventorySlot);
    LabelUpdateMethod m_labelUpdateDelegate = DefaultLabelSetter;

    UnityAction<InventorySlot<TOwnerClass, TItemClass>> m_buttonAction = null;

    public static UIInventorySlotBase<TOwnerClass, TItemClass> InstantiateUIInventorySlot(UIInventorySlotBase<TOwnerClass, TItemClass> UIInventorySlotPrefab, InventorySlot<TOwnerClass, TItemClass> inventorySlotTarget, Transform parent, LabelUpdateMethod labelUpdateMethod)
    {
        UIInventorySlotBase<TOwnerClass, TItemClass> newUISlot = Object.Instantiate(UIInventorySlotPrefab, parent);
        newUISlot.SetLabelUpdateMethod(labelUpdateMethod);
        newUISlot.SetInventorySlot(inventorySlotTarget, UseSlotItem);
        return newUISlot;
    }

    public void SetInventorySlot(InventorySlot<TOwnerClass, TItemClass> inventorySlot, UnityAction<InventorySlot<TOwnerClass, TItemClass>> buttonAction)
    {
        if (m_inventorySlot != null)
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

    public static string DefaultLabelSetter(InventorySlot<TOwnerClass, TItemClass> inventorySlot)
    {
        return "LabelSetMethod is not set";
    }

    public void SetLabelUpdateMethod(LabelUpdateMethod labelUpdateMethod)
    {
        if (labelUpdateMethod == null)
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

    static void UseSlotItem(InventorySlot<TOwnerClass, TItemClass> inventorySlot)
    {
        inventorySlot.UseItem();
    }
}
