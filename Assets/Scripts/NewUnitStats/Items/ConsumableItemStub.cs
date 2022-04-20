using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewUnitStats.Inventory
{
    [CreateAssetMenu(fileName = "New Consumable Item", menuName = "NewUnitTests/Consumable Item")]
    public class ConsumableItemStub : BaseItemStub
    {
        [SerializeField] int m_maxStackCount = 5;
        public override void UseAction(CharacterInfo targetUnit)
        {

        }

        public override int GetMaxStackCount()
        {
            return m_maxStackCount;
    }
    }
}
