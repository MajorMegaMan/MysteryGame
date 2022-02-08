using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Unit m_player = null;
    [SerializeField] GameMap m_gameMap = null;

    // Start is called before the first frame update
    void Start()
    {
        Tile tile = m_gameMap.GetTile(m_gameMap.startRoom.GetCentre());
        m_player.SetCurrentTile(tile);
        m_player.transform.position = tile.position;
        tile.SetCurrentUnit(m_player);
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_player.isMoving)
        {
            Tile.NeighbourDirection direction;
            if (FindInput(out direction))
            {
                m_player.LookTowards(direction);
                m_player.MoveToNeighbourTile(direction);
            }
        }
    }

    bool FindInput(out Tile.NeighbourDirection direction)
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal > 0)
        {
            if (vertical > 0)
            {
                direction = Tile.NeighbourDirection.upRight;
            }
            else if (vertical < 0)
            {
                direction = Tile.NeighbourDirection.downRight;
            }
            else
            {
                // No input
                direction = Tile.NeighbourDirection.right;
            }
        }
        else if (horizontal < 0)
        {
            if (vertical > 0)
            {
                direction = Tile.NeighbourDirection.upLeft;
            }
            else if (vertical < 0)
            {
                direction = Tile.NeighbourDirection.downLeft;
            }
            else
            {
                // No input
                direction = Tile.NeighbourDirection.left;
            }
        }
        else
        {
            if (vertical > 0)
            {
                direction = Tile.NeighbourDirection.up;
            }
            else if (vertical < 0)
            {
                direction = Tile.NeighbourDirection.down;
            }
            else
            {
                // No input
                direction = Tile.NeighbourDirection.up;
                return false;
            }
        }
        return true;
    }

    // returns true if player can cut corners
    bool CheckDiagonalAccess(Tile currentTile, Tile.NeighbourDirection direction)
    {
        if(m_player.tileAccess > Tile.Access.partial)
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
}
