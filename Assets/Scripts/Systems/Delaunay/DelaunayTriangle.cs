using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelaunayTriangle
{
    int m_index = -1;
    DelaunayPoint[] m_points = new DelaunayPoint[3];
    DelaunayTriangle[] m_neighbours = new DelaunayTriangle[3];

    // getters
    public int index { get { return m_index; } }
    public DelaunayPoint[] points { get { return m_points; } }
    public DelaunayTriangle[] neighbours { get { return m_neighbours; } }

    public DelaunayTriangle(int index)
    {
        m_index = index;
    }

    public void SetPointsDebug(DelaunayPoint[] delaunayPoints)
    {
        m_points = delaunayPoints;
    }

    public void Initialise(DelaunayPoint[] delaunayPoints, DelaunayTriangle[] delaunayTriangles, DelaunayTriangulationMap triangulationMap)
    {
        for (int i = 0; i < 3; i++)
        {
            m_points[i] = delaunayPoints[triangulationMap.GetTriPointIndex(m_index, i)];
        }

        for (int i = 0; i < 3; i++)
        {
            int neighbourIndex = triangulationMap.GetTriNeighbourIndex(m_index, i);
            if(neighbourIndex != -1)
            {
                m_neighbours[i] = delaunayTriangles[triangulationMap.GetTriNeighbourIndex(m_index, i)];
            }
            else
            {
                m_neighbours[i] = null;
            }
        }
    }

    public Vector2 CalcCircumcentre()
    {
        Vector2 a = points[0].position;
        Vector2 b = points[1].position;
        Vector2 c = points[2].position;

        Vector2 aTob = b - a;
        Vector2 aToc = c - a;

        Vector2 dir1 = aTob / 2.0f;
        Vector2 dir2 = aToc / 2.0f;

        Vector2 point1 = a + dir1;
        Vector2 point2 = a + dir2;

        dir1 = GetPerpindicular(dir1);
        dir2 = GetPerpindicular(dir2);

        return LineIntersection(point1, dir1, point2, dir2);
    }

    public Vector2 GetPerpindicular(Vector2 vector)
    {
        Vector2 result = vector;
        result.x = vector.y;
        result.y = -vector.x;
        return result;
    }

    public Vector2 LineIntersection(Vector2 p1, Vector2 d1, Vector2 p2, Vector2 d2)
    {
        float t2 = (d1.x * p2.y - d1.x * p1.y - d1.y * p2.x + d1.y * p1.x) / (d1.y * d2.x - d1.x * d2.y);
        return p2 + (d2 * t2);
    }

    static float sign(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }

    static bool PointIsInTriangle(Vector2 point, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        float d1, d2, d3;
        bool has_neg, has_pos;

        d1 = sign(point, v1, v2);
        d2 = sign(point, v2, v3);
        d3 = sign(point, v3, v1);

        has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

        return !(has_neg && has_pos);
    }

    public bool ContainsPoint(Vector2 point)
    {
        return PointIsInTriangle(point, points[0].position, points[1].position, points[2].position);
    }

    public Vector2 FindMeanAverage()
    {
        Vector2 triMean = Vector2.zero;
        triMean += points[0].position;
        triMean += points[1].position;
        triMean += points[2].position;
        triMean /= 3;
        return triMean;
    }
}
