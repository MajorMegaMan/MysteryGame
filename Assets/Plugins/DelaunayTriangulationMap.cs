using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DelaunayTriangulationMap
{
    public DelaunayTriangulationMap()
    {
        m_objectHandle = Create();
    }

    ~DelaunayTriangulationMap()
    {
        Destroy(m_objectHandle);
    }

    public void Initialise(Vector2[] points)
    {
        AllocatePointArray(m_objectHandle, points.Length);
        for(int i = 0; i < points.Length; i++)
        {
            SetPoint(m_objectHandle, i, points[i].x, points[i].y);
        }
        Triangulate(m_objectHandle);
    }

    public void Clear()
    {
        Clear(m_objectHandle);
    }

    public int GetPointCount()
    {
        return GetPointCount(m_objectHandle);
    }

    public Vector2 GetPoint(int pointIndex)
    {
        Vector2 point = Vector2.zero;
        point.x = GetPointValue(m_objectHandle, pointIndex, 0);
        point.y = GetPointValue(m_objectHandle, pointIndex, 1);
        return point;
    }

    public int GetTriCount()
    {
        return GetTriCount(m_objectHandle);
    }

    public int GetTriPointIndex(int triangleIndex, int verticeIndex)
    {
        return GetVerticePointIndex(m_objectHandle, triangleIndex, verticeIndex);
    }

    public int GetTriNeighbourIndex(int triangleIndex, int neighbourPosition)
    {
        return GetNeighbourIndex(m_objectHandle, triangleIndex, neighbourPosition);
    }

    // Name of the dll
    const string _dllName = "DelaunayTriangulation";

    // object handle to Delmap in dll
    IntPtr m_objectHandle = IntPtr.Zero;

    // Creates the DelaunayMap object
    [DllImport(_dllName, EntryPoint = "Export_DelaunayMap_Create")]
    private static extern IntPtr Create();

    // Destroys the DelaunayMap object
    [DllImport(_dllName, EntryPoint = "Export_DelaunayMap_Destroy")]
    private static extern void Destroy(IntPtr objectHandle);

    // Internally allocates a Vector2 array of size pointCount
    // This creates an empty array of vector2 points the user can then define
    [DllImport(_dllName, EntryPoint = "Export_DelaunayMap_AllocatePointArray")]
    private static extern void AllocatePointArray(IntPtr objectHandle, int pointCount);

    // Set the values of a specified point
    [DllImport(_dllName, EntryPoint = "Export_DelaunayMap_SetPoint")]
    private static extern void SetPoint(IntPtr objectHandle, int pointIndex, float x, float y);

    // Creates Delaunay Triangles
    [DllImport(_dllName, EntryPoint = "Export_DelaunayMap_Triangulate")]
    private static extern void Triangulate(IntPtr objectHandle);

    // Clears points and triangles
    [DllImport(_dllName, EntryPoint = "Export_DelaunayMap_Clear")]
    private static extern void Clear(IntPtr objectHandle);

    // Gets the number of Vector2 point elements
    [DllImport(_dllName, EntryPoint = "Export_DelaunayMap_GetPointCount")]
    private static extern int GetPointCount(IntPtr objectHandle);

    // Gets the value of a Vector2 point
    // valuePosition is the x or y component
    // x = 0, y = 1
    [DllImport(_dllName, EntryPoint = "Export_DelaunayMap_GetPointValue")]
    private static extern float GetPointValue(IntPtr objectHandle, int pointIndex, int valuePosition);

    // Gets the number of triangles that were created
    [DllImport(_dllName, EntryPoint = "Export_DelaunayMap_GetTriCount")]
    private static extern int GetTriCount(IntPtr objectHandle);

    // Gets the index of a Vector2 point from a triangles vertice.
    // Vertice position is the index of the triangles points array.
    // obviously, there are 3 verts.
    [DllImport(_dllName, EntryPoint = "Export_DelaunayMap_GetVerticePointIndex")]
    private static extern int GetVerticePointIndex(IntPtr objectHandle, int triangleIndex, int verticePosition);

    // Gets the index of a neighbouring triangle from a triangles neighbour array.
    // Triangles can have a total of 3 neighbours, one for each face.
    // Returns -1 if there was no neighbour in that neighbour index.
    [DllImport(_dllName, EntryPoint = "Export_DelaunayMap_GetNeighbourIndex")]
    private static extern int GetNeighbourIndex(IntPtr objectHandle, int triangleIndex, int neighbourPosition);
}
