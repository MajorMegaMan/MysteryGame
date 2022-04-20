using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewUnitStats.Inventory
{
    public class InventoryBase<TOwnerClass>
    {
        [System.NonSerialized] TOwnerClass m_unitOwner = default;
        public TOwnerClass unitOwner { get { return m_unitOwner; } }

        public InventoryBase(TOwnerClass unitOwner)
        {
            m_unitOwner = unitOwner;
        }
    }
}
