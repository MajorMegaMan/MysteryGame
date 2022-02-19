﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : PooledObject, ITurnTaker
{
    // Gameplay var
    [SerializeField] GameManager m_gameManager = null;
    [SerializeField] UnitProfile m_profile = null;
    UsableUnitStats m_usableStats;

    // Models and animation
    [SerializeField] ModelObject m_modelObject = null;
    [SerializeField] Animator m_anim = null;

    // UI Elements
    [SerializeField] Slider m_healthBar = null;
    [SerializeField] Transform m_healthBarContainer = null;

    // Input controller
    UnitController m_unitController = null;

    // Gameplay map var
    Tile m_currentTile = null;

    // Internal State machine logic
    Vector3 m_targetPosition = Vector3.zero;
    [SerializeField] float m_moveSpeed = 5.0f;
    [SerializeField] Tile.Access m_tileAccess = 0;

    Tile.NeighbourDirection m_currentLookDirection = 0;

    delegate void VoidFunc();
    VoidFunc m_moveAction = () => { };

    public delegate void StatsAction(UsableUnitStats usableUnitStats);
    StatsAction m_statChangeEvent;

    bool m_isMoving = false;

    // getters
    public bool isMoving { get { return m_isMoving; } }
    public Tile currentTile { get { return m_currentTile; } }
    public Tile.Access tileAccess { get { return m_tileAccess; } }
    public Tile.NeighbourDirection currentLookDirection { get { return m_currentLookDirection; } }

    public UnitProfile profile { get { return m_profile; } }
    public ModelObject modelObject { get { return m_modelObject; } }
    public TurnManager<Unit> turnManager { get { return m_gameManager.turnManager; } }
    public UnitManager unitManager { get { return m_gameManager.unitManager; } }
    public UsableUnitStats usableStats { get { return m_usableStats; } }

    #region UnityRelated
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        m_moveAction.Invoke();
    }

    // Use this instantiate method to create units as this will attach all the necessary components.
    public static Unit InstantiateUnit(GameManager gameManager, Unit prefab, Transform unitContainer)
    {
        Unit newUnit = Object.Instantiate(prefab, unitContainer);
        newUnit.SetGameManager(gameManager);
        return newUnit;
    }
    #endregion

    public void SetGameManager(GameManager gameManager)
    {
        m_gameManager = gameManager;
    }

    // When a profile is attached to a unit it will also need to update that unit's mesh and material
    public void SetProfile(UnitProfile profile, ModelObject modelObject)
    {
        m_profile = profile;

        modelObject.SetCurrentUnit(this);

        m_modelObject = modelObject;
        m_anim = m_modelObject.GetComponent<Animator>();

        // grab the stats from the profile. Will need to know if it should load stats or generate them, but for now, just generate
        m_usableStats = UsableUnitStats.GenerateOnUnit(this, profile, 1);

        name = profile.unitName;

        m_healthBar.maxValue = m_usableStats.maxHealth;
        m_healthBar.value = m_usableStats.currentHealth;
    }

    // Needs to also show/hide health bar when the unit is activated in the pool.
    public override void SetIsActiveInPool(bool isActive)
    {
        base.SetIsActiveInPool(isActive);
        m_healthBar.gameObject.SetActive(isActive);
    }

    #region GameMapNavigation
    void MoveToTargetPos()
    {
        Vector3 toTarget = m_targetPosition - transform.position;
        float distance = toTarget.magnitude;
        float deltaSpeed = Time.deltaTime * m_moveSpeed;
        if (distance < deltaSpeed)
        {
            transform.position = m_targetPosition;
            m_moveAction = () => { };
            m_isMoving = false;

            m_anim.SetFloat("Movement", 0.0f);
        }
        else
        {
            transform.position += (toTarget / distance) * deltaSpeed;
        }
    }

    void SetPosition(Vector3 position)
    {
        m_targetPosition = position;
        m_moveAction = MoveToTargetPos;
        m_isMoving = true;

        m_anim.SetFloat("Movement", 1.0f);
    }

    public void SetCurrentTile(Tile target)
    {
        m_currentTile = target;
        SetPosition(target.position);
    }

    public void EnterTile(Tile target)
    {
        m_currentTile.SetCurrentUnit(null);
        SetCurrentTile(target);
        m_currentTile.SetCurrentUnit(this);
    }

    public void SafeEnterTile(Tile target)
    {
        if(m_currentTile != null)
        {
            m_currentTile.SetCurrentUnit(null);
        }
        SetCurrentTile(target);
        m_currentTile.SetCurrentUnit(this);
    }

    // Force moving into a tile without checking if the unit is able to
    public void ForceMoveToNeighbourTile(Tile.NeighbourDirection direction)
    {
        Tile neighbour = m_currentTile.GetNeighbour(direction);
        if(neighbour != null)
        {
            EnterTile(neighbour);
        }
    }

    // Safely move to neighbour tile with a check if the unit is able before moving.
    public void MoveToNeighbourTile(Tile.NeighbourDirection direction)
    {
        if(CheckNeighbourTileMove(direction, out Tile neighbourTile))
        {
            EnterTile(neighbourTile);
        }
    }

    // returns true if the unit is able to move into the neighbouring tile
    public bool CheckNeighbourTileMove(Tile.NeighbourDirection direction, out Tile neighbourTile)
    {
        neighbourTile = m_currentTile.GetNeighbour(direction);
        if(neighbourTile == null)
        {
            return false;
        }

        if (m_tileAccess < neighbourTile.access || neighbourTile.GetCurrentUnit() != null)
        {
            return false;
        }

        // Check if trying to move diagonally
        if (direction > Tile.NeighbourDirection.down)
        {
            if (CheckDiagonalAccess(m_currentTile, direction))
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
    bool CheckDiagonalAccess(Tile currentTile, Tile.NeighbourDirection direction)
    {
        if (m_tileAccess > Tile.Access.partial)
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
    bool CheckPartialAccess(Tile currentTile, Tile.NeighbourDirection vertDirection, Tile.NeighbourDirection horiDirection)
    {
        Tile vertNeighbour = currentTile.GetNeighbour(vertDirection);
        Tile horiNeighbour = currentTile.GetNeighbour(horiDirection);

        return vertNeighbour.access <= Tile.Access.partial && horiNeighbour.access <= Tile.Access.partial;
    }

    public void LookTowards(Tile.NeighbourDirection direction)
    {
        m_currentLookDirection = direction;

        // For now, dodgy way to get the unit to face the direction it's moving
        Vector3 assumedNeighbourOffset = m_currentTile.GetNeighbourOffset(direction);
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
        Tile targetTile = m_currentTile.GetNeighbour(direction);
        if(targetTile != null)
        {
            Unit targetUnit = targetTile.GetCurrentUnit();
            if(targetUnit != null)
            {
                targetUnit.ReceiveDamage(CalcAttackDamage());
            }
        }
    }

    public void EndTurnAction()
    {
        // This is now a solid point where the unit will end their turn. End turn actions can be inserted here
        turnManager.EndCurrentTurn();
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
        m_usableStats.LevelUp();

        StatChangeEvent();
        m_healthBar.maxValue = m_usableStats.maxHealth;
    }

    public float CalcAttackDamage()
    {
        return m_usableStats.CalcAttackDamage();
    }

    public float CalcDamageToHealth(float attackValue)
    {
        float result = m_usableStats.CalcDamageToHealth(attackValue);
        return result;
    }

    public void ReceiveDamage(float attackValue)
    {
        m_usableStats.ReceiveDamage(attackValue);

        StatChangeEvent();
        m_healthBar.value = m_usableStats.currentHealth;
    }
    void StatChangeEvent()
    {
        m_statChangeEvent?.Invoke(m_usableStats);
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

    public void ShowHealthBar(bool shouldShow)
    {
        m_healthBar.gameObject.SetActive(shouldShow);
    }

    public void SetHealthBarPos(Camera camera)
    {
        m_healthBarContainer.rotation = camera.transform.rotation;
    }

    float ITurnTaker.GetTurnValue()
    {
        return m_usableStats.speed;
    }

    ITurnAction ITurnTaker.FindUnitTurnAction()
    {
        return FindUnitTurnAction();
    }

    bool ITurnTaker.IsEngaged()
    {
        return m_isMoving;
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
}