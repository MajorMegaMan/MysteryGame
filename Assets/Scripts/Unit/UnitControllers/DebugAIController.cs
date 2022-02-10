using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugAIController : UnitController
{
    public override IUnitTurnAction FindUnitTurnAction(Unit unit, out Tile.NeighbourDirection direction)
    {
        // Try to find a random direction to move.
        int randDirInt;
        for(int i = 0; i < 20; i++)
        {
            randDirInt = Random.Range(0, 8);
            Tile.NeighbourDirection randDir = (Tile.NeighbourDirection)randDirInt;
            if (unit.CheckNeighbourTileMove(randDir, out Tile neighbourTile))
            {
                direction = randDir;
                return TurnActions.GetTurnAction(TurnActionEnum.move);
            }
        }

        // if no direction can be found. Pass the turn
        direction = unit.currentLookDirection;
        return TurnActions.GetTurnAction(TurnActionEnum.pass);
    }
}
