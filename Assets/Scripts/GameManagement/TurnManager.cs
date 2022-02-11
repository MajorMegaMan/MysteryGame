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
    List<Unit> m_turnOrder = new List<Unit>();

    int m_currentUnitIndex = 0;

    delegate void VoidAction();
    VoidAction m_waitAction;

    public void Initialise(Unit player)
    {
        m_waitAction = WaitForInput;

        // Debug
        SetPlayer(player);
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
        Unit unit = m_turnOrder[m_currentUnitIndex];

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
            unitTurnAction.Perform(m_turnOrder[m_currentUnitIndex]);
        }
    }

    // Empty action, waiting for a unit to call end current turn
    void WaitForActionEnd()
    {
        
    }

    // ends the current unit's action and proceeds to wait for the next unit's input
    public void EndCurrentTurn()
    {
        m_currentUnitIndex++;

        if (m_currentUnitIndex == m_turnOrder.Count)
        {
            m_currentUnitIndex = 0;
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

        // add units until the player has been found
        bool foundPlayer = false;
        int tryCount = 0;
        while(!foundPlayer)
        {
            tryCount++;
            if(tryCount > 1000)
            {
                Debug.LogError("failed to establish turn order");
                break;
            }

            // increment turn values
            foreach (UnitTurn unitTurn in m_allUnits)
            {
                unitTurn.IncrementTurnValue();
            }

            m_tempTurnOrder.Clear();
            for (int i = 0; i < m_allUnits.Count; i++)
            {
                UnitTurn unitTurn = m_allUnits[i];
                if (unitTurn.TryAddToTurn(m_tempTurnOrder, m_speedPerTurn))
                {
                    if (unitTurn.unit == m_player)
                    {
                        // found the players turn
                        foundPlayer = true;
                    }
                }
            }

            m_tempTurnOrder.Sort(ITurnSort.comparer);
            foreach (UnitTurn unitTurn in m_tempTurnOrder)
            {
                unitTurn.ProcessTurnValue(m_speedPerTurn);
                m_turnOrder.Add(unitTurn.unit);
            }
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
        m_turnOrder.Add(unit);
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
                return;
            }
        }
    }
}

[System.Serializable]
class UnitTurn
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
        m_turnValue += unit.profile.baseSpeed;
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
            return -1;
        }
        else if (lhs.turnValue > rhs.turnValue)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}