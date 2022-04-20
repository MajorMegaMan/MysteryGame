using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewUnitStats.Inventory
{
    public abstract class BaseItemStub : ScriptableObject, IInventoryItem<CharacterInfo>
    {
        [SerializeField] string m_itemName = "NewCounsumable";
        [SerializeField] TempItemID m_itemID = 0;

        [SerializeField] Mesh m_tokenMesh = null;
        [SerializeField] Material[] m_tokenMaterials = null;

        public abstract void UseAction(CharacterInfo targetUnit);

        public string itemName { get { return m_itemName; } }

        public int GetUniqueItemID()
        {
            return (int)m_itemID;
        }

        public abstract int GetMaxStackCount();

        public Mesh GetTokenMesh()
        {
            return m_tokenMesh;
        }

        public Material[] GetTokenMaterials()
        {
            return m_tokenMaterials;
        }
    }
}
