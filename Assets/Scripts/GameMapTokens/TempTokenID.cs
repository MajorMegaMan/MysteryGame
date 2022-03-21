using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This will eventually be an enum of different types of tokens.
// Unit tokens would need to identify each other to be able to attack each other, while pickupable items shouldn't be attacked.
public enum TempTokenID
{
    unit,
    item
}
