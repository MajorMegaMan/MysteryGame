using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMouseclicks : MonoBehaviour
{
    [SerializeField] MouseController m_mouseControl = null;
    [SerializeField] GameMap m_gameMap = null;

    TileProfile[] tileProfiles;
    int m_currentIndex = 0;

    TileProfile currentProfile { get { return tileProfiles[m_currentIndex]; } }

    delegate void TileAction(GameMapTile tile);
    TileAction m_debugLeftClick = (GameMapTile tile) => { };

    delegate void VoidAction();
    VoidAction m_debugRightClick = () => { };

    [SerializeField] DebugAction m_currentDebugAction = 0;
    public enum DebugAction
    {
        noAction,
        settingTiles
    }

    // dumb little bool to get around editor calls, such as onValidate
    bool m_isAwake = false;

    // Start is called before the first frame update
    void Start()
    {
        tileProfiles = new TileProfile[m_gameMap.mapProfile.tileTextureSet.otherTileProfiles.Length + 2];
        tileProfiles[0] = m_gameMap.mapProfile.tileTextureSet.initialTileProfile;
        tileProfiles[1] = m_gameMap.mapProfile.tileTextureSet.openTileProfile;
        for(int i = 0; i < m_gameMap.mapProfile.tileTextureSet.otherTileProfiles.Length; i++)
        {
            tileProfiles[i + 2] = m_gameMap.mapProfile.tileTextureSet.otherTileProfiles[i];
        }

        m_mouseControl.mouseClickAction += MouseClick;
        SetDebugAction(m_currentDebugAction);
        m_isAwake = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            ToggleDebugAction(DebugAction.settingTiles);
        }
        if(Input.GetMouseButtonDown(1))
        {
            m_debugRightClick.Invoke();
        }
    }

    void MouseClick(Tile tile)
    {
        m_debugLeftClick.Invoke(tile as GameMapTile);
    }

    void ClearDebugAction()
    {
        m_debugLeftClick = PrintTile;
        m_debugRightClick = Empty;
    }

    void SetDebugAction(DebugAction debugAction)
    {
        m_currentDebugAction = debugAction;
        Debug.Log("Set Debug Action: " + debugAction.ToString());
        switch(debugAction)
        {
            case DebugAction.noAction:
            {
                    ClearDebugAction();
                    break;
            }
            case DebugAction.settingTiles:
                {
                    m_debugLeftClick = SetTileProfile;
                    m_debugRightClick = CycleProfileIndex;
                    break;
                }
            default:
                {
                    ClearDebugAction();
                    break;
                }
        }
    }

    void ToggleDebugAction(DebugAction debugAction)
    {
        if (m_currentDebugAction != debugAction)
        {
            SetDebugAction(debugAction);
        }
        else
        {
            ClearDebugAction();
        }
    }

    void PrintTile(GameMapTile tile)
    {
        string msg = "Tile: " + tile.profile.name;

        msg += "== Unit: ";
        GameMapToken unit = tile.GetToken(TempTokenID.unit);
        if(unit != null)
        {
            msg += unit.name;
        }
        else
        {
            msg += "null";
        }
        msg += "; ";
        Debug.Log(msg);
    }

    void Empty()
    {
        
    }

    void SetTileProfile(GameMapTile tile)
    {
        Debug.Log(tile.profile.name);
        if (tile.profile != currentProfile)
        {
            tile.SetProfile(currentProfile);
        }
    }

    void CycleProfileIndex()
    {
        m_currentIndex = (m_currentIndex + 1) % tileProfiles.Length;
    }

    private void OnValidate()
    {
        if(m_isAwake)
        {
            SetDebugAction(m_currentDebugAction);
        }
    }
}
