using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MysterySystems.UnitStats;

public class Unit : GameMapToken, ITurnTaker, IPooledObject
{
    // Gameplay var
    [SerializeField] GameManager m_gameManager = null;
    UnitStats m_unitStats = null;
    Dictionary<EOTConditionKey, UnitEndTurnCondition> m_endOfTurnConditions = new Dictionary<EOTConditionKey, UnitEndTurnCondition>();
    Inventory<Unit> m_inventory = null;
    EquipmentInventory m_equipment = null;

    // Models and animation
    [SerializeField] ModelObject m_modelObject = null;
    [SerializeField] Animator m_anim = null;
    float m_currentMoveAnimationValue = 0.0f;
    float m_targetMoveAnimationValue = 0.0f;
    float m_animationVelocity = 0.0f;
    [SerializeField] float m_animationSmoothTime = 0.1f;

    // UI Elements
    [SerializeField] Slider m_healthBar = null;
    [SerializeField] Transform m_healthBarContainer = null;

    // Input controller
    UnitController m_unitController = null;

    // Gameplay map var

    // Internal State machine logic
    [SerializeField] GameMapTile.Access m_tileAccess = 0;

    Tile.NeighbourDirection m_currentLookDirection = 0;

    public delegate void StatsAction(UnitStats unitStats);
    StatsAction m_statChangeEvent;

    // getters
    public GameMapTile.Access tileAccess { get { return m_tileAccess; } }
    public Tile.NeighbourDirection currentLookDirection { get { return m_currentLookDirection; } }

    public ModelObject modelObject { get { return m_modelObject; } }
    public TurnManager<Unit> turnManager { get { return m_gameManager.turnManager; } }
    public UnitManager unitManager { get { return m_gameManager.unitManager; } }
    public UnitStats unitStats { get { return m_unitStats; } }
    public UnitProfile profile { get { return m_unitStats.profile; } }
    public Inventory<Unit> inventory { get { return m_inventory; } }
    public EquipmentInventory equipment { get { return m_equipment; } }

    #region TokenMethods
    public override int GetTokenID()
    {
        return (int)TempTokenID.unit;
    }

    public override void OnMoveBegin(TokenMover tokenMover)
    {
        m_targetMoveAnimationValue = 1.0f;
        m_anim.SetFloat("Movement", m_targetMoveAnimationValue);
    }

    public override void OnMoveArrive()
    {
        m_targetMoveAnimationValue = 0.0f;
        m_anim.SetFloat("Movement", m_targetMoveAnimationValue);
    }

    public override void OnEnterTile(GameMapTile gameMapTile)
    {
        // Get the items on the tile.
        var itemToken = gameMapTile.GetToken(TempTokenID.item) as ItemToken;
        if (itemToken != null)
        {
            // There is an item on the ground. Try to add it to the inventory
            if(m_inventory.AddItem(itemToken.item))
            {
                // picked up item on the ground
                m_gameManager.turnLog.AddMessage(name + " picked up " + itemToken.item.itemName);
                itemToken.ReleaseToken();
            }
            else
            {
                // could not pick up ground item
                m_gameManager.turnLog.AddMessage(name + " stepped on " + itemToken.item.itemName);
            }
        }
    }
    #endregion

    #region UnityRelated
    // Update is called once per frame
    void LateUpdate()
    {
        m_currentMoveAnimationValue = Mathf.SmoothDamp(m_currentMoveAnimationValue, m_targetMoveAnimationValue, ref m_animationVelocity, m_animationSmoothTime);
        //m_anim.SetFloat("Movement", m_currentMoveAnimationValue);
    }

    // Use this instantiate method to create units as this will attach all the necessary components.
    public static Unit InstantiateUnit(GameManager gameManager, Unit prefab, Transform unitContainer)
    {
        Unit newUnit = Object.Instantiate(prefab, unitContainer);
        newUnit.SetGameManager(gameManager);
        newUnit.m_equipment = new EquipmentInventory(newUnit);
        return newUnit;
    }
    #endregion

    public void SetGameManager(GameManager gameManager)
    {
        m_gameManager = gameManager;
    }

    // This is the main function that is called when a Unit is spawned into the map
    // When a profile is attached to a unit it will also need to update that unit's mesh and material
    public void SetProfile(UnitProfile profile, ModelObject modelObject)
    {
        modelObject.SetCurrentUnit(this);

        m_modelObject = modelObject;
        m_anim = m_modelObject.GetComponent<Animator>();

        // grab the stats from the profile. Will need to know if it should load stats or generate them, but for now, just generate
        m_unitStats = new UnitStats(profile, 1);

        name = profile.profileName;

        ResourceStat healthStat = m_unitStats.GetStat(ResourceStatKey.health);
        healthStat.Reset();
        m_healthBar.maxValue = healthStat.maxValue;
        m_healthBar.value = healthStat.value;

        // For now, inventory can be set here. But it would eventually have a loot table associated with it's unit profile IF it is bot controlled.
        // Player units will load their own inventorys designated by the player.
        m_inventory = new Inventory<Unit>(this);
    }

    // Needs to also show/hide health bar when the unit is activated in the pool.
    public void SetIsActiveInPool(bool isActive)
    {
        gameObject.SetActive(isActive);
        m_healthBar.gameObject.SetActive(isActive);
    }

    #region GameMapNavigation

    // returns true if the unit is able to move into the neighbouring tile
    public bool CheckNeighbourTileMove(Tile.NeighbourDirection direction, out GameMapTile neighbourTile)
    {
        neighbourTile = currentTile.GetNeighbour<GameMapTile>(direction);
        if(neighbourTile == null)
        {
            return false;
        }

        if (m_tileAccess < neighbourTile.access || neighbourTile.GetToken(TempTokenID.unit) != null)
        {
            return false;
        }

        // Check if trying to move diagonally
        if (direction > Tile.NeighbourDirection.down)
        {
            if (CheckDiagonalAccess(currentTile, direction))
            {
                // player can move into tile
                return true;
            }

            return false;
        }
        else
        {
            // can freely move
            return true;
        }
    }

    // returns true if player can cut corners
    bool CheckDiagonalAccess(GameMapTile currentTile, Tile.NeighbourDirection direction)
    {
        if (m_tileAccess > GameMapTile.Access.partial)
        {
            return true;
        }

        switch (direction)
        {
            case Tile.NeighbourDirection.upLeft:
                {
                    return CheckPartialAccess(currentTile, Tile.NeighbourDirection.up, Tile.NeighbourDirection.left);
                }
            case Tile.NeighbourDirection.upRight:
                {
                    return CheckPartialAccess(currentTile, Tile.NeighbourDirection.up, Tile.NeighbourDirection.right);
                }
            case Tile.NeighbourDirection.downLeft:
                {
                    return CheckPartialAccess(currentTile, Tile.NeighbourDirection.down, Tile.NeighbourDirection.left);
                }
            case Tile.NeighbourDirection.downRight:
                {
                    return CheckPartialAccess(currentTile, Tile.NeighbourDirection.down, Tile.NeighbourDirection.right);
                }
            default:
                {
                    return false;
                }
        }
    }

    // returns true if both neighbour directions are partial
    bool CheckPartialAccess(GameMapTile currentTile, Tile.NeighbourDirection vertDirection, Tile.NeighbourDirection horiDirection)
    {
        GameMapTile vertNeighbour = currentTile.GetNeighbour<GameMapTile>(vertDirection);
        GameMapTile horiNeighbour = currentTile.GetNeighbour<GameMapTile>(horiDirection);

        return vertNeighbour.access <= GameMapTile.Access.partial && horiNeighbour.access <= GameMapTile.Access.partial;
    }

    public void LookTowards(Tile.NeighbourDirection direction)
    {
        m_currentLookDirection = direction;

        // For now, dodgy way to get the unit to face the direction it's moving
        Vector3 assumedNeighbourOffset = currentTile.GetNeighbourOffset(direction);
        transform.forward = assumedNeighbourOffset;
    }
    #endregion

    public void SetUnitController(UnitController unitController)
    {
        m_unitController = unitController;
    }

    public ITurnAction FindUnitTurnAction()
    {
        ITurnAction turnAction = m_unitController.FindUnitTurnAction(this, out Tile.NeighbourDirection direction);
        LookTowards(direction);
        return turnAction;
    }

    public void Attack(Tile.NeighbourDirection direction)
    {
        m_anim.SetTrigger("Attack");
        GameMapTile targetTile = currentTile.GetNeighbour<GameMapTile>(direction);
        if(targetTile != null)
        {
            GameMapToken targetToken = targetTile.GetToken(TempTokenID.unit);
            if(targetToken != null)
            {
                Unit targetUnit = targetToken as Unit;
                float damage = targetUnit.ReceiveDamage(CalcAttackDamage());
                m_gameManager.turnLog.AddMessage(name + " attacked " + targetToken.name + " for " + damage + " damage.");
            }
        }
    }

    public void EndTurnAction()
    {
        // This is now a solid point where the unit will end their turn. End turn actions can be inserted here
        turnManager.EndCurrentTurn();
        foreach(var EOTConditionPair in m_endOfTurnConditions)
        {
            EOTConditionPair.Value.PerformAction();
        }
    }

    public void Die()
    {
        // remove from turn manager
        turnManager.RemoveUnit(this);

        // It would be ideal to play a death animation first, but for now just despawn the unit
        // play animation
        //m_anim.CrossFade("Death", m_crossFadeTime);

        // clean up corpse
        unitManager.DespawnUnit(this);
    }

    #region StatUsage
    // These are buffer funtions to be able to control when these events take place. Such as controlling health bars when the unit takes damage
    public void LevelUp()
    {
        m_unitStats.LevelUp();

        InvokeStatChangeEvent();
        m_healthBar.maxValue = m_unitStats.GetStat(ResourceStatKey.health).maxValue;
    }

    public float CalcAttackDamage()
    {
        // This will eventually have bonuses from weapons and the like
        return m_unitStats.GetStat(CoreStatKey.strength).value;
    }

    public float CalcDamageToHealth(float attackValue)
    {
        // This will eventually have defense values intergrated into it
        // Things like armour or even natural defense values
        float result = attackValue;
        return result;
    }

    public float ReceiveDamage(float attackValue)
    {
        ResourceStat healthStat = m_unitStats.GetStat(ResourceStatKey.health);

        float damageTohealth = CalcDamageToHealth(attackValue);
        healthStat.value -= damageTohealth;
        if(healthStat.value <= 0)
        {
            Die();
        }

        InvokeStatChangeEvent();
        m_healthBar.value = healthStat.value;
        return damageTohealth;
    }

    public void InvokeStatChangeEvent()
    {
        m_statChangeEvent?.Invoke(m_unitStats);
    }

    public void AddStatChangeListener(StatsAction statsAction)
    {
        m_statChangeEvent += statsAction;
    }

    public void RemoveStatChangeListener(StatsAction statsAction)
    {
        m_statChangeEvent -= statsAction;
    }
    #endregion

    #region UIUsage
    public void ShowHealthBar(bool shouldShow)
    {
        m_healthBar.gameObject.SetActive(shouldShow);
    }

    public void SetHealthBarPos(Camera camera)
    {
        m_healthBarContainer.rotation = camera.transform.rotation;
    }
    #endregion

    #region TurnLogic
    float ITurnTaker.GetTurnValue()
    {
        return m_unitStats.GetStat(CoreStatKey.speed).value;
    }

    ITurnAction ITurnTaker.FindUnitTurnAction()
    {
        return FindUnitTurnAction();
    }

    bool ITurnTaker.IsEngaged()
    {
        return isMoving;
    }

    void ITurnTaker.EndTurn()
    {
        EndTurnAction();
    }

    public void LogAction(string actionName)
    {
        UnitActionLogInfo logInfo = new UnitActionLogInfo();
        logInfo.unitName = name;
        logInfo.actionName = actionName;
        logInfo.direction = m_currentLookDirection;

        m_gameManager.unitActionLog.actionLog.Add(logInfo);
    }
    #endregion

    public void AddEndOfTurnCondition(EOTConditionKey conditionKey)
    {
        var newCondition = UnitEndTurnCondition.CreateCondition(conditionKey);
        newCondition.Init(this);
        m_endOfTurnConditions[conditionKey] = newCondition;
    }

    public void RemoveEndOfTurnCondition(EOTConditionKey conditionKey)
    {
        m_endOfTurnConditions.Remove(conditionKey);
    }
}
