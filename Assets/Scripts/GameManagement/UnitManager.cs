using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This will be a class that has the logic needed for the game to spawn or despawn units.
// This will contain object pools for the base unit classes
// This will contain other object pools for each differening unit model grabbed from unit prorfiles.
// This will conatain a list of unit profiles
public class UnitManager
{
    GameManager m_gameManager = null;

    Transform m_unitContainer = null;
    Transform m_modelContainer = null;

    Unit m_unitPrefab = null;
    GameObjectPool<Unit> m_unitPool;

    UnitProfile[] m_unitProfiles = null;

    // This is dumb that I'm using a class as a kye value. 
    // I would like to use an enum or something. BUT, specifically an enum would require knowing all the actual profiles.
    // Currently, while debugging and creating the system. I don't know what the profiles will be.
    Dictionary<UnitProfile, GameObjectPool<ModelObject>> m_unitProfileModelDictionary;

    public UnitManager(GameManager gameManager, Unit unitPrefab, int unitPoolCount, UnitProfile[] unitProfiles, int initialUnitModelcount, Transform unitContainer, Transform modelContainer)
    {
        m_gameManager = gameManager;
        m_unitContainer = unitContainer;
        m_modelContainer = modelContainer;

        m_unitPrefab = unitPrefab;

        m_unitPool = new GameObjectPool<Unit>(m_unitPrefab, unitPoolCount, InstantiateUnit, DestroyUnit);
        
        m_unitProfiles = unitProfiles;
        m_unitProfileModelDictionary = new Dictionary<UnitProfile, GameObjectPool<ModelObject>>();
        for(int i = 0; i < m_unitProfiles.Length; i++)
        {
            var unitModelPool = new GameObjectPool<ModelObject>(m_unitProfiles[i].modelObjectPrefab, initialUnitModelcount, InstantiateModel, DestroyModel);
            m_unitProfileModelDictionary[m_unitProfiles[i]] = unitModelPool;
        }
    }

    Unit InstantiateUnit(Unit prefab)
    {
        Unit newUnit = Object.Instantiate(prefab, m_unitContainer);
        newUnit.SetGameManager(m_gameManager);
        return newUnit;
    }

    void DestroyUnit(Unit targetUnit)
    {
        Object.Destroy(targetUnit.gameObject);
    }

    ModelObject InstantiateModel(ModelObject prefab)
    {
        ModelObject newUnit = Object.Instantiate(prefab, m_modelContainer);
        return newUnit;
    }

    void DestroyModel(ModelObject targetModel)
    {
        Object.Destroy(targetModel.gameObject);
    }

    public Unit SpawnUnit(UnitProfile unitProfile)
    {
        Unit unit = m_unitPool.ActivateObject();

        ModelObject modelObject = GetModelObject(unitProfile);

        unit.SetProfile(unitProfile, modelObject);

        return unit;
    }

    public void DespawnUnit(Unit unit)
    {
        unit.modelObject.DetachCurrentUnit(m_modelContainer);
        m_unitPool.DeactivateObject(unit);
    }

    ModelObject GetModelObject(UnitProfile unitProfile)
    {
        var modelPool = m_unitProfileModelDictionary[unitProfile];
        ModelObject modelObject = modelPool.ActivateObject();
        if(modelObject == null)
        {
            modelPool.ExpandPool(unitProfile.modelObjectPrefab, 1);
            modelObject = modelPool.ActivateObject();
        }
        return modelObject;
    }
}
