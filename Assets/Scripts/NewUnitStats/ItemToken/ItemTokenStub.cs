using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NewUnitStats.Inventory;

namespace NewUnitStats
{
    public class ItemTokenStub : GameMapToken, IPooledObject
    {
        [SerializeField] MeshFilter m_meshFilter = null;
        [SerializeField] MeshRenderer m_meshRenderer = null;

        BaseItemStub m_item = null;

        public BaseItemStub item { get { return m_item; } }

        public override int GetTokenID()
        {
            return (int)TempTokenID.item;
        }

        public void SetMesh(Mesh mesh, Material[] materials)
        {
            m_meshFilter.mesh = mesh;
            m_meshRenderer.materials = materials;
        }

        public void SetItem(BaseItemStub item)
        {
            m_item = item;
            SetMesh(item.GetTokenMesh(), item.GetTokenMaterials());
        }

        void IPooledObject.SetIsActiveInPool(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public void Release(GameObjectPool<ItemTokenStub> itemTokenPool)
        {
            itemTokenPool.ReleaseObject(this);
        }
    }
}
