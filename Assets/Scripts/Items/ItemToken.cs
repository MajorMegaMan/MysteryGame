using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemToken : GameMapToken, IPooledObject
{
    IInventoryItem<Unit> m_item = null;
    [SerializeField] MeshFilter m_meshFilter = null;
    [SerializeField] MeshRenderer m_meshRenderer = null;

    public IInventoryItem<Unit> item { get { return m_item; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override int GetID()
    {
        return (int)TempTokenID.item;
    }

    public void SetIsActiveInPool(bool isActive)
    {
        gameObject.SetActive(isActive);
        if(!isActive)
        {
            TokenManager.ClearToken(this);
        }
    }

    public void SetAsItem(IInventoryItem<Unit> item, Mesh mesh, Material[] materials)
    {
        m_item = item;
        m_meshFilter.mesh = mesh;
        m_meshRenderer.materials = materials;
    }
}
