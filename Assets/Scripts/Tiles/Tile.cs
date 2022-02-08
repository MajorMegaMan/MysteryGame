using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public enum NeighbourDirection
    {
        up,
        right,
        left,
        down,

        upRight,
        upLeft,
        downRight,
        downLeft
    }

    public enum Access
    {
        open,
        partial,
        blocked
    }

    // Grid Data
    int m_gridX = 0;
    int m_gridY = 0;

    Tile[] m_neighbours = new Tile[8];
    TileGrid m_grid = null;
    TileFloorMesh m_floorMesh = null;
    TileProfile m_tileProfile = null;
    Vector3 m_position;

    // Gameplay Data
    Unit m_currentUnit = null;
    Room m_room = null;

    // getters
    public TileProfile profile { get { return m_tileProfile; } }
    public Vector3 position { get { return m_position; } }
    public Access access { get { return m_tileProfile.access; } }

    public int x { get { return m_gridX; } }
    public int y { get { return m_gridY; } }

    public Tile(int x, int y, TileGrid grid)
    {
        Vector3 position = grid.origin + grid.FindHalfSize();
        position.x += x * grid.tileSize + grid.tileSize / 2.0f;
        position.z += y * grid.tileSize + grid.tileSize / 2.0f;
        position -= grid.FindHalfSize();
        m_position = position;

        m_gridX = x;
        m_gridY = y;

        SetGrid(grid);
    }

    public void SetGrid(TileGrid grid)
    {
        m_grid = grid;
    }

    public void SetFloorMesh(TileFloorMesh tileFloorMesh)
    {
        m_floorMesh = tileFloorMesh;
    }

    // Set the profile of this tile. Should also call UpdateUVs in the floor mesh when it is ready to be updated and finished needing to be changed
    public void InitialiseProfile(TileProfile tileProfile)
    {
        m_tileProfile = tileProfile;
        UpdateColour();
    }

    void UpdateColour()
    {
        m_floorMesh.SetTileUVs(profile.UVArray, m_gridX, m_gridY);
    }

    public void SetProfile(TileProfile tileProfile)
    {
        m_tileProfile = tileProfile;
        UpdateColour();
        m_floorMesh.UpdateUVs();
    }

    public void ConnectToTile(Tile target, NeighbourDirection neighbourDirection)
    {
        m_neighbours[(int)neighbourDirection] = target;
    }

    public Tile GetNeighbour(NeighbourDirection neighbourDirection)
    {
        return m_neighbours[(int)neighbourDirection];
    }

    public Vector3 GetNeighbourOffset(NeighbourDirection neighbourDirection)
    {
        return ConvertDirection(neighbourDirection) * m_grid.tileSize;
    }

    public void SetCurrentUnit(Unit target)
    {
        m_currentUnit = target;
    }

    public Unit GetCurrentUnit()
    {
        return m_currentUnit;
    }

    public void SetRoom(Room room)
    {
        m_room = room;
    }

    public Room GetRoom()
    {
        return m_room;
    }

    public void MouseEnter()
    {
        
    }

    public void MouseExit()
    {

    }

    public void Click()
    {
        
    }

    static Vector3[] _dirArray;

    static Tile()
    {
        _dirArray = new Vector3[8];

        _dirArray[(int)NeighbourDirection.up] = Vector3.forward;
        _dirArray[(int)NeighbourDirection.right] = Vector3.right;
        _dirArray[(int)NeighbourDirection.left] = -Vector3.right;
        _dirArray[(int)NeighbourDirection.down] = -Vector3.forward;

        _dirArray[(int)NeighbourDirection.upRight] = (Vector3.forward + Vector3.right).normalized;
        _dirArray[(int)NeighbourDirection.upLeft] = (Vector3.forward - Vector3.right).normalized;
        _dirArray[(int)NeighbourDirection.downRight] = -_dirArray[(int)NeighbourDirection.upLeft];
        _dirArray[(int)NeighbourDirection.downLeft] = -_dirArray[(int)NeighbourDirection.upRight];
    }

    static public Vector3 ConvertDirection(NeighbourDirection direction)
    {
        return _dirArray[(int)direction];
    }
}
