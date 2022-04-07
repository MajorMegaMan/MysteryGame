using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BaseItem : IInventoryItem<Unit>
{
    [SerializeField] string m_itemName = "NewCounsumable";
    [SerializeField] TempItemID m_itemID = 0;

    [SerializeField] Mesh m_tokenMesh = null;
    [SerializeField] Material[] m_tokenMaterials = null;

    public abstract void UseAction(Unit targetUnit);

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
