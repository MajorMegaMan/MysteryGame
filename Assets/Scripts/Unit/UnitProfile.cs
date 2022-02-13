using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitProfile", menuName = "Unit Profile")]
public class UnitProfile : ScriptableObject
{
    [SerializeField] ModelObject m_modelObjectPrefab = null;

    [SerializeField] UnitStats m_baseStats = UnitStats.basic;

    // getters
    public float baseSpeed { get { return m_baseStats.speed; } }
    public ModelObject modelObjectPrefab { get { return m_modelObjectPrefab; } }
}