using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MysterySystems.UnitStats;

// This will be a class that has the logic needed for the game to spawn or despawn units.
// This will contain object pools for the base unit classes
// This will contain other object pools for each differing unit model grabbed from unit profiles.
// This will conatain a list of unit profiles
public class UnitManager
{
    GameManager m_gameManager = null;

    Transform m_unitContainer = null;
    Transform m_modelContainer = null;

    Unit m_unitPrefab = null;
    GameObjectPool<Unit> m_unitPool;

    Dictionary<UnitProfileKey, UnitProfile> m_unitProfileDictionary = null;
    Dictionary<UnitProfileKey, ModelObject> m_debugModelObjectPrefabDictionary = null;
    Dictionary<UnitProfileKey, GameObjectPool<ModelObject>> m_unitProfileModelPoolDictionary = null;

    List<Unit> m_activeUnits = null;

    [System.Serializable]
    public class UnitManagerPackage
    {
        public Unit unitPrefab;
        public int unitPoolCount;
        public SerialisedKeyValue<UnitProfileKey, ScriptableUnitProfile>[] unitProfiles;
        public int initialUnitModelcount;

        // containers
        public Transform unitContainer;
        public Transform modelContainer;
    }

    public Unit[] activeUnits { get { return m_activeUnits.ToArray(); } }

    public UnitManager(GameManager gameManager, UnitManagerPackage package)
    {
        Init(gameManager, package);
    }

    void Init(GameManager gameManager, UnitManagerPackage package)
    {
        // set up references
        m_gameManager = gameManager;
        m_unitContainer = package.unitContainer;
        m_modelContainer = package.modelContainer;

        m_unitPrefab = package.unitPrefab;

        // Set up dictionarys
        m_unitPool = new GameObjectPool<Unit>(m_unitPrefab, package.unitPoolCount, InstantiateUnit, DestroyUnit);

        m_unitProfileDictionary = new Dictionary<UnitProfileKey, UnitProfile>();
        m_debugModelObjectPrefabDictionary = new Dictionary<UnitProfileKey, ModelObject>();
        m_unitProfileModelPoolDictionary = new Dictionary<UnitProfileKey, GameObjectPool<ModelObject>>();

        foreach (var unitProfile in package.unitProfiles)
        {
            m_unitProfileDictionary.Add(unitProfile.key, new UnitProfile(unitProfile.value.testProfile));
            m_debugModelObjectPrefabDictionary[unitProfile.key] = unitProfile.value.modelObjectPrefab;
            m_unitProfileModelPoolDictionary[unitProfile.key] = new GameObjectPool<ModelObject>(unitProfile.value.modelObjectPrefab, package.initialUnitModelcount, InstantiateModel, DestroyModel);
        }

        m_activeUnits = new List<Unit>(package.unitPoolCount);
    }

    Unit InstantiateUnit(Unit prefab)
    {
        return Unit.InstantiateUnit(m_gameManager, prefab, m_unitContainer);
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
        var modelPool = m_unitProfileModelPoolDictionary[unitProfileKey];
        ModelObject modelObject = modelPool.ActivateObject();

        // if there was no model object found, expand the pool to find one.
        // Limits of units will be maintained elsewhere and there should always be an available model.
        if(modelObject == null)
        {
            modelPool.ExpandPool(m_debugModelObjectPrefabDictionary[unitProfileKey], 1);
            modelObject = modelPool.ActivateObject();
        }
        return modelObject;
    }
}
