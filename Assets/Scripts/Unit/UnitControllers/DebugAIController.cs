using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugAIController : UnitController
{
    public override ITurnAction FindUnitTurnAction(Unit unit, out Tile.NeighbourDirection direction)
    {
        // Find any attack target for debug purposes
        List<Tile> validTiles = new List<Tile>();
        GameMapToken attackTarget = null;
        direction = 0;
        for (int i = 0; i < 8; i++)
        {
            Tile.NeighbourDirection dir = (Tile.NeighbourDirection)i;
            if (unit.CheckNeighbourTileMove(dir, out GameMapTile neighbourTile))
            {
                validTiles.Add(neighbourTile);
            }

            attackTarget = neighbourTile.GetToken(TempTokenID.unit);
            if (attackTarget != null)
            {
                direction = dir;
                return TurnActions.GetTurnAction(TurnActionEnum.attack);
            }
        }

        // Try to find a random direction to move.
        int randDirInt;
        for(int i = 0; i < 20; i++)
        {
            randDirInt = Random.Range(0, 8);
            Tile.NeighbourDirection randDir = (Tile.NeighbourDirection)randDirInt;
            if (unit.CheckNeighbourTileMove(randDir, out GameMapTile neighbourTile))
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
