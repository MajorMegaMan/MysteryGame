using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class HungerCondition : UnitEndTurnCondition
{
    int m_stepCount = 0;
    int m_hungerCycle = 3;

    float m_depreciationValue = 1.0f;
    ResourceStat m_hungerStat = null;

    ResourceStat m_healthStat = null;

    public override void Setup()
    {
        m_hungerStat = targetUnit.unitStats.GetStat(ResourceStatKey.hunger);
        m_healthStat = targetUnit.unitStats.GetStat(ResourceStatKey.health);
    }

    public override void PerformAction()
    {
        m_stepCount++;
        if (m_stepCount > m_hungerCycle)
        {
            // Do hunger thing
            m_stepCount = 0;
            if (m_hungerStat.value > 0)
            {
                m_hungerStat.value -= m_depreciationValue;
            }
            else
            {
                m_healthStat.value -= m_depreciationValue;
            }
            targetUnit.InvokeStatChangeEvent();
        }
    }
}
*/