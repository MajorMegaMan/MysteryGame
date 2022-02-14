using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UnitStats
{
    public float health;
    public float strength;
    public float speed;


    static UnitStats _basic;
    static UnitStats _basicGrowth;
    static UnitStats _zero;

    public static UnitStats basic { get { return _basic; } }
    public static UnitStats basicGrowth { get { return _basicGrowth; } }
    public static UnitStats zero { get { return _zero; } }

    static UnitStats()
    {
        _basic = new UnitStats();

        _basic.health = 20.0f;
        _basic.strength = 5.0f;
        _basic.speed = 5.0f;

        _basicGrowth = new UnitStats();

        _basicGrowth.health = 10.0f;
        _basicGrowth.strength = 2.0f;
        _basicGrowth.speed = 0.1f;

        _zero = new UnitStats();

        _zero.health = 0.0f;
        _zero.strength = 0.0f;
        _zero.speed = 0.0f;
    }

    public static UnitStats operator+(UnitStats lhs, UnitStats rhs)
    {
        UnitStats result = lhs;
        result.health += rhs.health;
        result.strength += rhs.health;
        result.speed += rhs.health;
        return result;
    }
}
