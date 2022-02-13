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
    public static UnitStats basic { get { return _basic; } }

    static UnitStats()
    {
        _basic = new UnitStats();

        _basic.health = 20.0f;
        _basic.strength = 5.0f;
        _basic.speed = 5.0f;
    }
}
