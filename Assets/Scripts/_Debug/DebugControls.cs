﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugControls : MonoBehaviour
{
    [SerializeField] GameManager m_gameManager = null;
    [SerializeField] GameObject m_inventoryPanel = null;

    // Start is called before the first frame update
    void Start()
    {
        ToggleInventoryPanel(false);
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

        if(Input.GetKeyDown(KeyCode.I))
        {
            // Show inventory
            ToggleInventoryPanel(!m_inventoryPanel.activeSelf);
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
            var itemToken = m_gameManager.itemManager.GetItemToken(TempItemID.healthPotion);

            int breakCount = 0;
            var tile = m_gameManager.gameMap.startRoom.GetRandomTile() as GameMapTile;
            while(tile.GetToken(TempTokenID.item) != null)
            {
                tile = m_gameManager.gameMap.startRoom.GetRandomTile() as GameMapTile;
                breakCount++;
                if(breakCount > 10000)
                {
                    return;
                }
            }
            TokenManager.SetTokenToTile(itemToken, tile);
            itemToken.SetPositionToTile();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            var itemToken = m_gameManager.itemManager.GetItemToken(TempItemID.apple);

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
            TokenManager.SetTokenToTile(itemToken, tile);
            itemToken.SetPositionToTile();
        }
    }

    void ToggleInventoryPanel(bool shouldShow)
    {
        m_inventoryPanel.SetActive(shouldShow);
    }
}
