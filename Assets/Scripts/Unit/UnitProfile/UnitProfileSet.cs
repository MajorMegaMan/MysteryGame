using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is only used in editor.
public class UnitProfileSet : ScriptableObject
{
    [SerializeField] List<ScriptableUnitProfile> m_serialisedUnitProfileList = new List<ScriptableUnitProfile>();

    public int count { get { return m_serialisedUnitProfileList.Count; } }

    public void AddUnitProfile(ScriptableUnitProfile profile)
    {
        m_serialisedUnitProfileList.Add(profile);
    }

    public void RemoveUnitProfile(ScriptableUnitProfile target)
    {
        m_serialisedUnitProfileList.Remove(target);
    }
}
