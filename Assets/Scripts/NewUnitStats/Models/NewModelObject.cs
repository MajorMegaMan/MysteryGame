using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewUnitStats
{
    public class NewModelObject : MonoBehaviour, IPooledObject
    {
        [SerializeField] SkinnedMeshRenderer m_renderer = null;

        public void SetMaterials(Material[] materials)
        {
            m_renderer.materials = materials;
        }

        public void SetMesh(Mesh mesh)
        {
            m_renderer.sharedMesh = mesh;
        }

        void IPooledObject.SetIsActiveInPool(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}
