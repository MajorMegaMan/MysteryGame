using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IInventoryItem<TOwnerClass>
{
    string itemName { get; }
    int GetUniqueItemID();

    int GetMaxStackCount();

    void UseAction(TOwnerClass targetOwner);

    Mesh GetTokenMesh();

    Material[] GetTokenMaterials();
}
