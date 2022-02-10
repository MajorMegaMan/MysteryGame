using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(BoxCollider))]
public class GameMap : MonoBehaviour
{
    // Grid Var
    [SerializeField] GameMapProfile m_mapProfile = null;
    [SerializeField] int m_floor = 0;

    TileFloorMesh m_tileFloor = null;
    TileGrid m_grid = null;

    BoxCollider m_floorCollider = null;

    // Rooms var
    List<Room> m_rooms;

    // getters
    public int width { get { return m_mapProfile.width; } }
    public int height { get { return m_mapProfile.height; } }
    public float tileSize { get { return m_mapProfile.tileSize; } }

    public float worldWidth { get { return m_mapProfile.width * tileSize; } }
    public float worldHeight { get { return m_mapProfile.height * tileSize; } }


    public Room startRoom { get { return m_rooms[0]; } }

    public GameMapProfile mapProfile { get { return m_mapProfile; } }

    private void Awake()
    {
        m_floorCollider = GetComponent<BoxCollider>();
        m_mapProfile.SetFloorColliderSize(m_floorCollider);

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = m_mapProfile.tileTextureSet.CreateMaterial();

        Vector3 floorOrigin = new Vector3(-worldWidth / 2.0f, 0.0f, -worldHeight / 2.0f) + transform.position;

        Random.InitState(m_mapProfile.GetFloorSeed(m_floor));

        // Create the tile Grid container that holds tiles
        InitialiseTileGrid(floorOrigin);

        // Create the mesh that visually shows the tiles
        InitialiseTileFloor(floorOrigin);

        // Assign inital tiles a profile
        AssignInitialTileProfiles();

        // Change the profiles of various tiles to create rooms
        m_rooms = new List<Room>();
        for (int i = 0; i < m_mapProfile.roomGenerateCount; i++)
        {
            FindRandomRoom();
        }
        List<Vector2> roomTriangulationPoints = new List<Vector2>(m_rooms.Count);
        for (int i = 0; i < m_rooms.Count; i++)
        {
            Vector3 roomCentre = m_rooms[i].GetCentre();
            Vector2 triangulationPoint = Vector2.zero;
            triangulationPoint.x = roomCentre.x;
            triangulationPoint.y = roomCentre.z;
            roomTriangulationPoints.Add(triangulationPoint);
        }
        // Triangulate Rooms
        DelaunayMap delMap = new DelaunayMap();
        delMap.Initialise(roomTriangulationPoints.ToArray());

        List<RoomConnection> roomConnections = new List<RoomConnection>();

        for(int i = 0; i < delMap.GetTrianglesSize(); i++)
        {
            DelaunayTriangle tri = delMap.GetTriangle(i);
            for (int verticeIndex = 0; verticeIndex < 3; verticeIndex++)
            {
                Vector3 startPoint = tri.GetVector3Point(verticeIndex);
                Vector3 endPoint = tri.GetVector3Point((verticeIndex + 1) % 3);

                Tile startTile = GetTile(startPoint);
                Tile endTile = GetTile(endPoint);

                int xDiff = Mathf.Abs(startTile.x - endTile.x);
                int yDiff = Mathf.Abs(startTile.y - endTile.y);

                RoomConnection connection = new RoomConnection(startTile.GetRoom(), endTile.GetRoom(), xDiff + yDiff);

                if(!RoomConnection.ListContains(roomConnections, connection))
                {
                    roomConnections.Add(connection);
                }
            }
        }

        for (int i = 0; i < roomConnections.Count; i++)
        {
            if(roomConnections[i].manhattanDistance < m_mapProfile.maxPathDistance)
            {
                Tile startTile = GetTile(roomConnections[i].start.GetCentre());
                Tile endTile = GetTile(roomConnections[i].end.GetCentre());
                CreatePath(startTile, endTile);
            }
        }

        // After the tiles have had their profiles set. The mesh is ready to finally be updated
        m_tileFloor.UpdateUVs();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitialiseTileFloor(Vector3 origin)
    {
        m_tileFloor = new TileFloorMesh(m_mapProfile.gridSize);
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = m_tileFloor.CreateTileMesh(origin);
    }

    void InitialiseTileGrid(Vector3 origin)
    {
        m_grid = new TileGrid(m_mapProfile.gridSize, origin);
        m_grid.InitialiseTiles();
    }

    void AssignInitialTileProfiles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = m_grid.GetTile(x, y);
                tile.SetFloorMesh(m_tileFloor);
                tile.InitialiseProfile(m_mapProfile.tileTextureSet.initialTileProfile);
            }
        }
    }

    public Tile GetTile(int x, int y)
    {
        return m_grid.GetTile(x, y);
    }

    public Tile GetTile(Vector3 worldPos)
    {
        return m_grid.GetTile(worldPos);
    }

    void FindRandomRoom()
    {
        RoomProfile profile = GetRandomRoomProfile();

        int roomWidth = profile.GetRandomWidth();
        int roomHeight = profile.GetRandomHeight();

        int startX = Random.Range(0, width - roomWidth);
        int startY = Random.Range(0, height - roomHeight);

        if(startX < 1 || startY < 1)
        {
            return;
        }

        if (isOverlapping(startX, startY, startX + roomWidth, startY + roomHeight))
        {
            return;
        }

        GenerateRoom(startX, startY, startX + roomWidth, startY + roomHeight);
    }

    bool isOverlapping(int startX, int startY, int endX, int endY)
    {
        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                Tile preCheck = m_grid.GetTile(x, y);
                if (preCheck.access != m_mapProfile.tileTextureSet.initialTileProfile.access)
                {
                    return true;
                }
                for (int i = 0; i < 8; i++)
                {
                    Tile neighbour = preCheck.GetNeighbour((Tile.NeighbourDirection)i);
                    if (neighbour != null && neighbour.access != m_mapProfile.tileTextureSet.initialTileProfile.access)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    RoomProfile GetRandomRoomProfile()
    {
        return m_mapProfile.GetRandomRoomProfile();
    }

    void GenerateRoom(int startX, int startY, int endX, int endY)
    {
        Room room = new Room(startX, startY, endX, endY);
        m_rooms.Add(room);

        startX = Mathf.Clamp(startX, 0, width);
        startY = Mathf.Clamp(startY, 0, height);

        endX = Mathf.Clamp(endX, 0, width);
        endY = Mathf.Clamp(endY, 0, height);

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                Tile tile = m_grid.GetTile(x, y);
                tile.InitialiseProfile(m_mapProfile.tileTextureSet.openTileProfile);
                room.AddTileToRoom(tile);
            }
        }
    }

    void CreatePath(Tile startTile, Tile endTile)
    {
        Tile currentTile = startTile;
        int xDiff = endTile.x - currentTile.x;
        int yDiff = endTile.y - currentTile.y;

        int debugCount = 0;

        while(FindTileManhattanDir(xDiff, yDiff, out Tile.NeighbourDirection toTile))
        {
            // Do something with the found Tile
            currentTile.InitialiseProfile(m_mapProfile.tileTextureSet.openTileProfile);

            currentTile = currentTile.GetNeighbour(toTile);
            xDiff = endTile.x - currentTile.x;
            yDiff = endTile.y - currentTile.y;

            if(debugCount > 20000)
            {
                // Infinite Loop Maybe
                Debug.Log("Infinite Loop Maybe");
                break;
            }
        }
        endTile.InitialiseProfile(m_mapProfile.tileTextureSet.openTileProfile);
    }

    // vertDir
    Tile.NeighbourDirection FindVertical(int yDiff)
    {
        if (yDiff > 0)
        {
            // go up
            return Tile.NeighbourDirection.up;
        }
        else
        {
            // go down
            return Tile.NeighbourDirection.down;
        }
    }

    // Returns false if this is the same tile
    bool FindTileManhattanDir(int xDiff, int yDiff, out Tile.NeighbourDirection toTile)
    {
        int absX = System.Math.Abs(xDiff);
        int absY = System.Math.Abs(yDiff);
        int distance = absX + absY;

        if(distance == 0)
        {
            // There is no distance which means the two tiles are the same
            toTile = 0;
            return false;
        }

        if (xDiff < 0)
        {
            if(absX > absY)
            {
                // go left
                toTile = Tile.NeighbourDirection.left;
            }
            else
            {
                toTile = FindVertical(yDiff);
            }
        }
        else if (xDiff > 0)
        {
            if (absX > absY)
            {
                // go right
                toTile = Tile.NeighbourDirection.right;
            }
            else
            {
                toTile = FindVertical(yDiff);
            }

        }
        else
        {
            // no horizontal
            toTile = FindVertical(yDiff);
        }
        return true;
    }

    private void OnValidate()
    {
        if(m_mapProfile != null)
        {
            BoxCollider floorCollider = GetComponent<BoxCollider>();
            m_mapProfile.SetFloorColliderSize(floorCollider);
        }
    }

    private void Reset()
    {
        if (m_mapProfile != null)
        {
            BoxCollider floorCollider = GetComponent<BoxCollider>();
            m_mapProfile.SetFloorColliderSize(floorCollider);
        }
    }
}