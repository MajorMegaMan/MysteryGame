using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] CameraController m_cameraControl = null;

    [SerializeField] GameMap m_gameMap = null;

    UnitManager m_unitManager;
    UnitProfile[] m_unitProfiles = null;

    [SerializeField] Unit m_unitPrefab = null;
    [SerializeField] UnitProfile m_playerUnitProfile = null;

    [SerializeField] int m_debugBotCount = 2;
    [SerializeField] UnitProfile m_debugAIUnitProfile = null;

    Unit m_player = null;
    
    TurnManager m_turnManager = new TurnManager();

    Unit[] m_debugAIUnits = null;

    public TurnManager turnManager { get { return m_turnManager; } }

    private void Awake()
    {
        m_unitProfiles = new UnitProfile[2];
        m_unitProfiles[0] = m_playerUnitProfile;
        m_unitProfiles[1] = m_debugAIUnitProfile;
        m_unitManager = new UnitManager(this, m_unitPrefab, 5, m_unitProfiles, 3, null, null);
        m_player = m_unitManager.SpawnUnit(m_playerUnitProfile);

        m_cameraControl.SetFollowTarget(m_player);

        m_debugAIUnits = new Unit[m_debugBotCount];
        for (int i = 0; i < m_debugBotCount; i++)
        {
            m_debugAIUnits[i] = m_unitManager.SpawnUnit(m_debugAIUnitProfile);
        }

        m_turnManager.Initialise(m_player);
        CreateControllers();

        m_player.SetUnitController(GetUnitController(UnitControllerEnum.player));

        foreach(Unit botUnit in m_debugAIUnits)
        {
            botUnit.SetUnitController(GetUnitController(UnitControllerEnum.debugAI));
            m_turnManager.AddUnit(botUnit);
        }

        m_turnManager.FindTurnOrder();
    }

    // Start is called before the first frame update
    void Start()
    {
        Tile playerTile = m_gameMap.GetTile(m_gameMap.startRoom.GetCentre());
        SetInitialTile(m_player, playerTile);


        foreach (Unit botUnit in m_debugAIUnits)
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
            SetInitialTile(botUnit, botTile);
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_turnManager.Update();
    }

    public void SetInitialTile(Unit unit, Tile tile)
    {
        unit.SetCurrentTile(tile);
        unit.transform.position = tile.position;
        tile.SetCurrentUnit(unit);
    }

    public enum UnitControllerEnum
    {
        player,
        debugAI
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
}
