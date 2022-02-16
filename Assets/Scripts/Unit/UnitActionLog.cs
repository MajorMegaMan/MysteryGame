using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionLog
{
    List<UnitActionLogInfo> m_actionLog = new List<UnitActionLogInfo>();

    public List<UnitActionLogInfo> actionLog {  get { return m_actionLog; } }
}

[System.Serializable]
public struct UnitActionLogInfo
{
    public string actionName;
    public string unitName;
    public Tile.NeighbourDirection direction;
}
