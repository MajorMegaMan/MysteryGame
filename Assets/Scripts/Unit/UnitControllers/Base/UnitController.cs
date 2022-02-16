using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitController
{
    // This is a factory pattern. The idea is that there is a managed list of UnitControllers that will be created.
    // All UnitControllers are singletons. even derived classes.
    static List<UnitController> _unitControllers;

    static UnitController()
    {
        _unitControllers = new List<UnitController>();
    }

    // As all controllers singletons. A search for the type of existing controllers must be made first.
    // if the UnitContoller Type already exists. it will not be created, otherwise it is created and then added to the list.
    // returns the newly created controller.
    // or null if the creation failed.
    static public UnitController CreateController<T>() where T : UnitController, new()
    {
        foreach(UnitController controller in _unitControllers)
        {
            if(controller.GetType() == typeof(T))
            {
                return null;
            }
        }
        T newcontroller = new T();
        _unitControllers.Add(newcontroller);
        return newcontroller;
    }

    static public UnitController GetUnitController(int index)
    {
        return _unitControllers[index];
    }

    static public T GetUnitController<T>(int index) where T : UnitController
    {
        return _unitControllers[index] as T;
    }

    static public int GetUnitControllersSize()
    {
        return _unitControllers.Count;
    }

    // This shouldn't be accessable, use CreateController instead
    protected UnitController()
    {

    }

    public abstract ITurnAction FindUnitTurnAction(Unit unit, out Tile.NeighbourDirection direction);
}