using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameMapToken : MonoBehaviour, IGameMapTileSetter
{
    GameMapTile m_currentTile = null;

    public float tokenMoveSpeedScale = 1.0f;

    float m_currentMoveProgress = 0.0f;
    TokenMover m_tokenMover = new TokenMover();
    bool m_isMoving = false;

    Queue<GameMapTile> m_currentPath = new Queue<GameMapTile>();

    public GameMapTile currentTile { get { return m_currentTile; } }
    public bool isMoving { get { return m_isMoving; } }

    #region IObjectRefMethods
    void IGameMapTileSetter.SetTile(GameMapTile target)
    {
        m_currentTile = target;
    }

    GameMapTile IGameMapTileSetter.GetTile()
    {
        return m_currentTile;
    }

    IGameMapTokenSetter IGameMapTileSetter.GetITile()
    {
        return m_currentTile;
    }

    public abstract int GetTokenID();
    #endregion

    public void SetPositionToTile()
    {
        transform.position = m_currentTile.position;
    }

    public Queue<GameMapTile> FindPathToTile(GameMapTile targetTile)
    {
        return null;
    }

    // Moves to a target tile but finds a path first
    public void MoveToTile(GameMapTile targetTile)
    {
        // Get a path to tile
        Queue<GameMapTile> path = FindPathToTile(targetTile);

        MoveToTile(path);
    }

    public void MoveToTile(Queue<GameMapTile> path)
    {
        // for each tile in path move to next tile
        m_currentPath = path;

        MoveToNextTile(m_currentPath.Dequeue());
    }

    // Moves to target tile directly without following a queue of tiles to get there
    public void MoveToNextTile(GameMapTile targetTile)
    {
        // Set Token Mover positions
        m_tokenMover.SetTiles(currentTile, targetTile);
        // Enable Token mover
        MovingTokenManager.instance.AddMovingToken(this);
        // Reset current move progress
        m_currentMoveProgress = 0.0f;
        m_isMoving = true;
        OnMoveBegin(m_tokenMover);

        // Update currentTile and token
        MovingTokenManager.SetTokenToTile(this, m_tokenMover.endTile);
    }

    // This is called by the MoveableTokenManager automatically. The user does not need to call this method.
    public void UpdateTransformMovement(float speed, float deltaTime)
    {
        // Read evaluation in update step and apply to this Transform
        m_currentMoveProgress += speed * deltaTime * tokenMoveSpeedScale;
        if (m_currentMoveProgress < 1.0f)
        {
            transform.position = m_tokenMover.Evaluate(m_currentMoveProgress);
        }
        else
        {
            // Reached destination
            if (m_currentPath.Count != 0)
            {
                MoveToNextTile(m_currentPath.Dequeue());
            }
            else
            {
                MovingTokenManager.instance.RemoveMovingToken(this);
            }
        }
    }

    void IGameMapTileSetter.ResolveTokenMove()
    {
        SetPositionToTile();
        m_isMoving = false;
        OnMoveArrive();
    }

    #region UserOverrides
    public virtual void OnMoveBegin(TokenMover tokenMover)
    {

    }

    public virtual void OnMoveArrive()
    {

    }

    public virtual void OnEnterTile(GameMapTile gameMapTile)
    {

    }

    public virtual void OnExitTile(GameMapTile gameMapTile)
    {

    }
    #endregion
}
