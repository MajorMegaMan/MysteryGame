using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerHUD
{
    [SerializeField] Slider m_healthBar = null;
    [SerializeField] Image m_heathBarFill = null;
    [SerializeField] Gradient m_healthGradient = null;

    Unit m_player = null;

    public void InitialiseWithPlayerUnit(Unit player)
    {
        m_player = player;
        m_player.AddStatChangeListener(UpdateHUD);
        UpdateHUD(m_player.usableStats);
        m_player.ShowHealthBar(false);
    }

    public void ChangePlayerUnit(Unit targetPlayer)
    {
        m_player.RemoveStatChangeListener(UpdateHUD);
        m_player.ShowHealthBar(true);

        InitialiseWithPlayerUnit(targetPlayer);
    }

    void UpdateHUD(UsableUnitStats unitStats)
    {
        m_healthBar.maxValue = unitStats.maxHealth;
        m_healthBar.value = unitStats.currentHealth;
        m_heathBarFill.color = m_healthGradient.Evaluate(m_healthBar.normalizedValue);
    }
}
