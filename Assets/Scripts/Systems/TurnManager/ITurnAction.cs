using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITurnAction
{
    void Perform(ITurnTaker unit);
}
