using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelObject : MonoBehaviour, IPooledObject
{
    Unit m_currentUnit = null;

    public void SetIsActiveInPool(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void SetCurrentUnit(Unit unit)
    {
        m_currentUnit = unit;
        transform.parent = unit.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void DetachCurrentUnit(Transform container)
    {
        m_currentUnit = null;
        transform.parent = container;

        SetIsActiveInPool(false);
    }

    // This is called by animation events to end the unit's turn.
    void RelayEvent()
    {
        m_currentUnit.EndTurnAction();
    }
}
