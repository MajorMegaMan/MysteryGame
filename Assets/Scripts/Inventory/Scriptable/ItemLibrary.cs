using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemLibrary", menuName = "ItemLibrary")]
public class ItemLibrary : ScriptableObject
{
    [SerializeField] List<ConsumableItem> m_consumables = new List<ConsumableItem>();

    public ConsumableItem GetConsumableItem(TempItemID tempItemID)
    {
        return m_consumables[(int)tempItemID];
    }
}
