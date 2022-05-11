using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NewUnitStats;

[System.Serializable]
public class PlayerHUD
{
    [SerializeField] UIResourceBar m_healthBar = null;
    [SerializeField] UIResourceBar m_hungerBar = null;

    [System.NonSerialized] UnitCharacterInfo m_player = null;

    MessageLog m_targetMessageLog = null;
    [SerializeField] MessageBoxEvents m_messageBoxEvents = null;

    [SerializeField] UIInventory m_UIInventory = null;

    public void SetPlayerUnit(UnitCharacterInfo targetPlayer)
    {
        if(m_player != null)
        {
            m_healthBar.RemoveResourceStat(m_player.unitStats.GetStat(ResourceStatKey.health));
            m_hungerBar.RemoveResourceStat(m_player.unitStats.GetStat(ResourceStatKey.hunger));
            m_UIInventory.DestroyUIInventorySlots();
        }

        m_player = targetPlayer;

        if (m_player != null)
        {
            m_healthBar.AttachResourceStat(m_player.unitStats.GetStat(ResourceStatKey.health));
            m_hungerBar.AttachResourceStat(m_player.unitStats.GetStat(ResourceStatKey.hunger));

            m_UIInventory.SetInventory(m_player.inventory);
            m_UIInventory.InstantiateUIInventorySlots();
        }
    }

    // Attachs a message Log to the messageBox Display
    public void SetMessageLog(MessageLog messageLog)
    {
        RemoveMessageBox();
        m_targetMessageLog = messageLog;
        if(m_targetMessageLog == null)
        {
            return;
        }

        m_messageBoxEvents.ClearText();
        string[] messages = messageLog.GetMessages();

        foreach(string msg in messages)
        {
            m_messageBoxEvents.ShowMessage(msg);
        }
        m_messageBoxEvents.AddEventsToMessageLog(messageLog);
    }

    public void RemoveMessageBox()
    {
        if(m_targetMessageLog != null)
        {
            m_messageBoxEvents.RemoveEventsFromMessageLog(m_targetMessageLog);
        }
    }
}