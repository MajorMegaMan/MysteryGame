using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewUnitStats.Inventory
{
    public interface IInventoryItem<TOwnerClass>
    {
        string itemName { get; }
        int GetUniqueItemID();

        int GetMaxStackCount();

        void UseAction(TOwnerClass targetOwner);

        Mesh GetTokenMesh();

        Material[] GetTokenMaterials();
    }
}
