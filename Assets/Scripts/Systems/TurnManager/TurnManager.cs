using System.Collections;
using System.Collections.Generic;

public class TurnManager<T> where T : ITurnTaker
{
    float m_speedPerTurn = 10.0f;
    int m_targetTurnOrderCount = 8;

    // Simple list that contains all references to unit. Doesn't care what order it is
    List<UnitTurnPlacementRef<T>> m_allUnits = new List<UnitTurnPlacementRef<T>>();

    List<UnitTurnPlacementRef<T>> m_tempTurnOrder = new List<UnitTurnPlacementRef<T>>();

    // Holds a reference to each unit with it's upcoming turn, Varies greatly and often
    LinkedList<T> m_turnOrder = new LinkedList<T>();

    delegate void VoidAction();
    VoidAction m_waitAction;

    TurnSorter<T> m_comparer = new TurnSorter<T>();

    int m_overflowProtectionCounter = 0;
    int m_overflowProtectionLimit = 50;

    public int targetTurnOrderCount { get { return m_targetTurnOrderCount; } set { m_targetTurnOrderCount = value; } }

    // Debug Getters
    public T[] turnOrder { get { T[] result = new T[m_turnOrder.Count]; m_turnOrder.CopyTo(result, 0); return result; } }
    public T currentUnitsTurn { get { return m_turnOrder.First.Value; } }

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
        m_overflowProtectionCounter = 0;
        m_waitAction.Invoke();
    }

    // waits for input from the current unit.
    void WaitForInput()
    {
        T unit = m_turnOrder.First.Value;

        // check if the unit is currently engaged in something other than a turn related action
        // For example, if the unit is currently traversing between tiles.
        if (unit.IsEngaged())
        {
            return;
        }

        // Wait for input
        ITurnAction unitTurnAction = unit.FindUnitTurnAction();
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

        if (m_turnOrder.Count < m_targetTurnOrderCount)
        {
            FindTurnOrder();
        }

        m_waitAction = WaitForInput;

        // Call again so that units won't have to wait for a frame before being able to do something
        // This can cause a stack overflow
        m_overflowProtectionCounter++;
        if (m_overflowProtectionCounter < m_overflowProtectionLimit)
        {
            WaitForInput();
        }
    }

    // changes the turnorder based on unit's speed value.
    // Searches all units until the player is found in the turn order.
    public void FindTurnOrder()
    {
        if(m_allUnits.Count == 0)
        {
            // There are no units to try to find a turn order.
            return;
        }

        while (m_turnOrder.Count < m_targetTurnOrderCount)
        {
            // Set up lists
            m_tempTurnOrder.Clear();
            bool unitReachedValue = false;
            while (!unitReachedValue)
            {
                // increment turn values
                foreach (UnitTurnPlacementRef<T> unitTurn in m_allUnits)
                {
                    unitTurn.IncrementTurnValue();
                    if (unitTurn.turnValue >= m_speedPerTurn)
                    {
                        m_tempTurnOrder.Add(unitTurn);
                        unitReachedValue = true;
                    }
                }
            }

            // sort and add to turn order
            m_tempTurnOrder.Sort(m_comparer);
            foreach (UnitTurnPlacementRef<T> unitTurn in m_tempTurnOrder)
            {
                unitTurn.ProcessTurnValue(m_speedPerTurn);
                m_turnOrder.AddLast(unitTurn.unit);
            }
        }
    }

    public void AddUnit(T unit)
    {
        m_allUnits.Add(new UnitTurnPlacementRef<T>(unit));
        m_turnOrder.AddLast(unit);
    }

    // Need to find the unit in m_all units before removing it.
    // TODO: Switch to object pooling
    public void RemoveUnit(T unit)
    {
        for (int i = 0; i < m_allUnits.Count; i++)
        {
            if (m_allUnits[i].unit.Equals(unit))
            {
                m_allUnits.RemoveAt(i);
                break;
            }
        }
        while(m_turnOrder.Contains(unit))
        {
            m_turnOrder.Remove(unit);
        }
        if(m_turnOrder.Count < m_targetTurnOrderCount)
        {
            FindTurnOrder();
        }
    }
}

class UnitTurnPlacementRef<T> where T : ITurnTaker
{
    T m_unit;

    float m_turnValue = 0.0f;

    public T unit { get { return m_unit; } }
    public float turnValue { get { return m_turnValue; } }

    public UnitTurnPlacementRef(T unit)
    {
        m_unit = unit;
    }

    public void IncrementTurnValue()
    {
        m_turnValue += unit.GetTurnValue();
    }

    public bool TryAddToTurn(List<UnitTurnPlacementRef<T>> turnOrder, float speedPerTurn)
    {

        if (m_turnValue > speedPerTurn)
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

public class TurnSorter<T> : IComparer<UnitTurnPlacementRef<T>> where T : ITurnTaker
{
    int IComparer<UnitTurnPlacementRef<T>>.Compare(UnitTurnPlacementRef<T> lhs, UnitTurnPlacementRef<T> rhs)
    {
        if (lhs.turnValue < rhs.turnValue)
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