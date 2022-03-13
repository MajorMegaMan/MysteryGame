using System;
using System.Collections.Generic;
using System.Text;

namespace MysterySystems.UnitStats.Profile.Internal
{
    class UnitProfileStats<KeyTypeEnum> where KeyTypeEnum : Enum
    {
        Dictionary<KeyTypeEnum, ProfileStat> m_values = new Dictionary<KeyTypeEnum, ProfileStat>();

        public KeyTypeEnum[] statKeys { 
            get 
            {
                KeyTypeEnum[] keys = new KeyTypeEnum[m_values.Count];
                m_values.Keys.CopyTo(keys, 0);
                return keys;
            }
        }

        public ProfileStat CreateStat(KeyTypeEnum statKey, float value = 0.0f)
        {
            ProfileStat unitStat = new ProfileStat(value);
            m_values.Add(statKey, unitStat);
            return unitStat;
        }

        public float GetValue(KeyTypeEnum statKey)
        {
            return m_values[statKey].value;
        }

        public void SetValue(KeyTypeEnum statKey, float value)
        {
            m_values[statKey].value = value;
        }
    }
}
