using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NewUnitStats;

//[System.Serializable]
public class UIResourceBar : MonoBehaviour
{
    [SerializeField] Slider m_Slider = null;
    [SerializeField] Image m_barFill = null;
    [SerializeField] Gradient m_gradient = null;

    public void AttachResourceStat(ResourceStat targetStat)
    {
        targetStat.AddOnValueChange(UpdateHealthBarCurrent);
        targetStat.AddOnMaxValueChange(UpdateHealthBarMax);

        UpdateHealthBarMax(targetStat.maxValue);
        UpdateHealthBarCurrent(targetStat.value);
    }

    public void RemoveResourceStat(ResourceStat targetStat)
    {
        targetStat.RemoveOnValueChange(UpdateHealthBarCurrent);
        targetStat.RemoveOnMaxValueChange(UpdateHealthBarMax);
    }

    void UpdateHealthBarCurrent(float currentValue)
    {
        m_Slider.value = currentValue;
        m_barFill.color = m_gradient.Evaluate(m_Slider.normalizedValue);
    }

    void UpdateHealthBarMax(float maxValue)
    {
        m_Slider.maxValue = maxValue;
        m_barFill.color = m_gradient.Evaluate(m_Slider.normalizedValue);
    }
}