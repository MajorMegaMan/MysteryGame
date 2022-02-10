using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitTurnAction
{
    void Perform(Unit unit);
}

public static class TurnActions
{
    static IUnitTurnAction[] _allTurnActions;

    static TurnActions()
    {
        _allTurnActions = new IUnitTurnAction[(int)(TurnActionEnum.last) + 1];

        _allTurnActions[(int)TurnActionEnum.pass] = new PassTurnAction();
        _allTurnActions[(int)TurnActionEnum.move] = new MoveTurnAction();
    }

    static public IUnitTurnAction GetTurnAction(TurnActionEnum turnActionEnum)
    {
        return _allTurnActions[(int)turnActionEnum];
    }
}

public enum TurnActionEnum
{
    pass,
    move,

    last = move
}

public class PassTurnAction : IUnitTurnAction
{
    void IUnitTurnAction.Perform(Unit unit)
    {
        // immediately end turn because there was no action to take
        unit.turnManager.EndCurrentTurn();
    }
}

public class MoveTurnAction : IUnitTurnAction
{
    void IUnitTurnAction.Perform(Unit unit)
    {
        // can force movement as the checks have already been made in find input
        unit.ForceMoveToNeighbourTile(unit.currentLookDirection);

        // immediately end turn because this was a movement Action
        unit.turnManager.EndCurrentTurn();
    }
}
