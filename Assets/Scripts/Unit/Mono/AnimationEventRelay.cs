using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventRelay : MonoBehaviour
{
    public Unit targetUnit = null;

    void RelayEvent()
    {
        targetUnit.EndTurnAction();
    }
}
