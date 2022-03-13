using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MysterySystems.UnitStats;

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
        UpdateHUD(m_player.unitStats);
        m_player.ShowHealthBar(false);
    }

    public void ChangePlayerUnit(Unit targetPlayer)
    {
        m_player.RemoveStatChangeListener(UpdateHUD);
        m_player.ShowHealthBar(true);

        InitialiseWithPlayerUnit(targetPlayer);
    }

    void UpdateHUD(UnitStats unitStats)
    {
        ResourceStat healthStat = unitStats.GetStat(ResourceStatKey.health);
        m_healthBar.maxValue = healthStat.maxValue;
        m_healthBar.value = healthStat.value;

        m_heathBarFill.color = m_healthGradient.Evaluate(m_healthBar.normalizedValue);
    }
}
