using System;
using System.Collections.Generic;
using System.Text;
using MysterySystems.UnitStats.Serialised;
using MysterySystems.UnitStats.Profile.Internal;

namespace MysterySystems.UnitStats
{
    // This will be a read only class. It will be created during runtime
    // Dictionarys are not serialised so another class (SerialisedUnitProfile) is used for creation and saving.
    public class UnitProfile
    {
        string m_ProfileName = null;

        UnitProfileStatsGroup<ResourceStatKey> m_resourceStats = new UnitProfileStatsGroup<ResourceStatKey>();
        UnitProfileStatsGroup<CoreStatKey> m_coreStats = new UnitProfileStatsGroup<CoreStatKey>();

        public string profileName { get { return m_ProfileName; } }
        public ResourceStatKey[] resourceStatKeys { get { return m_resourceStats.statKeys; } }
        public CoreStatKey[] coreStatKeys { get { return m_coreStats.statKeys; } }

        public UnitProfile(SerialisedUnitProfile serialisedUnitProfile)
        {
            m_ProfileName = serialisedUnitProfile.unitName;
            CreateStatsInStatGroup(m_resourceStats, serialisedUnitProfile.resourceKeyValues);
            CreateStatsInStatGroup(m_coreStats, serialisedUnitProfile.coreKeyValues);
        }

        void CreateStatsInStatGroup<KeyTypeEnum>(UnitProfileStatsGroup<KeyTypeEnum> targetGroup, UnitStatKeyValues<KeyTypeEnum>[] keyValues) where KeyTypeEnum : Enum
        {
            for (int i = 0; i < keyValues.Length; i++)
            {
                targetGroup.CreateStat(keyValues[i]);
            }
        }

        public float GetBaseValue(CoreStatKey statKey)
        {
            return GetBaseValue(m_coreStats, statKey);
        }

        public float GetGrowthValue(CoreStatKey statKey)
        {
            return GetGrowthValue(m_coreStats, statKey);
        }

        public float GetBaseValue(ResourceStatKey statKey)
        {
            return GetBaseValue(m_resourceStats, statKey);
        }

        public float GetGrowthValue(ResourceStatKey statKey)
        {
            return GetGrowthValue(m_resourceStats, statKey);
        }

        float GetBaseValue<KeyTypeEnum>(UnitProfileStatsGroup<KeyTypeEnum> targetGroup, KeyTypeEnum statKey) where KeyTypeEnum : Enum
        {
            return targetGroup.GetBaseValue(statKey);
        }

        float GetGrowthValue<KeyTypeEnum>(UnitProfileStatsGroup<KeyTypeEnum> targetGroup, KeyTypeEnum statKey) where KeyTypeEnum : Enum
        {
            return targetGroup.GetGrowthValue(statKey);
        }
    }
}
