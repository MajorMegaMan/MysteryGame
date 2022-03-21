using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    List<GameMapTile> m_tiles;
    int m_startX = 0;
    int m_startY = 0;
    int m_endX = 0;
    int m_endY = 0;

    // Getters
    public int startX { get { return m_startX; } }
    public int startY { get { return m_startY; } }
    public int endX { get { return m_endX; } }
    public int endY { get { return m_endY; } }


    static IRoomSort _roomSortComparer = new IRoomSort();
    public static IRoomSort roomComparer { get { return _roomSortComparer; } }

    public Room(int startX, int startY, int endX, int endY)
    {
        m_tiles = new List<GameMapTile>();
        m_startX = startX;
        m_startY = startY;
        m_endX = endX;
        m_endY = endY;
    }

    public void AddTileToRoom(GameMapTile tile)
    {
        m_tiles.Add(tile);
        tile.SetRoom(this);
    }

    public Vector3 GetCentre()
    {
        if(m_tiles.Count == 0)
        {
            return Vector3.zero;
        }
        Vector3 result = Vector3.zero;
        for(int i = 0; i < m_tiles.Count; i++)
        {
            result += m_tiles[i].position;
        }
        return result / m_tiles.Count;
    }

    public Tile GetRandomTile()
    {
        return m_tiles[Random.Range(0, m_tiles.Count)];
    }
}

public class IRoomSort : IComparer<Room>
{
    int IComparer<Room>.Compare(Room lhs, Room rhs)
    {
        Vector3 lhsCentre = lhs.GetCentre();
        Vector3 rhsCentre = rhs.GetCentre();

        if (lhsCentre.x < rhsCentre.x)
        {
            return -1;
        }
        else if (lhsCentre.x > rhsCentre.x)
        {
            return 1;
        }
        else
        {
            if (lhsCentre.y < rhsCentre.y)
            {
                return -1;
            }
            else if (lhsCentre.y > rhsCentre.y)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}

struct RoomConnection
{
    public Room start;
    public Room end;

    public int manhattanDistance;

    public RoomConnection(Room start, Room end, int manhattanDistance)
    {
        this.start = start;
        this.end = end;

        this.manhattanDistance = manhattanDistance;
    }

    public bool Compare(RoomConnection otherConnection)
    {
        bool exactlyEqual = start == otherConnection.start && end == otherConnection.end;
        bool isReverse = start == otherConnection.end && end == otherConnection.start;
        return exactlyEqual || isReverse;
    }

    public static bool ListContains(List<RoomConnection> roomConnections, RoomConnection target)
    {
        for(int i = 0; i < roomConnections.Count; i++)
        {
            if(roomConnections[i].Compare(target))
            {
                return true;
            }
        }
        return false;
    }
}
