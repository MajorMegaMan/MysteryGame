using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    Tile m_currentTile = null;

    Vector3 m_targetPosition = Vector3.zero;
    [SerializeField] float m_moveSpeed = 5.0f;
    [SerializeField] Tile.Access m_tileAccess = 0;

    Tile.NeighbourDirection m_currentLookDirection = 0;

    delegate void VoidFunc();
    VoidFunc m_moveAction = () => { };

    bool m_isMoving = false;

    // getters
    public bool isMoving { get { return m_isMoving; } }
    public Tile.Access tileAccess { get { return m_tileAccess; } }
    Tile.NeighbourDirection currentLookDirection { get { return m_currentLookDirection; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        m_moveAction.Invoke();
    }

    void MoveToTargetPos()
    {
        Vector3 toTarget = m_targetPosition - transform.position;
        float distance = toTarget.magnitude;
        float deltaSpeed = Time.deltaTime * m_moveSpeed;
        if (distance < deltaSpeed)
        {
            transform.position = m_targetPosition;
            m_moveAction = () => { };
            m_isMoving = false;
        }
        else
        {
            transform.position += (toTarget / distance) * deltaSpeed;
        }
    }

    void SetPosition(Vector3 position)
    {
        m_targetPosition = position;
        m_moveAction = MoveToTargetPos;
        m_isMoving = true;
    }

    public void SetCurrentTile(Tile target)
    {
        m_currentTile = target;
        SetPosition(target.position);
    }

    public Tile GetCurrentTile()
    {
        return m_currentTile;
    }

    public void EnterTile(Tile target)
    {
        m_currentTile.SetCurrentUnit(null);
        SetCurrentTile(target);
        m_currentTile.SetCurrentUnit(this);
    }

    // Force moving into a tile without checking if the unit is able to
    public void ForceMoveToNeighbourTile(Tile.NeighbourDirection direction)
    {
        Tile neighbour = m_currentTile.GetNeighbour(direction);
        if(neighbour != null)
        {
            // For now, dodgy way to get the unit to face the direction it's moving
            Vector3 toTarget = neighbour.position - m_currentTile.position;
            transform.forward = toTarget;

            EnterTile(neighbour);
        }
    }

    // Safely move to neighbour tile with a check if the unit is able before moving.
    public void MoveToNeighbourTile(Tile.NeighbourDirection direction)
    {
        if(CheckNeighbourTileMove(direction, out Tile neighbourTile))
        {
            EnterTile(neighbourTile);
        }
    }

    // returns true if the unit is able to move into the neighbouring tile
    public bool CheckNeighbourTileMove(Tile.NeighbourDirection direction, out Tile neighbourTile)
    {
        neighbourTile = m_currentTile.GetNeighbour(direction);
        if(neighbourTile == null)
        {
            return false;
        }

        if (m_tileAccess < neighbourTile.access || neighbourTile.GetCurrentUnit() != null)
        {
            return false;
        }

        // Check if trying to move diagonally
        if (direction > Tile.NeighbourDirection.down)
        {
            if (CheckDiagonalAccess(m_currentTile, direction))
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
    bool CheckDiagonalAccess(Tile currentTile, Tile.NeighbourDirection direction)
    {
        if (m_tileAccess > Tile.Access.partial)
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
    bool CheckPartialAccess(Tile currentTile, Tile.NeighbourDirection vertDirection, Tile.NeighbourDirection horiDirection)
    {
        Tile vertNeighbour = currentTile.GetNeighbour(vertDirection);
        Tile horiNeighbour = currentTile.GetNeighbour(horiDirection);

        return vertNeighbour.access <= Tile.Access.partial && horiNeighbour.access <= Tile.Access.partial;
    }

    public void LookTowards(Tile.NeighbourDirection direction)
    {
        m_currentLookDirection = direction;

        // For now, dodgy way to get the unit to face the direction it's moving
        Vector3 assumedNeighbourOffset = m_currentTile.GetNeighbourOffset(direction);
        transform.forward = assumedNeighbourOffset;
    }
}
