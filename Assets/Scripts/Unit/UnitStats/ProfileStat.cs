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

    public abstract class BaseStat
    {
        public float value = 0.0f;

        public BaseStat()
        {

        }

        public BaseStat(float value)
        {
            Init(value);
        }

        public virtual void Init(float value)
        {
            this.value = value;
        }
    }

    // used for things like strength or speed
    public class CoreStat : BaseStat
    {
        public CoreStat()
        {

        }

        public CoreStat(float value) : base(value)
        {
            
        }
    }

    // used for things like health
    public class ResourceStat : BaseStat
    {
        public float maxValue = 0.0f;

        public ResourceStat()
        {

        }

        public ResourceStat(float value) : base(value)
        {

        }

        public ResourceStat(float value, float maxValue)
        {
            this.value = value;
            this.maxValue = maxValue;
        }

        public override void Init(float value)
        {
            this.value = value;
            this.maxValue = value;
        }

        public void Reset()
        {
            value = maxValue;
        }
    }
}
