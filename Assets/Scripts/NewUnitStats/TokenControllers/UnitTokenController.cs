using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewUnitStats
{
    public abstract class UnitTokenController
    {
        // This is a factory pattern. The idea is that there is a managed list of UnitControllers that will be created.
        // All UnitControllers are singletons. even derived classes.
        static List<UnitTokenController> _unitControllers;

        static UnitTokenController()
        {
            _unitControllers = new List<UnitTokenController>();
        }

        // As all controllers singletons. A search for the type of existing controllers must be made first.
        // if the UnitContoller Type already exists. it will not be created, otherwise it is created and then added to the list.
        // returns the newly created controller.
        // or null if the creation failed.
        static public UnitTokenController CreateController<T>() where T : UnitTokenController, new()
        {
            foreach (UnitTokenController controller in _unitControllers)
            {
                if (controller.GetType() == typeof(T))
                {
                    return null;
                }
            }
            T newcontroller = new T();
            _unitControllers.Add(newcontroller);
            return newcontroller;
        }

        static public UnitTokenController GetUnitController(int index)
        {
            return _unitControllers[index];
        }

        static public T GetUnitController<T>(int index) where T : UnitTokenController
        {
            return _unitControllers[index] as T;
        }

        static public int GetUnitControllersSize()
        {
            return _unitControllers.Count;
        }

        // This shouldn't be accessable, use CreateController instead
        protected UnitTokenController()
        {

        }

        public abstract ITurnAction FindUnitTurnAction(UnitTokenStub unit, out Tile.NeighbourDirection direction);


        #region DirectionTesting
        public static bool CheckNeighbourTileMove(GameMapTile currentTile, Tile.NeighbourDirection direction, out GameMapTile neighbourTile, GameMapTile.Access tileAccess)
        {
            neighbourTile = currentTile.GetNeighbour<GameMapTile>(direction);
            if (neighbourTile == null)
            {
                return false;
            }

            if (tileAccess < neighbourTile.access || neighbourTile.GetToken(TempTokenID.unit) != null)
            {
                return false;
            }

            // Check if trying to move diagonally
            if (direction > Tile.NeighbourDirection.down)
            {
                if (CheckDiagonalAccess(currentTile, direction, tileAccess))
                {
                    // player can move into tile
                    return true;
                }

                return false;
            }
            else
            {
                // can freely move
                return true;
            }
        }

        // returns true if player can cut corners
        static bool CheckDiagonalAccess(GameMapTile currentTile, Tile.NeighbourDirection direction, GameMapTile.Access tileAccess)
        {
            if (tileAccess > GameMapTile.Access.partial)
            {
                return true;
            }

            switch (direction)
            {
                case Tile.NeighbourDirection.upLeft:
                    {
                        return CheckPartialAccess(currentTile, Tile.NeighbourDirection.up, Tile.NeighbourDirection.left);
                    }
                case Tile.NeighbourDirection.upRight:
                    {
                        return CheckPartialAccess(currentTile, Tile.NeighbourDirection.up, Tile.NeighbourDirection.right);
                    }
                case Tile.NeighbourDirection.downLeft:
                    {
                        return CheckPartialAccess(currentTile, Tile.NeighbourDirection.down, Tile.NeighbourDirection.left);
                    }
                case Tile.NeighbourDirection.downRight:
                    {
                        return CheckPartialAccess(currentTile, Tile.NeighbourDirection.down, Tile.NeighbourDirection.right);
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        // returns true if both neighbour directions are partial
        static bool CheckPartialAccess(GameMapTile currentTile, Tile.NeighbourDirection vertDirection, Tile.NeighbourDirection horiDirection)
        {
            GameMapTile vertNeighbour = currentTile.GetNeighbour<GameMapTile>(vertDirection);
            GameMapTile horiNeighbour = currentTile.GetNeighbour<GameMapTile>(horiDirection);

            return vertNeighbour.access <= GameMapTile.Access.partial && horiNeighbour.access <= GameMapTile.Access.partial;
        }
        #endregion
    }
}
