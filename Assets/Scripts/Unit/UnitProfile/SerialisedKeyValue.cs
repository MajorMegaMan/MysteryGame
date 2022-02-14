using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SerialisedKeyValue<Key,Value>
{
    public Key key;
    public Value value;
}
