using System;
using System.Collections.Generic;
using System.Text;

namespace MysterySystems.UnitStats.Serialised
{
    // This is a serialised version of stats and their values.
    // This will be the main way that the user will interact with in editor to create stats.
    [System.Serializable]
    public struct UnitStatKeyValues<KeyTypeEnum> where KeyTypeEnum : Enum
    {
        public KeyTypeEnum key;
        public string keyStringID;
        public float baseValue;
        public float growthValue;

        public UnitStatKeyValues(KeyTypeEnum statKey, float baseValue = 0.0f, float growthValue = 0.0f)
        {
            key = statKey;
            keyStringID = statKey.ToString();
            this.baseValue = baseValue;
            this.growthValue = growthValue;
        }

        public static void CreateStatInList(List<UnitStatKeyValues<KeyTypeEnum>> list, KeyTypeEnum statKey, float baseValue = 0.0f, float growthValue = 0.0f)
        {
            UnitStatKeyValues<KeyTypeEnum> keyValues = new UnitStatKeyValues<KeyTypeEnum>(statKey, baseValue, growthValue);
            list.Add(keyValues);
        }
    }
}
