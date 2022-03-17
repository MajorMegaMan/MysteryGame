using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelaunayMap
{
    DelaunayPoint[] m_points;
    DelaunayTriangle[] m_triangles;

    // getters
    public DelaunayPoint[] points { get { return m_points; } }
    public DelaunayTriangle[] triangles { get { return m_triangles; } }

    public void Initialise(Vector2[] points)
    {
        m_points = new DelaunayPoint[points.Length];

        if (points.Length < 3)
        {
            // not enough points to create at least one triangle
            for (int i = 0; i < points.Length; i++)
            {
                m_points[i] = new DelaunayPoint(i, points[i]);
            }
            m_triangles = null;
            return;
        }

        // Can create a del map
        DelaunayTriangulationMap tempMap = new DelaunayTriangulationMap();
        tempMap.Initialise(points);

        // Get Point info as the order of points has most likely changed
        for (int i = 0; i < points.Length; i++)
        {
            m_points[i] = new DelaunayPoint(i, tempMap.GetPoint(i));
        }

        // Create Triangles array
        int triangleCount = tempMap.GetTriCount();
        m_triangles = new DelaunayTriangle[triangleCount];

        for(int i = 0; i < triangleCount; i++)
        {
            m_triangles[i] = new DelaunayTriangle(i);
        }

        // Get Triangles info
        foreach(DelaunayTriangle triangle in m_triangles)
        {
            triangle.Initialise(m_points, m_triangles, tempMap);
        }
    }

    public void Clear()
    {
        m_points = null;
        m_triangles = null;
    }

    public int GetPointsSize()
    {
        if (m_points == null)
        {
            return 0;
        }
        return m_points.Length;
    }

    public DelaunayPoint GetPoint(int index)
    {
        return m_points[index];
    }

    public int GetTrianglesSize()
    {
        if(m_triangles == null)
        {
            return 0;
        }
        return m_triangles.Length;
    }

    public DelaunayTriangle GetTriangle(int index)
    {
        return m_triangles[index];
    }
}
