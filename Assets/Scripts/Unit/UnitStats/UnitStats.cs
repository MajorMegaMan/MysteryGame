using System;
using System.Collections.Generic;
using System.Text;

namespace MysterySystems.UnitStats
{
    public class UnitStats
    {
        UnitProfile m_profile = null;
        string m_unitName = null;
        int m_level = 0;
        Dictionary<ResourceStatKey, ResourceStat> m_resourceValues = new Dictionary<ResourceStatKey, ResourceStat>();
        Dictionary<CoreStatKey, CoreStat> m_coreValues = new Dictionary<CoreStatKey, CoreStat>();

        public UnitProfile profile { get { return m_profile; } }

        public string unitName { get { return m_unitName; } }
        public string profileName { get { return m_profile.profileName; } }
        public int level { get { return m_level; } }

        public CoreStatKey[] coreStatKeys
        {
            get
            {
                CoreStatKey[] keys = new CoreStatKey[m_coreValues.Count];
                m_coreValues.Keys.CopyTo(keys, 0);
                return keys;
            }
        }

        public ResourceStatKey[] resourceStatKeys
        {
            get
            {
                ResourceStatKey[] keys = new ResourceStatKey[m_resourceValues.Count];
                m_resourceValues.Keys.CopyTo(keys, 0);
                return keys;
            }
        }

        public UnitStats(UnitProfile unitProfile, int level)
        {
            m_unitName = unitProfile.profileName;
            m_profile = unitProfile;
            GenerateFromUnitProfile(unitProfile, level);
        }

        public UnitStats(string unitName, UnitProfile unitProfile, int level)
        {
            m_unitName = unitName;
            m_profile = unitProfile;
            GenerateFromUnitProfile(unitProfile, level);
        }

        void LoadFromPlayerFile()
        {
            // This will load player units
        }

        void GenerateFromUnitProfile(UnitProfile unitProfile, int level)
        {
            CoreStatKey[] coreKeys = unitProfile.coreStatKeys;
            for(int i = 0; i < coreKeys.Length; i++)
            {
                CreateStat(coreKeys[i], unitProfile.GetBaseValue(coreKeys[i]));
            }

            ResourceStatKey[] resourceKeys = unitProfile.resourceStatKeys;
            for (int i = 0; i < resourceKeys.Length; i++)
            {
                CreateStat(resourceKeys[i], unitProfile.GetBaseValue(resourceKeys[i]));
            }

            for (int i = 0; i < level; i++)
            {
                LevelUp();
            }

            for (int i = 0; i < resourceKeys.Length; i++)
            {
                m_resourceValues[resourceKeys[i]].Reset();
            }
        }

        void CreateStat(CoreStatKey statKey, float value)
        {
            CreateStat(m_coreValues, statKey, value);
        }

        void CreateStat(ResourceStatKey statKey, float value)
        {
            CreateStat(m_resourceValues, statKey, value);
        }

        void CreateStat<KeyTypeEnum, T>(Dictionary<KeyTypeEnum, T> targetStatDictionary, KeyTypeEnum key, float value) where KeyTypeEnum : Enum where T : BaseStat, new()
        {
            T coreStat = new T();
            coreStat.Init(value);
            targetStatDictionary.Add(key, coreStat);
        }

        public void LevelUp()
        {
            m_level++;
            CoreStatKey[] coreKeys = m_profile.coreStatKeys;
            for (int i = 0; i < coreKeys.Length; i++)
            {
                CoreStat coreStat = m_coreValues[coreKeys[i]];
                coreStat.value += m_profile.GetGrowthValue(coreKeys[i]);
            }

            ResourceStatKey[] resourceKeys = m_profile.resourceStatKeys;
            for (int i = 0; i < resourceKeys.Length; i++)
            {
                ResourceStat resourceStat = m_resourceValues[resourceKeys[i]];
                float growthValue = m_profile.GetGrowthValue(resourceKeys[i]);
                resourceStat.maxValue += growthValue;
            }
        }

        public ResourceStat GetStat(ResourceStatKey statKey)
        {
            return GetStat(m_resourceValues, statKey);
        }

        public CoreStat GetStat(CoreStatKey statKey)
        {
            return GetStat(m_coreValues, statKey);
        }

        T GetStat<KeyTypeEnum, T>(Dictionary<KeyTypeEnum, T> targetStatDictionary, KeyTypeEnum statKey) where KeyTypeEnum : Enum where T : BaseStat
        {
            return targetStatDictionary[statKey];
        }
    }
}
