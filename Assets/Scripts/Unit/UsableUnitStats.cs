using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a struct that the unit is actually using when performing calculations, such as attack and defense.
public struct UsableUnitStats
{
    int m_level;
    float m_currentHealth;
    UnitStats m_currentStats;

    Unit m_unitRef;

    public float speed { get { return m_currentStats.speed; } }


    static UsableUnitStats _zero;
    public static UsableUnitStats zero { get { return _zero; } }

    static UsableUnitStats()
    {
        _zero = new UsableUnitStats();
        _zero.m_level = 0;
        _zero.m_currentHealth = 0;
        _zero.m_currentStats = UnitStats.zero;
        _zero.m_unitRef = null;
    }

    void LoadUnit()
    {
        // load unit stats from some file.
        // This is how the players unit's will be loaded.
    }

    void Generate(int level, UnitStats baseStats)
    {
        m_currentStats = baseStats;
        for(int i = 0; i < level; i++)
        {
            LevelUp();
        }
        m_currentHealth = m_currentStats.health;
    }

    public static UsableUnitStats GenerateOnUnit(Unit unit, UnitProfile unitProfile, int level)
    {
        UsableUnitStats result = UsableUnitStats.zero;
        result.m_unitRef = unit;
        result.Generate(level, unitProfile.baseStats);
        return result;
    }

    void LevelUp()
    {
        m_level++;
        m_currentStats += m_unitRef.profile.statGrowth;
    }

    public float CalcAttackDamage()
    {
        return m_currentStats.strength;
    }

    public float CalcDamageToHealth(float attackValue)
    {
        // this will eventually take defense values into account
        return attackValue;
    }

    public void ReceiveDamage(float attackValue)
    {
        m_currentHealth -= CalcDamageToHealth(attackValue);
        if(m_currentHealth < 0)
        {
            m_unitRef.Die();
        }
    }
}
