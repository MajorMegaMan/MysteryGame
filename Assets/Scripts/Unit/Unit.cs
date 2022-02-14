using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : PooledObject
{
    // Gameplay var
    [SerializeField] UnitProfile m_profile = null;
    [SerializeField] GameManager m_gameManager = null;
    UsableUnitStats m_usableStats;

    // Models and animation
    [SerializeField] ModelObject m_modelObject = null;
    [SerializeField] Animator m_anim = null;
    [SerializeField, Range(0.0f, 1.0f)] float m_crossFadeTime = 0.2f;
    float m_runAnimNormalisedTime = 0.0f;


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

    bool m_isMoving = false;

    // getters
    public bool isMoving { get { return m_isMoving; } }
    public Tile currentTile { get { return m_currentTile; } }
    public Tile.Access tileAccess { get { return m_tileAccess; } }
    public Tile.NeighbourDirection currentLookDirection { get { return m_currentLookDirection; } }

    public UnitProfile profile { get { return m_profile; } }
    public ModelObject modelObject { get { return m_modelObject; } }
    public TurnManager turnManager { get { return m_gameManager.turnManager; } }
    public UnitManager unitManager { get { return m_gameManager.unitManager; } }
    public UsableUnitStats usableStats { get { return m_usableStats; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        m_moveAction.Invoke();
    }

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
    }

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

            AnimatorStateInfo stateInfo = m_anim.GetCurrentAnimatorStateInfo(0);
            m_runAnimNormalisedTime = stateInfo.normalizedTime;
            m_anim.CrossFade("Idle", m_crossFadeTime);
            //m_anim.SetTrigger("Idle");
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

        m_anim.CrossFade("Run", m_crossFadeTime, 0, m_runAnimNormalisedTime);
        
        //m_anim.SetTrigger("Run");
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

    public void SetUnitController(UnitController unitController)
    {
        m_unitController = unitController;
    }

    public IUnitTurnAction FindUnitTurnAction()
    {
        IUnitTurnAction turnAction = m_unitController.FindUnitTurnAction(this, out Tile.NeighbourDirection direction);
        LookTowards(direction);
        return turnAction;
    }

    public void Attack(Tile.NeighbourDirection direction)
    {
        m_anim.CrossFade("Attack", m_crossFadeTime);
        Tile targetTile = m_currentTile.GetNeighbour(direction);
        if(targetTile != null)
        {
            Unit targetUnit = targetTile.GetCurrentUnit();
            if(targetUnit != null)
            {
                targetUnit.m_usableStats.ReceiveDamage(m_usableStats.CalcAttackDamage());
            }
        }
    }

    public void EndTurnAction()
    {
        m_anim.CrossFade("Idle", m_crossFadeTime);
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
}
