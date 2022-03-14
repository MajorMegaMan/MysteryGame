using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}
