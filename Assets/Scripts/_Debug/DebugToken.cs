using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugToken : GameMapToken
{
    [SerializeField] GameMap m_gameMap = null;

    // Start is called before the first frame update
    void Start()
    {
        GameMapTile targetTile = m_gameMap.startRoom.GetRandomTile() as GameMapTile;
        while(targetTile.GetToken(TempTokenID.unit) != null)
        {
            targetTile = m_gameMap.startRoom.GetRandomTile() as GameMapTile;
        }
        MovingTokenManager.SetTokenToTile(this, targetTile);
        SetPositionToTile();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMoving)
        {
            if(Input.GetKey(KeyCode.J))
            {
                MoveToNextTile(currentTile.GetNeighbour<GameMapTile>(Tile.NeighbourDirection.left));
            }
            else if (Input.GetKey(KeyCode.L))
            {
                MoveToNextTile(currentTile.GetNeighbour<GameMapTile>(Tile.NeighbourDirection.right));
            }
        }
    }

    public override int GetTokenID()
    {
        return (int)TempTokenID.item;
    }
}
