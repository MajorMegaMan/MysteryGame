using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MysterySystems.UnitStats
{
    public abstract class BaseStat
    {
        protected float m_value = 0.0f;

        public delegate void StatAction(float value);
        StatAction m_onValueChange = null;
        public float value { get { return m_value; } set { m_value = value; m_onValueChange?.Invoke(m_value); } }

        public BaseStat()
        {

        }

        public BaseStat(float value)
        {
            Init(value);
        }

        public virtual void Init(float value)
        {
            this.m_value = value;
        }

        public void AddOnValueChange(StatAction statAction)
        {
            m_onValueChange += statAction;
        }

        public void RemoveOnValueChange(StatAction statAction)
        {
            m_onValueChange -= statAction;
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
        float m_maxValue = 0.0f;

        StatAction m_onMaxValueChange = null;
        public float maxValue { get { return m_maxValue; } set { m_maxValue = value; m_onMaxValueChange?.Invoke(m_maxValue); } }

        public ResourceStat()
        {

        }

        public ResourceStat(float value) : base(value)
        {

        }

        public ResourceStat(float value, float maxValue)
        {
            Init(value, maxValue);
        }

        public override void Init(float value)
        {
            this.m_value = value;
            this.m_maxValue = value;
        }

        public void Init(float value, float maxValue)
        {
            this.m_value = value;
            this.m_maxValue = maxValue;
        }

        public void Reset()
        {
            value = m_maxValue;
        }

        public void AddOnMaxValueChange(StatAction statAction)
        {
            m_onMaxValueChange += statAction;
        }

        public void RemoveOnMaxValueChange(StatAction statAction)
        {
            m_onMaxValueChange -= statAction;
        }
    }
}
