using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : UnitController
{
    public override IUnitTurnAction FindUnitTurnAction(Unit unit, out Tile.NeighbourDirection direction)
    {
        // TODO: Apply checks for what buttons the player is pressing to determine the action that should be taken
        // for now only move or pass
        if(Input.GetMouseButtonDown(0))
        {
            direction = unit.currentLookDirection;
            return TurnActions.GetTurnAction(TurnActionEnum.attack);
        }


        // test for passing turn
        if (Input.GetKey(KeyCode.Space))
        {
            direction = unit.currentLookDirection;
            return TurnActions.GetTurnAction(TurnActionEnum.pass);
        }

        // test for look direction
        if (FindDirection(unit, out direction))
        {
            // test for movement
            if (unit.CheckNeighbourTileMove(direction, out Tile neighbourTile))
            {
                return TurnActions.GetTurnAction(TurnActionEnum.move);
            }
            else
            {
                return null;
            }
        }
        else
        {
            direction = unit.currentLookDirection;
            return null;
        }
    }

    bool FindDirection(Unit unit, out Tile.NeighbourDirection direction)
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal > 0)
        {
            if (vertical > 0)
            {
                direction = Tile.NeighbourDirection.upRight;
            }
            else if (vertical < 0)
            {
                direction = Tile.NeighbourDirection.downRight;
            }
            else
            {
                // No input
                direction = Tile.NeighbourDirection.right;
            }
        }
        else if (horizontal < 0)
        {
            if (vertical > 0)
            {
                direction = Tile.NeighbourDirection.upLeft;
            }
            else if (vertical < 0)
            {
                direction = Tile.NeighbourDirection.downLeft;
            }
            else
            {
                // No input
                direction = Tile.NeighbourDirection.left;
            }
        }
        else
        {
            if (vertical > 0)
            {
                direction = Tile.NeighbourDirection.up;
            }
            else if (vertical < 0)
            {
                direction = Tile.NeighbourDirection.down;
            }
            else
            {
                // No input
                direction = Tile.NeighbourDirection.up;
                return false;
            }
        }
        return true;
    }
}
