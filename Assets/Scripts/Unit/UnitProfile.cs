using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitProfile", menuName = "Unit Profile")]
public class UnitProfile : ScriptableObject
{
    [SerializeField] GameObject m_modelObjectPrefab = null;

    [SerializeField] float m_baseSpeed = 5.0f;

    // getters
    public float baseSpeed { get { return m_baseSpeed; } }
    public GameObject modelObjectPrefab { get { return m_modelObjectPrefab; } }
}
