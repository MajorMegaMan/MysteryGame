using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid
{
    TileGridSize m_gridSize = null;

    Tile[,] m_tiles = null;

    Vector3 m_origin = Vector3.zero;

    // getters
    public int width { get { return m_gridSize.width; } }
    public int height { get { return m_gridSize.height; } }
    public float tileSize { get { return m_gridSize.tileSize; } }

    public float worldWidth { get { return m_gridSize.width * tileSize; } }
    public float worldHeight { get { return m_gridSize.height * tileSize; } }

    public Vector3 origin { get { return m_origin; } }

    public TileGrid(TileGridSize gridSize)
    {
        Init(gridSize);
    }

    public TileGrid(TileGridSize gridSize, Vector3 origin)
    {
        Init(gridSize);
        m_origin = origin;
    }

    void Init(TileGridSize gridSize)
    {
        m_gridSize = gridSize;

        m_tiles = new Tile[gridSize.width, gridSize.height];
    }

    float FindHalf(float size)
    {
        return (size * tileSize) / 2.0f;
    }

    public Vector3 FindHalfSize()
    {
        Vector3 result = Vector3.zero;
        result.x = FindHalf(width);
        result.z = FindHalf(height);
        return result;
    }

    public void InitialiseTiles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                InitialiseTile(x, y);
            }
        }

        SetNeighbours();
    }

    public Tile InitialiseTile(int x, int y)
    {
        m_tiles[x, y] = new Tile(x, y, this);
        return m_tiles[x, y];
    }

    public Tile GetTile(int x, int y)
    {
        return m_tiles[x, y];
    }

    // Maybe this'll work
    public Tile GetTile(Vector3 worldPos)
    {
        Vector3 halfSize = FindHalfSize();
        Vector3 toPos = worldPos + halfSize;
        toPos /= tileSize;

        return GetTile((int)toPos.x, (int)toPos.z);
    }

    void SetNeighbours()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = GetTile(x, y);

                if (y + 1 < height)
                {
                    tile.ConnectToTile(GetTile(x, y + 1), Tile.NeighbourDirection.up);

                    if (x + 1 < width)
                    {
                        tile.ConnectToTile(GetTile(x + 1, y + 1), Tile.NeighbourDirection.upRight);
                    }
                    if (x - 1 >= 0)
                    {
                        tile.ConnectToTile(GetTile(x - 1, y + 1), Tile.NeighbourDirection.upLeft);
                    }
                }

                if (y - 1 >= 0)
                {
                    tile.ConnectToTile(GetTile(x, y - 1), Tile.NeighbourDirection.down);

                    if (x + 1 < width)
                    {
                        tile.ConnectToTile(GetTile(x + 1, y - 1), Tile.NeighbourDirection.downRight);
                    }
                    if (x - 1 >= 0)
                    {
                        tile.ConnectToTile(GetTile(x - 1, y - 1), Tile.NeighbourDirection.downLeft);
                    }
                }

                if (x + 1 < width)
                {
                    tile.ConnectToTile(GetTile(x + 1, y), Tile.NeighbourDirection.right);
                }
                if (x - 1 >= 0)
                {
                    tile.ConnectToTile(GetTile(x - 1, y), Tile.NeighbourDirection.left);
                }
            }
        }
    }
}
