using System;
using System.Collections.Generic;
using System.Text;
using MysterySystems.UnitStats.Serialised;

namespace MysterySystems.UnitStats.Profile.Internal
{
    // This is used by UnitProfile to manage stats based on the KeyType
    class UnitProfileStatsGroup<KeyTypeEnum> where KeyTypeEnum : Enum
    {
        UnitProfileStats<KeyTypeEnum> m_baseStats = new UnitProfileStats<KeyTypeEnum>();
        UnitProfileStats<KeyTypeEnum> m_growthStats = new UnitProfileStats<KeyTypeEnum>();

        public KeyTypeEnum[] statKeys { get { return m_baseStats.statKeys; } }

        public void CreateStat(UnitStatKeyValues<KeyTypeEnum> keyValues)
        {
            CreateStat(keyValues.key, keyValues.baseValue, keyValues.growthValue);
        }

        public void CreateStat(KeyTypeEnum statKey, float baseValue, float growthValue)
        {
            m_baseStats.CreateStat(statKey, baseValue);
            m_growthStats.CreateStat(statKey, growthValue);
        }

        public float GetBaseValue(KeyTypeEnum statKey)
        {
            return m_baseStats.GetValue(statKey);
        }

        public float GetGrowthValue(KeyTypeEnum statKey)
        {
            return m_growthStats.GetValue(statKey);
        }
    }
}
