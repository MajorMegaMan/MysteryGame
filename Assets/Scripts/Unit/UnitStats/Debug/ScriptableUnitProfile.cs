using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MysterySystems.UnitStats.Serialised;

public class ScriptableUnitProfile : ScriptableObject
{
    public SerialisedUnitProfile testProfile;
    public ModelObject modelObjectPrefab;

    public void Init()
    {
        testProfile.Init();
    }
}
