using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitProfile", menuName = "Unit Profile")]
public class UnitProfile : ScriptableObject
{
    [SerializeField] ModelObject m_modelObjectPrefab = null;

    [SerializeField] UnitStats m_baseStats = UnitStats.basic;
    [SerializeField] UnitStats m_statGrowth = UnitStats.basicGrowth;

    // getters
    public UnitStats baseStats { get { return m_baseStats; } }
    public UnitStats statGrowth { get { return m_statGrowth; } }
    public ModelObject modelObjectPrefab { get { return m_modelObjectPrefab; } }
}