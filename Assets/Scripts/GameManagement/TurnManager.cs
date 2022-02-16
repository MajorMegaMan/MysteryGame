using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager
{
    float m_speedPerTurn = 10.0f;

    Unit m_player = null;

    // Simple list that contains all references to unit. Doesn't care what order it is
    List<UnitTurn> m_allUnits = new List<UnitTurn>();

    List<UnitTurn> m_tempTurnOrder = new List<UnitTurn>();

    // Holds a reference to each unit with it's upcoming turn, Varies greatly and often
    LinkedList<Unit> m_turnOrder = new LinkedList<Unit>();

    delegate void VoidAction();
    VoidAction m_waitAction;

    // Debug Getters
    public UnitTurn[] allUnitTurns { get { return m_allUnits.ToArray(); } }
    public Unit[] turnOrder { get { Unit[] result = new Unit[m_turnOrder.Count]; m_turnOrder.CopyTo(result, 0); return result; } }
    public Unit currentUnitsTurn { get { return m_turnOrder.First.Value; } }

    public TurnManager()
    {
        m_waitAction = WaitForInput;
    }

    // This should be called once per frame. It Invokes the current turn action that the unit is trying to perform.
    // This could be waitning for an input, or waiting for the Unit to end it's action so the next unit can start waiting for input.
    // Player units wait for user input.
    // AI units will have an AI logic to automatically find input.
    public void Update()
    {
        m_waitAction.Invoke();
    }

    // waits for input from the current unit.
    void WaitForInput()
    {
        Unit unit = m_turnOrder.First.Value;

        // check if the unit is currently engaged in something other than a turn related action
        // For example, if the unit is currently traversing between tiles.
        if (unit.isMoving)
        {
            return;
        }

        // Wait for input
        IUnitTurnAction unitTurnAction = unit.FindUnitTurnAction();
        if (unitTurnAction != null)
        {
            // Current unit do action
            m_waitAction = WaitForActionEnd;
            unitTurnAction.Perform(unit);
        }
    }

    // Empty action, waiting for a unit to call end current turn
    void WaitForActionEnd()
    {
        
    }

    // ends the current unit's action and proceeds to wait for the next unit's input
    public void EndCurrentTurn()
    {
        m_turnOrder.RemoveFirst();

        if (m_turnOrder.Count == 0)
        {
            FindTurnOrder();
        }

        m_waitAction = WaitForInput;

        // Call again so that units won't have to wait for a frame before being able to do something
        WaitForInput();
    }

    // changes the turnorder based on unit's speed value.
    // Searches all units until the player is found in the turn order.
    public void FindTurnOrder()
    {
        // Set up lists
        m_turnOrder.Clear();

        m_tempTurnOrder.Clear();
        bool unitReachedValue = false;
        while(!unitReachedValue)
        {
            // increment turn values
            foreach (UnitTurn unitTurn in m_allUnits)
            {
                unitTurn.IncrementTurnValue();
                if(unitTurn.turnValue >= m_speedPerTurn)
                {
                    m_tempTurnOrder.Add(unitTurn);
                    unitReachedValue = true;
                }
            }
        }

        // sort and add to turn order
        m_tempTurnOrder.Sort(ITurnSort.comparer);
        foreach (UnitTurn unitTurn in m_tempTurnOrder)
        {
            unitTurn.ProcessTurnValue(m_speedPerTurn);
            m_turnOrder.AddLast(unitTurn.unit);
        }
    }

    // Sets the player that the user is controlling. We need this as it's the stop point when searching for turn order.
    // If the player has not already been added to all units it will be.
    public void SetPlayer(Unit unit)
    {
        m_player = unit;

        // Search all units for player, if it does not exist add it to all units
        foreach(UnitTurn unitTurn in m_allUnits)
        {
            if(unitTurn.unit == unit)
            {
                return;
            }
        }

        AddUnit(unit);
    }

    public void AddUnit(Unit unit)
    {
        m_allUnits.Add(new UnitTurn(unit));
        m_turnOrder.AddLast(unit);
    }

    // Need to find the unit in m_all units before removing it.
    // TODO: Switch to object pooling
    public void RemoveUnit(Unit unit)
    {
        for(int i = 0; i < m_allUnits.Count; i++)
        {
            if(m_allUnits[i].unit == unit)
            {
                m_allUnits.RemoveAt(i);
                break;
            }
        }
        m_turnOrder.Remove(unit);
    }
}

[System.Serializable]
public class UnitTurn
{
    Unit m_unit;

    float m_turnValue = 0.0f;

    public Unit unit { get { return m_unit; } }
    public float turnValue { get { return m_turnValue; } }

    public UnitTurn(Unit unit)
    {
        m_unit = unit;
    }

    public void IncrementTurnValue()
    {
        m_turnValue += unit.usableStats.speed;
    }

    public bool TryAddToTurn(List<UnitTurn> turnOrder, float speedPerTurn)
    {

        if(m_turnValue > speedPerTurn)
        {
            turnOrder.Add(this);
            return true;
        }
        return false;
    }

    public void ProcessTurnValue(float speedPerTurn)
    {
        m_turnValue -= speedPerTurn;
    }
}

public class ITurnSort : IComparer<UnitTurn>
{
    static ITurnSort _comparer;

    public static ITurnSort comparer { get { return _comparer; } }

    ITurnSort() { }

    static ITurnSort()
    {
        _comparer = new ITurnSort();
    }

    int IComparer<UnitTurn>.Compare(UnitTurn lhs, UnitTurn rhs)
    {
        if(lhs.turnValue < rhs.turnValue)
        {
            return 1;
        }
        else if (lhs.turnValue > rhs.turnValue)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}