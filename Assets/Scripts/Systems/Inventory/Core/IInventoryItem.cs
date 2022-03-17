using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryItem<T>
{
    string itemName { get; }
    int GetID();

    int GetMaxStackCount();

    void Use(T targetUnit);
}
