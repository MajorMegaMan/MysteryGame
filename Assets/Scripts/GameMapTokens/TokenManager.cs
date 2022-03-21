using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenManager : MonoSingletonBase<TokenManager>
{
    List<GameMapToken> m_movingTokens = new List<GameMapToken>();
    float m_tokenSpeed = 5.0f;
    float m_tokenTimeScale = 1.0f;

    // This is a Queue of tokens that are ready to be removed from the active moving List
    Queue<GameMapToken> m_cleanupTokens = new Queue<GameMapToken>();

    private void Update()
    {
        // Clean up any ready to move tokens that have finished arriving at their destination
        while (m_cleanupTokens.Count > 0)
        {
            m_movingTokens.Remove(m_cleanupTokens.Dequeue());
        }

        // Move tokens that are moving towards their target tile.
        foreach (var movingToken in m_movingTokens)
        {
            movingToken.UpdateTransformMovement(m_tokenSpeed, Time.deltaTime * m_tokenTimeScale);
        }
    }

    private void LateUpdate()
    {

    }

    public void AddMovingToken(GameMapToken gameMapToken)
    {
        m_movingTokens.Add(gameMapToken);
    }

    public void RemoveMovingToken(GameMapToken gameMapToken)
    {
        m_cleanupTokens.Enqueue(gameMapToken);
    }

    // Token controls
    // This function will initialise the token to the gameMap.
    public static void SetTokenToTile(GameMapToken gameMapToken, GameMapTile gameMapTile)
    {
        // Get Interfaces
        IGameMapTileSetter tokenInterface = gameMapToken;
        IGameMapTokenSetter tileInterface = gameMapTile;

        // first cleanup old tiles
        var currentTile = tokenInterface.GetITile();
        if (currentTile != null)
        {
            currentTile.ClearToken(tokenInterface.GetID());
            gameMapToken.OnExitTile(gameMapTile);
        }

        // Set Current Token and Current tile to the targets
        tokenInterface.SetTile(gameMapTile);
        tileInterface.SetToken(gameMapToken);
        gameMapToken.OnEnterTile(gameMapTile);
    }

    // Removes a Token from the gamemap.
    // The target token loses it's reference to any tile and it's current tile loses it's reference to any token in the same tokenID slot.
    public static void ClearToken(GameMapToken gameMapToken)
    {
        ClearIToken(gameMapToken);
    }

    static void ClearIToken(IGameMapTileSetter gameMapToken)
    {
        // first cleanup old tiles
        var currentTile = gameMapToken.GetITile();
        if (currentTile != null)
        {
            currentTile.ClearToken(gameMapToken.GetID());
        }

        // Set Current Tile to null
        gameMapToken.SetTile(null);
    }
}
