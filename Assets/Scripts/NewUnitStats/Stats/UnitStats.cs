using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Serialisable;

namespace NewUnitStats
{
    [System.Serializable]
    public class UnitStats
    {
        [SerializeField] SerialisedDictionary<CoreStatKey, CoreStat> m_coreStats = new SerialisedDictionary<CoreStatKey, CoreStat>();
        [SerializeField] SerialisedDictionary<ResourceStatKey, ResourceStat> m_resourceStats = new SerialisedDictionary<ResourceStatKey, ResourceStat>();

        public Dictionary<CoreStatKey, CoreStat> coreStats { get { return m_coreStats.GetDictionary(); } }
        public Dictionary<ResourceStatKey, ResourceStat> resourceStats { get { return m_resourceStats.GetDictionary(); } }

        public UnitStats()
        {

        }

        public UnitStats(UnitStats other)
        {
            m_coreStats.SetDictionary(new Dictionary<CoreStatKey, CoreStat>());
            m_resourceStats.SetDictionary(new Dictionary<ResourceStatKey, ResourceStat>());
            Copy(other);
        }

        public void Copy(UnitStats other)
        {
            CopyStatDictionary(other.coreStats);
            CopyStatDictionary(other.resourceStats);
        }

        void CopyStatDictionary(Dictionary<CoreStatKey, CoreStat> coreStatsSource)
        {
            foreach (var corePair in coreStatsSource)
            {
                coreStats[corePair.Key] = new CoreStat(corePair.Value.value);
                CoreStat coreStat = GetStat(corePair.Key);
                coreStat.value = corePair.Value.value;
            }
        }

        void CopyStatDictionary(Dictionary<ResourceStatKey, ResourceStat> resourceStatSource)
        {
            foreach (var resourcePair in resourceStatSource)
            {
                ResourceStat resourceStat = GetStat(resourcePair.Key);
                resourceStat.value = resourcePair.Value.value;
                resourceStat.maxValue = resourcePair.Value.maxValue;
            }
        }

        public CoreStat GetStat(CoreStatKey statKey)
        {
            return GetStat(coreStats, statKey);
        }

        public ResourceStat GetStat(ResourceStatKey statKey)
        {
            return GetStat(resourceStats, statKey);
        }

        static TValue GetStat<TKey, TValue>(Dictionary<TKey, TValue> statSource, TKey statKey) where TValue : new()
        {
            if(statSource.TryGetValue(statKey, out TValue resultStat))
            {
                return resultStat;
            }
            else
            {
                TValue newStat = new TValue();
                statSource[statKey] = newStat;
                return newStat;
            }
        }
    }
}
