using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    [SerializeField] Camera m_camera = null;

    [SerializeField] GameMap m_gameMap = null;

    [SerializeField] LayerMask m_tileLayer = 0;

    Tile m_hoverTile = null;

    public delegate void TileAction(Tile target);
    TileAction m_rayHitAction = null;

    public TileAction mouseEnterAction;
    public TileAction mouseExitAction;
    public TileAction mouseClickAction;

    private void Awake()
    {
        m_rayHitAction = HoverCheckNull;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CamRay(out RaycastHit hitInfo))
        {
            m_rayHitAction.Invoke(m_gameMap.GetTile(hitInfo.point));
        }
        else
        {
            m_rayHitAction.Invoke(null);
        }

        if(Input.GetMouseButtonDown(0))
        {
            if(m_hoverTile != null)
            {
                m_hoverTile.Click();
                mouseClickAction?.Invoke(m_hoverTile);
            }
        }
    }

    void HoverCheckNull(Tile tile)
    {
        if(tile != null)
        {
            HoverEnter(tile);
        }
    }

    void HoverEnter(Tile tile)
    {
        m_hoverTile = tile;

        tile.MouseEnter();
        mouseEnterAction?.Invoke(tile);

        m_rayHitAction = HoverCheckExit;
    }

    void HoverCheckExit(Tile tile)
    {
        if(tile != m_hoverTile)
        {
            HoverExit(tile);
        }
    }

    void HoverExit(Tile tile)
    {
        m_hoverTile.MouseExit();
        mouseExitAction?.Invoke(tile);
        if (tile != null)
        {
            HoverEnter(tile);
        }
        else
        {
            m_hoverTile = tile;
            m_rayHitAction = HoverCheckNull;
        }
    }

    bool CamRay(out RaycastHit hitInfo)
    {
        Ray camRay = m_camera.ScreenPointToRay(Input.mousePosition);
        return RayToFloor(camRay.origin, camRay.direction, out hitInfo);
    }

    bool RayToFloor(Vector3 origin, Vector3 dir, out RaycastHit hitInfo)
    {
        return Physics.Raycast(origin, dir, out hitInfo, float.PositiveInfinity, m_tileLayer);
    }

    private void OnDrawGizmos()
    {
        Camera cam = Camera.main;
        Ray camRay = cam.ScreenPointToRay(Input.mousePosition);
        if (RayToFloor(camRay.origin, camRay.direction, out RaycastHit hitInfo))
        {
            Gizmos.DrawLine(camRay.origin, hitInfo.point);
        }
        else
        {
            Gizmos.DrawLine(camRay.origin, camRay.direction * 100.0f);
        }
    }
}
