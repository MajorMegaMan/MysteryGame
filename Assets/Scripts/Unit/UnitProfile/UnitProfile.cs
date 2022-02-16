using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitProfile", menuName = "Unit Profile")]
public class UnitProfile : ScriptableObject
{
    [SerializeField] ModelObject m_modelObjectPrefab = null;

    [SerializeField] string m_unitName = "Unit";

    [SerializeField] UnitStats m_baseStats = UnitStats.basic;
    [SerializeField] UnitStats m_statGrowth = UnitStats.basicGrowth;

    // getters
    public UnitStats baseStats { get { return m_baseStats; } }
    public UnitStats statGrowth { get { return m_statGrowth; } }
    public string unitName { get { return m_unitName; } }
    public ModelObject modelObjectPrefab { get { return m_modelObjectPrefab; } }
}