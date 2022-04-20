using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewUnitStats
{
    public class NewItemManager : SingletonBase<NewItemManager>
    {
        GameObjectPool<ItemTokenStub> m_itemTokens = null;

        NewItemManager()
        {
            
        }

        public NewItemManager(NewItemManagerPackage package)
        {
            m_itemTokens = new GameObjectPool<ItemTokenStub>(package.itemTokenPrefab, package.itemTokenPoolCapacity, CreateItemToken, DestroyItemToken);
        }

        public void Init(NewItemManagerPackage package)
        {
            m_itemTokens = new GameObjectPool<ItemTokenStub>(package.itemTokenPrefab, package.itemTokenPoolCapacity, CreateItemToken, DestroyItemToken);
        }

        #region ItemToken
        static ItemTokenStub CreateItemToken(ItemTokenStub itemTokenPrefab)
        {
            ItemTokenStub newItemToken = Object.Instantiate(itemTokenPrefab);
            return newItemToken;
        }

        static void DestroyItemToken(ItemTokenStub itemToken)
        {
            Object.Destroy(itemToken);
        }
        #endregion

        #region ItemManagement
        public ItemTokenStub SpawnItem(Inventory.BaseItemStub item, GameMapTile tile)
        {
            ItemTokenStub toSpawn = m_itemTokens.ActivateObject();
            if (toSpawn != null)
            {
                toSpawn.SetItem(item);

                MovingTokenManager.SetTokenToTile(toSpawn, tile);
                toSpawn.SetPositionToTile();

                return toSpawn;
            }
            else
            {
                // Failed to create a new unit
                return null;
            }
        }

        public void ReleaseItem(ItemTokenStub token)
        {
            m_itemTokens.ReleaseObject(token);
        }
        #endregion

        public static ItemTokenStub SpawnItemToken(Inventory.BaseItemStub item, GameMapTile tile)
        {
            return instance.SpawnItem(item, tile);
        }

        public static void ReleaseItemToken(ItemTokenStub token)
        {
            instance.ReleaseItem(token);
        }
    }

    [System.Serializable]
    public class NewItemManagerPackage
    {
        public ItemTokenStub itemTokenPrefab = null;
        public int itemTokenPoolCapacity = 5;
    }
}
