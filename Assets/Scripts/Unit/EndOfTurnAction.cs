using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public interface IEndOfTurnAction
//{
//    // This will be called at the end of the unit's turn
//    void PerformAction();
//}
//
//public abstract class UnitEndTurnCondition : IEndOfTurnAction
//{
//    UsableUnitStats m_targetUnit = null;
//
//    protected UsableUnitStats targetUnit { get { return m_targetUnit; } }
//
//    public void Init(UsableUnitStats targetUnit)
//    {
//        m_targetUnit = targetUnit;
//        Setup();
//    }
//
//    public abstract void Setup();
//
//    public abstract void PerformAction();
//}
//
//public class HungerCondition : UnitEndTurnCondition
//{
//    int m_stepCount = 0;
//    int m_hungerCycle = 3;
//
//    float m_hungerValue = 100.0f;
//
//    public override void Setup()
//    {
//        
//    }
//
//    public override void PerformAction()
//    {
//        m_stepCount++;
//        if(m_stepCount > m_hungerCycle)
//        {
//            // Do hunger thing
//        }
//    }
//}