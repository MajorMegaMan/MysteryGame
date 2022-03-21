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

    // Grid Data
    int m_gridX = 0;
    int m_gridY = 0;

    Tile[] m_neighbours = new Tile[8];
    TileGrid m_grid = null;

    Vector3 m_position;

    // getters
    public Vector3 position { get { return m_position; } }

    public int x { get { return m_gridX; } }
    public int y { get { return m_gridY; } }

    public static TTileType CreateNewTile<TTileType>(int x, int y, TileGrid grid) where TTileType : Tile, new()
    {
        TTileType newTile = new TTileType();
        newTile.Init(x, y, grid);
        return newTile;
    }

    // Init must be called otherwise the tile will not function
    void Init(int x, int y, TileGrid grid)
    {
        Vector3 position = grid.origin + grid.FindHalfSize();
        position.x += x * grid.tileSize + grid.tileSize / 2.0f;
        position.z += y * grid.tileSize + grid.tileSize / 2.0f;
        position -= grid.FindHalfSize();
        m_position = position;

        m_gridX = x;
        m_gridY = y;

        SetGrid(grid);
        OnCreate();
    }

    protected virtual void OnCreate()
    {

    }

    public void SetGrid(TileGrid grid)
    {
        m_grid = grid;
    }

    public void ConnectToTile(Tile target, NeighbourDirection neighbourDirection)
    {
        m_neighbours[(int)neighbourDirection] = target;
    }

    public Tile GetNeighbour(NeighbourDirection neighbourDirection)
    {
        return m_neighbours[(int)neighbourDirection];
    }

    public TTileType GetNeighbour<TTileType>(NeighbourDirection neighbourDirection) where TTileType : Tile
    {
        return m_neighbours[(int)neighbourDirection] as TTileType;
    }

    public Vector3 GetNeighbourOffset(NeighbourDirection neighbourDirection)
    {
        return ConvertDirection(neighbourDirection) * m_grid.tileSize;
    }

    public virtual void MouseEnter()
    {
        
    }

    public virtual void MouseExit()
    {

    }

    public virtual void Click()
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
