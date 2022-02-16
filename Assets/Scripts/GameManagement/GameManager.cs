using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Control Stuff")]
    [SerializeField] CameraController m_cameraControl = null;
    [SerializeField] GameMap m_gameMap = null;
    [SerializeField] PlayerHUD m_playerHUD = null;

    // managers
    UnitManager m_unitManager;
    TurnManager m_turnManager;
    [SerializeField] UnitManager.UnitManagerPackage m_unitManagerPackage = null;

    // Unit references
    Unit m_player = null;

    [Header("Debug Stuff")]
    [SerializeField] int m_debugBotCount = 2;
    Unit[] m_debugAIUnits = null;

    // getters
    public TurnManager turnManager { get { return m_turnManager; } }
    public UnitManager unitManager { get { return m_unitManager; } }

    public enum UnitControllerEnum
    {
        player,
        debugAI
    }

    private void Awake()
    {
        // Initialise unitManager
        m_unitManager = new UnitManager(this, m_unitManagerPackage);
        m_turnManager = new TurnManager();
        CreateControllers();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Spawn units
        Tile playerTile = m_gameMap.GetTile(m_gameMap.startRoom.GetCentre());
        m_player = SpawnUnit(UnitProfileKey.debugPlayer, UnitControllerEnum.player, playerTile);

        m_playerHUD.InitialiseWithPlayerUnit(m_player);

        m_debugAIUnits = new Unit[m_debugBotCount];
        for (int i = 0; i < m_debugBotCount; i++)
        {
            int randX = Random.Range(m_gameMap.startRoom.startX, m_gameMap.startRoom.endX);
            int randY = Random.Range(m_gameMap.startRoom.startY, m_gameMap.startRoom.endY);
            Tile botTile = m_gameMap.GetTile(randX, randY);
            while (botTile.GetCurrentUnit() != null)
            {
                randX = Random.Range(m_gameMap.startRoom.startX, m_gameMap.startRoom.endX);
                randY = Random.Range(m_gameMap.startRoom.startY, m_gameMap.startRoom.endY);
                botTile = m_gameMap.GetTile(randX, randY);
            }

            m_debugAIUnits[i] = SpawnUnit(UnitProfileKey.debugBot, UnitControllerEnum.debugAI, botTile);
        }

        m_cameraControl.SetFollowTarget(m_player);

        // Setup Turn Manager
        foreach (Unit unit in m_unitManager.activeUnits)
        {
            m_turnManager.AddUnit(unit);
        }
        m_turnManager.SetPlayer(m_player);

        m_turnManager.FindTurnOrder();

        m_cameraControl.AddRenderEvent(UpdateHealthBarPositions);
    }

    // Update is called once per frame
    void Update()
    {
        m_turnManager.Update();
    }

    public void SetInitialTile(Unit unit, Tile tile)
    {
        unit.SafeEnterTile(tile);
        unit.transform.position = tile.position;
    }

    void CreateControllers()
    {
        // create in same order as UnitControllerEnum
        UnitController.CreateController<PlayerController>();
        UnitController.CreateController<DebugAIController>();
    }

    UnitController GetUnitController(UnitControllerEnum controllerEnum)
    {
        return UnitController.GetUnitController((int)controllerEnum);
    }

    public Unit SpawnUnit(UnitProfileKey unitProfileKey, UnitControllerEnum controllerEnum, Tile tile)
    {
        Unit unit = m_unitManager.SpawnUnit(unitProfileKey);
        unit.SetUnitController(GetUnitController(controllerEnum));

        SetInitialTile(unit, tile);

        return unit;
    }

    void UpdateHealthBarPositions()
    {
        foreach (Unit unit in m_debugAIUnits)
        {
            unit.SetHealthBarPos(m_cameraControl.cam);
        }
    }
}
