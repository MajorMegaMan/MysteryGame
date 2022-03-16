using System.Collections;
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
    }

    void ToggleInventoryPanel(bool shouldShow)
    {
        m_inventoryPanel.SetActive(shouldShow);
    }
}
