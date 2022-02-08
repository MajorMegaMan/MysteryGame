using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newGameMapProfile", menuName = "Game Map Profile")]
public class GameMapProfile : ScriptableObject
{
    [SerializeField] int m_seed = 0;

    [SerializeField] TileGridSize m_gridSize = null;
    [SerializeField] TileTextureSet m_tileTextureSet = null;

    [SerializeField] RoomProfile[] m_roomProfiles = null;

    [SerializeField] bool m_automaticGenerateCount = true;
    [SerializeField] int m_roomGenerateCount = 50;

    [SerializeField] int m_maxPathDistance = 50;

    // getters
    public int seed { get { return m_seed; } }

    public TileGridSize gridSize { get { return m_gridSize; } }
    public int width { get { return m_gridSize.width; } }
    public int height { get { return m_gridSize.height; } }
    public float tileSize { get { return m_gridSize.tileSize; } }

    public float worldWidth { get { return m_gridSize.width * tileSize; } }
    public float worldHeight { get { return m_gridSize.height * tileSize; } }

    public TileTextureSet tileTextureSet { get { return m_tileTextureSet; } }

    public int roomGenerateCount { get { return m_roomGenerateCount; } }

    public int maxPathDistance { get { return m_maxPathDistance; } }

    public RoomProfile GetRandomRoomProfile()
    {
        int profileIndex = Random.Range(0, m_roomProfiles.Length);
        return m_roomProfiles[profileIndex];
    }

    public void SetFloorColliderSize(BoxCollider floorCollider)
    {
        Vector3 boxSize = floorCollider.size;
        boxSize.x = worldWidth;
        boxSize.y = 0.05f;
        boxSize.z = worldHeight;
        floorCollider.size = boxSize;

        Vector3 boxPos = floorCollider.center;
        boxPos.y = -boxSize.y / 2.0f;
        floorCollider.center = boxPos;
    }

    public int GetFloorSeed(int floor)
    {
        Random.InitState(m_seed);
        int floorSeed = m_seed;
        for(int i = 0; i < floor; i++)
        {
            floorSeed = Random.Range(0, int.MaxValue);
        }
        return floorSeed;
    }

    private void OnValidate()
    {
        if(m_automaticGenerateCount)
        {
            m_roomGenerateCount = width * height / 10;
            if(m_roomGenerateCount < 1)
            {
                m_roomGenerateCount = 1;
            }
        }
    }
}
