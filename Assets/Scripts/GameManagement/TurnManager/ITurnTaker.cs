using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITurnTaker
{
    float GetTurnValue();

    bool IsEngaged();

    ITurnAction FindUnitTurnAction();

    void EndTurn();
}
