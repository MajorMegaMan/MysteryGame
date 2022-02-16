using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTurnReader : MonoBehaviour
{
    public GameManager gameManager = null;

    public Unit currentUnit = null;
    public Unit[] turnOrder = null;

    public UnitActionLogInfo[] logs = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentUnit = gameManager.turnManager.currentUnitsTurn;
        turnOrder = gameManager.turnManager.turnOrder;
        logs = gameManager.unitActionLog.actionLog.ToArray();
    }
}
