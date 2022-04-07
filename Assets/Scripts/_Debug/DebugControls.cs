using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugControls : MonoBehaviour
{
    [SerializeField] GameManager m_gameManager = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            var healthStat = m_gameManager.playerUnit.unitStats.GetStat(MysterySystems.UnitStats.ResourceStatKey.health);
            var hungerStat = m_gameManager.playerUnit.unitStats.GetStat(MysterySystems.UnitStats.ResourceStatKey.hunger);
            healthStat.Reset();
            hungerStat.Reset();
            m_gameManager.playerUnit.InvokeStatChangeEvent();
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_gameManager.itemManager.AddItemToInventory(m_gameManager.playerUnit.inventory, TempItemID.apple);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_gameManager.itemManager.AddItemToInventory(m_gameManager.playerUnit.inventory, TempItemID.healthPotion);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            SpawnItemToken(TempItemID.healthPotion);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            SpawnItemToken(TempItemID.apple);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            SpawnItemToken(TempItemID.sword);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            SpawnItemToken(TempItemID.sheild);
        }
    }

    void SpawnItemToken(TempItemID itemID)
    {
        var itemToken = m_gameManager.itemManager.GetItemToken(itemID);
        if (itemToken == null)
        {
            // failed to find a pooled item token
            Debug.LogWarning("Failed to find a pooled Item Token. Consider exapnding the size of the pool.");
            return;
        }

        int breakCount = 0;
        var tile = m_gameManager.gameMap.startRoom.GetRandomTile() as GameMapTile;
        while (tile.GetToken(TempTokenID.item) != null)
        {
            tile = m_gameManager.gameMap.startRoom.GetRandomTile() as GameMapTile;
            breakCount++;
            if (breakCount > 10000)
            {
                return;
            }
        }
        MovingTokenManager.SetTokenToTile(itemToken, tile);
        itemToken.SetPositionToTile();
    }
}
