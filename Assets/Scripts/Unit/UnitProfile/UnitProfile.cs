using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitProfile", menuName = "Unit Profile")]
public class UnitProfile : ScriptableObject
{
    [SerializeField] string m_unitName = "Unit";
    [SerializeField] GameObject m_modelGameObjectPrefab = null;
    [SerializeField] UnitStats m_baseStats = UnitStats.basic;
    [SerializeField] UnitStats m_statGrowth = UnitStats.basicGrowth;

    // getters
    public UnitStats baseStats { get { return m_baseStats; } }
    public UnitStats statGrowth { get { return m_statGrowth; } }
    public string unitName { get { return m_unitName; } }
    public ModelObject modelObjectPrefab { get { return m_modelGameObjectPrefab.GetComponent<ModelObject>(); } }

    public static class Edit
    {
        public static bool SetUnitName(UnitProfile unitProfile, string unitName)
        {
            bool changed = unitProfile.m_unitName.Equals(unitName);
            unitProfile.m_unitName = unitName;
            return changed;
        }

        public static bool SetModelGameObject(UnitProfile unitProfile, GameObject modelObject)
        {
            bool changed = unitProfile.m_modelGameObjectPrefab.Equals(modelObject);
            unitProfile.m_modelGameObjectPrefab = modelObject;
            return changed;
        }

        public static GameObject GetModelGameObject(UnitProfile unitProfile)
        {
            return unitProfile.m_modelGameObjectPrefab;
        }

        public static bool SetBaseStats(UnitProfile unitProfile, UnitStats baseStats)
        {
            bool changed = unitProfile.m_baseStats.Equals(baseStats);
            unitProfile.m_baseStats = baseStats;
            return changed;
        }

        public static bool SetGrowthStats(UnitProfile unitProfile, UnitStats statGrowth)
        {
            bool changed = unitProfile.m_statGrowth.Equals(statGrowth);
            unitProfile.m_statGrowth = statGrowth;
            return changed;
        }
    }
}