using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewUnitStats
{
    [CreateAssetMenu(fileName = "New Unit Profile Set", menuName = "NewUnitTests/Unit Profile Set")]
    public class UnitProfileSet : ScriptableObject
    {
        [SerializeField] List<UnitProfile> m_unitProfiles = new List<UnitProfile>();
    }
}
