using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelaunayPoint
{
    int m_index = -1;
    Vector2 m_position = Vector2.zero;

    // getters
    public int index { get { return m_index; } }
    public Vector2 position { get { return m_position; } }

    public DelaunayPoint(int index, Vector2 position)
    {
        m_index = index;
        m_position = position;
    }
}
