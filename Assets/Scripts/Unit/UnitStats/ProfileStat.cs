using System;
using System.Collections.Generic;
using System.Text;

namespace MysterySystems.UnitStats
{
    // This may look dumb, but I want this to be a reference type rather than a value type
    public class ProfileStat
    {
        public float value = 0.0f;

        public ProfileStat()
        {
            
        }

        public ProfileStat(float value)
        {
            this.value = value;
        }
    }
}
