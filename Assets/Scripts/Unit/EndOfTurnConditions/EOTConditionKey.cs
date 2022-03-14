using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EOTConditionKey
{
    hunger
}

public static class EndOfTurnConditionCreator
{
    public static UnitEndTurnCondition CreateCondition(EOTConditionKey conditionKey)
    {
        switch (conditionKey)
        {
            case EOTConditionKey.hunger:
                {
                    return new HungerCondition();
                }
            default:
                {
                    return null;
                }
        }
    }
}
