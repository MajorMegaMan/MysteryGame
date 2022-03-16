using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MysterySystems.UnitStats;

[System.Serializable]
public class PlayerHUD
{
    [SerializeField] UIResourceBar m_healthBar = null;
    [SerializeField] UIResourceBar m_hungerBar = null;

    Unit m_player = null;
    [SerializeField] UIInventory m_UIInventory = null;

    public void InitialiseWithPlayerUnit(Unit player)
    {
        m_player = player;

        m_healthBar.AttachResourceStat(m_player.unitStats.GetStat(ResourceStatKey.health));
        m_hungerBar.AttachResourceStat(m_player.unitStats.GetStat(ResourceStatKey.hunger));

        m_player.ShowHealthBar(false);

        m_UIInventory.SetInventory(m_player.inventory);
        m_UIInventory.InstantiateUIInventorySlots();
    }

    public void ChangePlayerUnit(Unit targetPlayer)
    {
        m_healthBar.RemoveResourceStat(m_player.unitStats.GetStat(ResourceStatKey.health));
        m_hungerBar.RemoveResourceStat(m_player.unitStats.GetStat(ResourceStatKey.hunger));

        m_player.ShowHealthBar(true);
        m_UIInventory.DestroyUIInventorySlots();

        InitialiseWithPlayerUnit(targetPlayer);
    }
}
