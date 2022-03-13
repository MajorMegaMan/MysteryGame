using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MysterySystems.UnitStats.Serialised
{
    // This will be the class that the user interacts with in the editor.
    [System.Serializable]
    public class SerialisedUnitProfile
    {
        [SerializeField] string m_unitName = null;
        [SerializeField] List<UnitStatKeyValues<ResourceStatKey>> m_resourceKeyValues = new List<UnitStatKeyValues<ResourceStatKey>>();
        [SerializeField] List<UnitStatKeyValues<CoreStatKey>> m_coreKeyValues = new List<UnitStatKeyValues<CoreStatKey>>();

        public string unitName { get { return m_unitName; } set { m_unitName = value; } }
        public UnitStatKeyValues<ResourceStatKey>[] resourceKeyValues { get { return m_resourceKeyValues.ToArray(); } }
        public UnitStatKeyValues<CoreStatKey>[] coreKeyValues { get { return m_coreKeyValues.ToArray(); } }

        public void Init()
        {
            Array resourceKeys = Enum.GetValues(typeof(ResourceStatKey));
            Array coreKeys = Enum.GetValues(typeof(CoreStatKey));

            for(int i = 0; i < resourceKeys.Length; i++)
            {
                CreateStat((ResourceStatKey)resourceKeys.GetValue(i));
            }

            for (int i = 0; i < coreKeys.Length; i++)
            {
                CreateStat((CoreStatKey)coreKeys.GetValue(i));
            }
        }

        // This should be called if enums keys and keystringIDs do not match the actual enums.
        // This could happen when the enum class is changed.
        public void Clean()
        {
            Clean(ref m_resourceKeyValues);
            Clean(ref m_coreKeyValues);
        }

        void Clean<TStatKey>(ref List<UnitStatKeyValues<TStatKey>> keyValuesList) where TStatKey : Enum
        {
            Array resourceKeys = Enum.GetValues(typeof(TStatKey));

            List<UnitStatKeyValues<TStatKey>> oldKeyValues = keyValuesList;
            keyValuesList = new List<UnitStatKeyValues<TStatKey>>(resourceKeys.Length);
            // loop through enum keys
            foreach (var value in resourceKeys)
            {
                TStatKey key = (TStatKey)value;

                // find if key exists in old values
                if(ListContainsStringID(oldKeyValues, key, out int successIndex))
                {
                    keyValuesList.Add(oldKeyValues[successIndex]);
                    oldKeyValues.RemoveAt(successIndex);
                }
                else
                {
                    CreateStat(keyValuesList, key);
                }
            }
        }

        bool ListContainsStringID<TStatKey>(List<UnitStatKeyValues<TStatKey>> keyValuesList, TStatKey key, out int keyValueIndex) where TStatKey : Enum
        {
            // find if key exists in old values
            for (int i = 0; i < keyValuesList.Count; i++)
            {
                if (keyValuesList[i].keyStringID == key.ToString())
                {
                    keyValueIndex = i;
                    return true;
                }
            }
            keyValueIndex = -1;
            return false;
        }

        // This is currently for debug purposes
        public void CreateStat(CoreStatKey statKey, float baseValue = 0.0f, float growthValue = 0.0f)
        {
            CreateStat(m_coreKeyValues, statKey, baseValue, growthValue);
        }

        // This is currently for debug purposes
        public void CreateStat(ResourceStatKey statKey, float baseValue = 0.0f, float growthValue = 0.0f)
        {
            CreateStat(m_resourceKeyValues, statKey, baseValue, growthValue);
        }

        void CreateStat<TStatKey>(List<UnitStatKeyValues<TStatKey>> keyValuesList, TStatKey statKey, float baseValue = 0.0f, float growthValue = 0.0f) where TStatKey : Enum
        {
            UnitStatKeyValues<TStatKey> keyValues = new UnitStatKeyValues<TStatKey>(statKey, baseValue, growthValue);
            keyValuesList.Add(keyValues);
        }

        public UnitStatKeyValues<CoreStatKey> GetCoreStatKeyValue(int i)
        {
            return GetUnitStatKeyValue(m_coreKeyValues, i);
        }

        public UnitStatKeyValues<ResourceStatKey> GetResourceStatKeyValue(int i)
        {
            return GetUnitStatKeyValue(m_resourceKeyValues, i);
        }

        static UnitStatKeyValues<KeyTypeEnum> GetUnitStatKeyValue<KeyTypeEnum>(List<UnitStatKeyValues<KeyTypeEnum>> keyValues, int i) where KeyTypeEnum : Enum
        {
            return keyValues[i];
        }

        public bool Contains(CoreStatKey statKey)
        {
            return Contains(m_coreKeyValues, statKey);
        }

        public bool Contains(ResourceStatKey statKey)
        {
            return Contains(m_resourceKeyValues, statKey);
        }

        static bool Contains<KeyTypeEnum>(List<UnitStatKeyValues<KeyTypeEnum>> keyValues, KeyTypeEnum statKey) where KeyTypeEnum : Enum
        {
            foreach(var element in keyValues)
            {
                if(element.key.Equals(statKey))
                {
                    return true;
                }
            }
            return false;
        }

        public void Remove(CoreStatKey statKey)
        {
            Remove(m_coreKeyValues, statKey);
        }

        public void Remove(ResourceStatKey statKey)
        {
            Remove(m_resourceKeyValues, statKey);
        }

        static void Remove<KeyTypeEnum>(List<UnitStatKeyValues<KeyTypeEnum>> keyValues, KeyTypeEnum statKey) where KeyTypeEnum : Enum
        {
            for(int i = 0; i < keyValues.Count; i++)
            {
                var element = keyValues[i];
                if (element.key.Equals(statKey))
                {
                    keyValues.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
