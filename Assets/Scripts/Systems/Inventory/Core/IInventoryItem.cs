using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryItem<TOwnerClass>
{
    string itemName { get; }
    int GetID();

    int GetMaxStackCount();

    void Use(TOwnerClass targetOwner);

    Mesh GetMesh();

    Material[] GetMaterials();
}
