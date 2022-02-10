using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameMap m_gameMap = null;
    [SerializeField] Unit m_player = null;
    TurnManager m_turnManager = new TurnManager();

    [SerializeField] Unit m_debugAIUnit = null;

    public TurnManager turnManager { get { return m_turnManager; } }

    private void Awake()
    {
        m_turnManager.Initialise(m_player);
        CreateControllers();

        m_player.SetUnitController(GetUnitController(UnitControllerEnum.player));

        m_debugAIUnit.SetUnitController(GetUnitController(UnitControllerEnum.debugAI));
        m_turnManager.AddUnit(m_debugAIUnit);
    }

    // Start is called before the first frame update
    void Start()
    {
        Tile playerTile = m_gameMap.GetTile(m_gameMap.startRoom.GetCentre());
        SetInitialTile(m_player, playerTile);

        int randX = Random.Range(m_gameMap.startRoom.startX, m_gameMap.startRoom.endX);
        int randY = Random.Range(m_gameMap.startRoom.startY, m_gameMap.startRoom.endY);
        Tile botTile = m_gameMap.GetTile(randX, randY);
        while(botTile.GetCurrentUnit() != null)
        {
            randX = Random.Range(m_gameMap.startRoom.startX, m_gameMap.startRoom.endX);
            randY = Random.Range(m_gameMap.startRoom.startY, m_gameMap.startRoom.endY);
            botTile = m_gameMap.GetTile(randX, randY);
        }
        SetInitialTile(m_debugAIUnit, botTile);
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
