using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DelaunayExtensions
{
    public static Vector3 GetVector3Point(this DelaunayTriangle tri, int pointIndex)
    {
        Vector3 position = Vector3.zero;
        Vector2 point = tri.points[pointIndex].position;
        position.x = point.x;
        position.z = point.y;
        return position;
    }
}
