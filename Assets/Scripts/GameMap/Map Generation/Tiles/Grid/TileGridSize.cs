using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Grid sizes are used throught most of the grid classes. Therefore this class contains the relevant data while being a reference type
[System.Serializable]
public class TileGridSize
{
    [SerializeField] int m_width = 20;
    [SerializeField] int m_height = 20;
    [SerializeField] float m_tileSize = 1;

    // getters
    public int width { get { return m_width; } }
    public int height { get { return m_height; } }
    public float tileSize { get { return m_tileSize; } }

    public float worldWidth { get { return m_width * tileSize; } }
    public float worldHeight { get { return m_height * tileSize; } }

    // this is for editor purposes
    public void Validate()
    {
        if(m_width < 1)
        {
            m_width = 1;
        }
        if (m_height < 1)
        {
            m_height = 1;
        }
        if (m_tileSize < 0.0001f)
        {
            m_tileSize = 0.0001f;
        }
    }
}
