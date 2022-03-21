using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMapTile : Tile, IGameMapTokenSetter
{
    // Gameplay Data
    Room m_room = null;
    TileProfile m_tileProfile = null;

    List<GameMapToken> m_currentTokens;

    // Mesh Data
    TileFloorMesh m_floorMesh = null;

    // getters
    public TileProfile profile { get { return m_tileProfile; } }

    // Quicker get for tile access. Returns the access from this Tile's profile
    public Access access { get { return m_tileProfile.access; } }


    protected TileFloorMesh floorMesh { get { return m_floorMesh; } }

    public enum Access
    {
        open,
        partial,
        blocked
    }

    protected override void OnCreate()
    {
        System.Array tokenIDValues = System.Enum.GetValues(typeof(TempTokenID));
        m_currentTokens = new List<GameMapToken>(tokenIDValues.Length);
        for(int i = 0; i < tokenIDValues.Length; i++)
        {
            m_currentTokens.Add(null);
        }
    }

    void IGameMapTokenSetter.SetToken(GameMapToken gameMapToken)
    {
        m_currentTokens[gameMapToken.GetID()] = gameMapToken;
    }

    void IGameMapTokenSetter.SetToken(GameMapToken gameMapToken, int tokenID)
    {
        m_currentTokens[tokenID] = gameMapToken;
    }

    void IGameMapTokenSetter.ClearToken(int tokenID)
    {
        m_currentTokens[tokenID] = null;
    }

    GameMapToken IGameMapTokenSetter.GetToken(int tokenID)
    {
        return m_currentTokens[tokenID];
    }

    public GameMapToken GetToken(TempTokenID tokenID)
    {
        return m_currentTokens[(int)tokenID];
    }

    public void InitialiseFloorMesh(TileFloorMesh tileFloorMesh, TileProfile tileProfile)
    {
        SetFloorMesh(tileFloorMesh);
        InitialiseProfile(tileProfile);
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
        floorMesh.SetTileUVs(profile.UVArray, x, y);
    }

    public void SetProfile(TileProfile tileProfile)
    {
        m_tileProfile = tileProfile;
        UpdateColour();
        floorMesh.UpdateUVs();
    }

    public void SetRoom(Room room)
    {
        m_room = room;
    }

    public Room GetRoom()
    {
        return m_room;
    }
}
