using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewUnitStats
{
    public class UnitTokenTurnActions
    {
        static ITurnAction[] _allTurnActions;

        static UnitTokenTurnActions()
        {
            System.Array enumValues = System.Enum.GetValues(typeof(TurnActionEnum));
            _allTurnActions = new ITurnAction[enumValues.Length];

            _allTurnActions[(int)TurnActionEnum.pass] = new PassTurnAction();
            _allTurnActions[(int)TurnActionEnum.attack] = new AttackTurnAction();
            _allTurnActions[(int)TurnActionEnum.move] = new MoveTurnAction();
        }

        static public ITurnAction GetTurnAction(TurnActionEnum turnActionEnum)
        {
            return _allTurnActions[(int)turnActionEnum];
        }

        public enum TurnActionEnum
        {
            pass,
            attack,
            move,
        }

        public abstract class BaseAction<TTurnTaker> : ITurnAction where TTurnTaker : ITurnTaker
        {
            void ITurnAction.Perform(ITurnTaker turnTaker)
            {
                TTurnTaker unit = (TTurnTaker)turnTaker;
                //unit.LogAction(GetActionEnum().ToString());
                Perform(unit, turnTaker);
            }

            protected abstract void Perform(TTurnTaker unit, ITurnTaker unitAsTurnTaker);
        }
        public class PassTurnAction : BaseAction<UnitTokenStub>
        {
            protected override void Perform(UnitTokenStub unit, ITurnTaker unitAsTurnTaker)
            {
                // immediately end turn because there was no action to take
                unitAsTurnTaker.EndTurn();
            }
        }

        public class MoveTurnAction : BaseAction<UnitTokenStub>
        {
            protected override void Perform(UnitTokenStub unit, ITurnTaker unitAsTurnTaker)
            {
                // can force movement as the checks have already been made in find input
                //unit.ForceMoveToNeighbourTile(unit.currentLookDirection);
                unit.MoveToNextTile(unit.currentTile.GetNeighbour<GameMapTile>(unit.currentLookDirection));

                // immediately end turn because this was a movement Action
                unitAsTurnTaker.EndTurn();
            }
        }

        public class AttackTurnAction : BaseAction<UnitTokenStub>
        {
            protected override void Perform(UnitTokenStub unit, ITurnTaker unitAsTurnTaker)
            {
                //unit.Attack(unit.currentLookDirection);
                Debug.Log("Should be an attack");
                // DEBUG: Ending turn for testing purposes.
                unitAsTurnTaker.EndTurn();
            }
        }
    }
}
