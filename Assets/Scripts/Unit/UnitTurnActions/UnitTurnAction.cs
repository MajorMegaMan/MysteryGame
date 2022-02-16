using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TurnActions
{
    static ITurnAction[] _allTurnActions;

    static TurnActions()
    {
        _allTurnActions = new ITurnAction[(int)(TurnActionEnum.last) + 1];

        _allTurnActions[(int)TurnActionEnum.pass] = new PassTurnAction();
        _allTurnActions[(int)TurnActionEnum.attack] = new AttackTurnAction();
        _allTurnActions[(int)TurnActionEnum.move] = new MoveTurnAction();
    }

    static public ITurnAction GetTurnAction(TurnActionEnum turnActionEnum)
    {
        return _allTurnActions[(int)turnActionEnum];
    }
}

public enum TurnActionEnum
{
    pass,
    attack,
    move,

    last = move
}
public abstract class BaseAction : ITurnAction
{
    void ITurnAction.Perform(ITurnTaker turnTaker)
    {
        Unit unit = turnTaker as Unit;
        unit.LogAction(GetActionEnum().ToString());
        Perform(unit, turnTaker);
    }

    protected abstract void Perform(Unit unit, ITurnTaker unitAsTurnTaker);

    public abstract TurnActionEnum GetActionEnum();
}
public class PassTurnAction : BaseAction
{
    protected override void Perform(Unit unit, ITurnTaker unitAsTurnTaker)
    {
        // immediately end turn because there was no action to take
        unitAsTurnTaker.EndTurn();
    }

    public override TurnActionEnum GetActionEnum()
    {
        return TurnActionEnum.pass;
    }
}

public class MoveTurnAction : BaseAction
{
    protected override void Perform(Unit unit, ITurnTaker unitAsTurnTaker)
    {
        // can force movement as the checks have already been made in find input
        unit.ForceMoveToNeighbourTile(unit.currentLookDirection);

        // immediately end turn because this was a movement Action
        unitAsTurnTaker.EndTurn();
    }

    public override TurnActionEnum GetActionEnum()
    {
        return TurnActionEnum.move;
    }
}

public class AttackTurnAction : BaseAction
{
    protected override void Perform(Unit unit, ITurnTaker unitAsTurnTaker)
    {
        unit.Attack(unit.currentLookDirection);
    }

    public override TurnActionEnum GetActionEnum()
    {
        return TurnActionEnum.attack;
    }
}
