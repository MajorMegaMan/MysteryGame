using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewUnitStats
{
    public class NewUnitManager : SingletonBase<NewUnitManager>
    {
        GameObjectPool<UnitTokenStub> m_unitTokens = null;
        GameObjectPool<NewModelObject> m_modelObjects = null;

        NewUnitManager()
        {

        }

        public NewUnitManager(NewUnitManagerPackage package)
        {
            m_unitTokens = new GameObjectPool<UnitTokenStub>(package.unitTokenPrefab, package.unitTokenPoolCapacity, CreateUnitTokenStub, DestroyUnitToken);
            m_modelObjects = new GameObjectPool<NewModelObject>(package.modelObjectPrefab, package.modelObjectPoolCapacity, CreateModelObject, DestroyModelObject);
        }

        public void Init(NewUnitManagerPackage package)
        {
            m_unitTokens = new GameObjectPool<UnitTokenStub>(package.unitTokenPrefab, package.unitTokenPoolCapacity, CreateUnitTokenStub, DestroyUnitToken);
            m_modelObjects = new GameObjectPool<NewModelObject>(package.modelObjectPrefab, package.modelObjectPoolCapacity, CreateModelObject, DestroyModelObject);
        }

        #region UnitToken
        static UnitTokenStub CreateUnitTokenStub(UnitTokenStub unitTokenPrefab)
        {
            UnitTokenStub newToken = Object.Instantiate(unitTokenPrefab);
            return newToken;
        }

        static void DestroyUnitToken(UnitTokenStub unitToken)
        {
            Object.Destroy(unitToken);
        }
        #endregion
        #region ModelManagement
        static NewModelObject CreateModelObject(NewModelObject modelObjectPrefab)
        {
            NewModelObject newModelObject = Object.Instantiate(modelObjectPrefab);
            return newModelObject;
        }

        static void DestroyModelObject(NewModelObject modelObject)
        {
            Object.Destroy(modelObject);
        }
        #endregion

        #region UnitManagement
        // Spawn unit token based on unit profile. This will mostly be used to Generate a character
        public UnitTokenStub SpawnUnit(UnitProfile unitProfile, GameMapTile tile)
        {
            UnitTokenStub toSpawn = m_unitTokens.ActivateObject();
            if (toSpawn != null)
            {
                NewModelObject modelObject = m_modelObjects.ActivateObject();
                if (modelObject != null)
                {
                    toSpawn.GenerateUnitFromProfile(unitProfile);
                    toSpawn.SetModelObject(modelObject);
                    MovingTokenManager.SetTokenToTile(toSpawn, tile);
                    toSpawn.SetPositionToTile();

                    return toSpawn;
                }
                else
                {
                    // failed to create model for token
                    return null;
                }
            }
            else
            {
                // Failed to create a new unit
                return null;
            }
        }

        // Spawn unit Token based on characterInfo. This will mostly be used to load saved characters
        public UnitTokenStub SpawnUnit(UnitCharacterInfo character, GameMapTile tile)
        {
            UnitTokenStub toSpawn = m_unitTokens.ActivateObject();
            if (toSpawn != null)
            {
                NewModelObject modelObject = m_modelObjects.ActivateObject();
                if (modelObject != null)
                {
                    toSpawn.GenerateFromCharacterInfo(character);

                    toSpawn.SetModelObject(modelObject);
                    MovingTokenManager.SetTokenToTile(toSpawn, tile);

                    toSpawn.SetPositionToTile();

                    return toSpawn;
                }
                else
                {
                    // failed to create model for token
                    return null;
                }
            }
            else
            {
                // Failed to create a new unit
                return null;
            }
        }
        #endregion
    }

    [System.Serializable]
    public class NewUnitManagerPackage
    {
        public UnitTokenStub unitTokenPrefab = null;
        public int unitTokenPoolCapacity = 5;

        public NewModelObject modelObjectPrefab = null;
        public int modelObjectPoolCapacity = 5;
    }
}
