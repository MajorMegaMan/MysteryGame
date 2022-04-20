using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewUnitStats
{
    [CreateAssetMenu(fileName = "New Unit Profile", menuName = "NewUnitTests/Unit Profile")]
    public class UnitProfile : ScriptableObject
    {
        [SerializeField] string m_profileName = "New Unit Profile";

        [SerializeField] UnitStats m_baseValues = new UnitStats();
        [SerializeField] UnitStats m_growthValues = new UnitStats();

        public UnitStats baseValues { get { return m_baseValues; } }
        public UnitStats growthValues { get { return m_growthValues; } }
    }
}
