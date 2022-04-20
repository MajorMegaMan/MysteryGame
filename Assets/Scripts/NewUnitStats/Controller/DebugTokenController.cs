using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewUnitStats
{
    public class DebugTokenController : UnitTokenController
    {
        public override ITurnAction FindUnitTurnAction(UnitTokenStub unit, out Tile.NeighbourDirection direction)
        {
            direction = unit.currentLookDirection;
            return UnitTokenTurnActions.GetTurnAction(UnitTokenTurnActions.TurnActionEnum.pass);
        }
    }
}
