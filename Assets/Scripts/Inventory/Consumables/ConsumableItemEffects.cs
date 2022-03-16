using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MysterySystems.UnitStats;

public enum ConsumableItemEffectType
{
    heal,
    hungerFill
}

public enum ConsumableEffectResult
{
    success,
    failed,
    failedAlreadyMaxValue
}

public static class ConsumableItemEffects
{
    public delegate ConsumableEffectResult UseAction(ConsumableItem consumableItem, Unit targetUnit);
    static UseAction[] _useActions;

    static ConsumableItemEffects()
    {
        System.Array consumableItemEffectTypeValues = System.Enum.GetValues(typeof(ConsumableItemEffectType));
        _useActions = new UseAction[consumableItemEffectTypeValues.Length];
        _useActions[0] = Heal;
        _useActions[1] = HungerFill;
    }

    public static ConsumableEffectResult UseConsumable(ConsumableItem consumableItem, Unit targetUnit)
    {
        return _useActions[(int)consumableItem.effectType].Invoke(consumableItem, targetUnit);
    }

    public static ConsumableEffectResult Heal(ConsumableItem consumableItem, Unit targetUnit)
    {
        var healthStat = targetUnit.unitStats.GetStat(ResourceStatKey.health);
        return AddValueToResourceStat(healthStat, consumableItem.useValue);
    }

    public static ConsumableEffectResult HungerFill(ConsumableItem consumableItem, Unit targetUnit)
    {
        var hungerStat = targetUnit.unitStats.GetStat(ResourceStatKey.hunger);
        return AddValueToResourceStat(hungerStat, consumableItem.useValue);
    }

    public static ConsumableEffectResult AddValueToResourceStat(ResourceStat resourceStat, float value)
    {
        // Check if already max value
        if (resourceStat.value >= resourceStat.maxValue)
        {
            return ConsumableEffectResult.failedAlreadyMaxValue;
        }

        // add the value
        resourceStat.value += value;

        // clamp to max
        if (resourceStat.value > resourceStat.maxValue)
        {
            resourceStat.value = resourceStat.maxValue;
        }

        return ConsumableEffectResult.success;
    }
}
