using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager
{
    ItemLibrary m_itemLibrary = null;
    GameMap m_gameMap = null;
    GameObjectPool<ItemToken> m_tokenPool = null;

    Transform m_tokenContainer = null;

    [System.Serializable]
    public class Package
    {
        public ItemLibrary itemLibrary = null;
        public ItemToken itemTokenPrefab = null;
        public int poolCount = 20;
        public Transform container = null;
    }

    public ItemManager(Package itemPackage, GameMap gameMap)
    {
        Init(itemPackage, gameMap);
    }

    void Init(Package itemPackage, GameMap gameMap)
    {
        m_gameMap = gameMap;
        SetItemLibrary(itemPackage.itemLibrary);
        m_tokenPool = new GameObjectPool<ItemToken>(itemPackage.itemTokenPrefab, itemPackage.poolCount, InstantiateItemToken, DestroyItemToken);
        m_tokenContainer = itemPackage.container;
    }

    public void SetItemLibrary(ItemLibrary itemLibrary)
    {
        m_itemLibrary = itemLibrary;
    }

    public void AddItemToInventory(Inventory<Unit> inventory, TempItemID tempItemID)
    {
        inventory.AddItem(m_itemLibrary.GetConsumableItem(tempItemID));
    }

    #region ItemTokens
    ItemToken InstantiateItemToken(ItemToken prefab)
    {
        ItemToken newItemToken = Object.Instantiate(prefab, m_tokenContainer);
        return newItemToken;
    }

    void DestroyItemToken(ItemToken itemToken)
    {
        Object.Destroy(itemToken);
    }

    public ItemToken GetItemToken(TempItemID itemID)
    {
        IInventoryItem<Unit> itemType = m_itemLibrary.GetConsumableItem(itemID);
        ItemToken itemToken = m_tokenPool.ActivateObject();
        if (itemToken != null)
        {
            itemToken.SetAsItem(itemType, itemType.GetMesh(), itemType.GetMaterials());
            return itemToken;
        }
        else
        {
            // all pooled objects are currently in use
            return null;
        }
    }
    #endregion
}
