using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewUnitStats
{
    public class NewTurnManager : SingletonBase<NewTurnManager>
    {
        TurnManager<UnitTokenStub> m_turnManager;

        public NewTurnManager()
        {
            m_turnManager = new TurnManager<UnitTokenStub>();
        }

        /////////////////////////////////////////////////////////////////////////////
        ///

        public int targetTurnOrderCount { get { return instance.m_turnManager.targetTurnOrderCount; } set { instance.m_turnManager.targetTurnOrderCount = value; } }

        // Debug Getters
        public static UnitTokenStub[] turnOrder { get { return instance.m_turnManager.turnOrder; } }
        public static UnitTokenStub currentUnitsTurn { get { return instance.m_turnManager.currentUnitsTurn; } }

        public static void Update()
        {
            instance.m_turnManager.Update();
        }

        // ends the current unit's action and proceeds to wait for the next unit's input
        public static void EndCurrentTurn()
        {
            instance.m_turnManager.EndCurrentTurn();
        }

        public static void FindTurnOrder()
        {
            instance.m_turnManager.FindTurnOrder();
        }

        public static void AddUnit(UnitTokenStub unit)
        {
            instance.m_turnManager.AddUnit(unit);
        }

        public static void RemoveUnit(UnitTokenStub unit)
        {
            instance.m_turnManager.RemoveUnit(unit);
        }
    }
}
