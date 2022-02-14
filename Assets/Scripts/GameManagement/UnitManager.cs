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

    Dictionary<UnitProfileKey, UnitProfile> m_unitProfileDictionary = null;
    Dictionary<UnitProfileKey, GameObjectPool<ModelObject>> m_unitProfileModelDictionary = null;

    List<Unit> m_activeUnits = null;

    public Unit[] activeUnits { get { return m_activeUnits.ToArray(); } }

    public UnitManager(GameManager gameManager, Unit unitPrefab, int unitPoolCount, SerialisedKeyValue<UnitProfileKey, UnitProfile>[] unitProfiles, int initialUnitModelcount, Transform unitContainer, Transform modelContainer)
    {
        Init(gameManager, unitPrefab, unitPoolCount, unitProfiles, initialUnitModelcount, unitContainer, modelContainer);
    }

    void Init(GameManager gameManager, Unit unitPrefab, int unitPoolCount, SerialisedKeyValue<UnitProfileKey, UnitProfile>[] unitProfiles, int initialUnitModelcount, Transform unitContainer, Transform modelContainer)
    {
        m_gameManager = gameManager;
        m_unitContainer = unitContainer;
        m_modelContainer = modelContainer;

        m_unitPrefab = unitPrefab;

        m_unitPool = new GameObjectPool<Unit>(m_unitPrefab, unitPoolCount, InstantiateUnit, DestroyUnit);

        m_unitProfileDictionary = new Dictionary<UnitProfileKey, UnitProfile>();
        foreach (var unitProfile in unitProfiles)
        {
            m_unitProfileDictionary.Add(unitProfile.key, unitProfile.value);
        }

        m_unitProfileModelDictionary = new Dictionary<UnitProfileKey, GameObjectPool<ModelObject>>();
        foreach (var pair in m_unitProfileDictionary)
        {
            var profile = pair.Value;
            m_unitProfileModelDictionary[pair.Key] = new GameObjectPool<ModelObject>(profile.modelObjectPrefab, initialUnitModelcount, InstantiateModel, DestroyModel);
        }

        m_activeUnits = new List<Unit>(unitPoolCount);
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

    public Unit SpawnUnit(UnitProfileKey unitProfileKey)
    {
        Unit unit = m_unitPool.ActivateObject();
        ModelObject modelObject = GetModelObject(unitProfileKey);

        unit.SetProfile(m_unitProfileDictionary[unitProfileKey], modelObject);

        m_activeUnits.Add(unit);

        return unit;
    }

    public void DespawnUnit(Unit unit)
    {
        unit.currentTile.SetCurrentUnit(null);
        //unit.SetCurrentTile(null);
        unit.modelObject.DetachCurrentUnit(m_modelContainer);
        m_unitPool.DeactivateObject(unit);

        m_activeUnits.Remove(unit);
    }

    ModelObject GetModelObject(UnitProfileKey unitProfileKey)
    {
        var modelPool = m_unitProfileModelDictionary[unitProfileKey];
        ModelObject modelObject = modelPool.ActivateObject();

        // if there was no model object found, expand the pool to find one.
        // Limits of units will be maintained elsewhere and there should always be an available model.
        if(modelObject == null)
        {
            modelPool.ExpandPool(m_unitProfileDictionary[unitProfileKey].modelObjectPrefab, 1);
            modelObject = modelPool.ActivateObject();
        }
        return modelObject;
    }
}
