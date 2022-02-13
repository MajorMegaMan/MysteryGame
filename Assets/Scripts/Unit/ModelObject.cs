using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelObject : PooledObject
{
    Unit m_currentUnit = null;

    public void SetCurrentUnit(Unit unit)
    {
        m_currentUnit = unit;
        transform.parent = unit.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        GetComponent<AnimationEventRelay>().targetUnit = unit;
    }

    public void DetachCurrentUnit(Transform container)
    {
        m_currentUnit = null;
        transform.parent = container;

        SetIsActiveInPool(false);
    }
}
