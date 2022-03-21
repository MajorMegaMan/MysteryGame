using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConsumableItem : IInventoryItem<Unit>
{
    [SerializeField] string m_itemName = "NewCounsumable";
    [SerializeField] TempItemID m_itemID = 0;
    [SerializeField] int m_maxStackCount = 99;

    [SerializeField] ConsumableItemEffectType m_effectType = 0;
    [SerializeField] float m_useValue = 1;

    [SerializeField] Mesh m_mesh = null;
    [SerializeField] Material[] m_materials = null;

    public float useValue { get { return m_useValue; } }
    public ConsumableItemEffectType effectType { get { return m_effectType; } }

    public string itemName { get { return m_itemName; } }

    public void Use(Unit targetUnit)
    {
        ConsumableItemEffects.UseConsumable(this, targetUnit);
    }

    public int GetID()
    {
        return (int)m_itemID;
    }

    public int GetMaxStackCount()
    {
        return m_maxStackCount;
    }

    public Mesh GetMesh()
    {
        return m_mesh;
    }

    public Material[] GetMaterials()
    {
        return m_materials;
    }
}
