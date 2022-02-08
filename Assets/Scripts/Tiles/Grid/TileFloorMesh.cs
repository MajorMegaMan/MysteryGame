using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFloorMesh
{
    TileGridSize m_tileGridSize = null;

    Mesh m_mesh;
    Vector2[] m_uvArray;

    public int width { get { return m_tileGridSize.width; } }
    public int height { get { return m_tileGridSize.height; } }
    public float tileSize { get { return m_tileGridSize.tileSize; } }

    public TileFloorMesh(TileGridSize tileGridSize)
    {
        m_tileGridSize = tileGridSize;
    }

    public Mesh CreateTileMesh()
    {
        return CreateTileMesh(Vector3.zero);
    }

    public Mesh CreateTileMesh(Vector3 origin)
    {
        m_mesh = new Mesh();
        m_mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        CreateEmptyMeshArrays(out Vector3[] vertices, out int[] triangles, width, height);

        SetVertices(vertices, origin);

        // Initialise tile uvs with generics uvs. These can be changed by the user later
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int index = i * height + j;

                m_uvArray[index * 4 + 0] = new Vector2(0.0f, 0.0f);
                m_uvArray[index * 4 + 1] = new Vector2(0.0f, 1.0f);
                m_uvArray[index * 4 + 2] = new Vector2(1.0f, 1.0f);
                m_uvArray[index * 4 + 3] = new Vector2(1.0f, 0.0f);
            }
        }

        SetTriangles(triangles);

        m_mesh.vertices = vertices;
        m_mesh.uv = m_uvArray;
        m_mesh.triangles = triangles;
        m_mesh.RecalculateNormals();
        m_mesh.RecalculateTangents();
        m_mesh.RecalculateBounds();
        return m_mesh;
    }

    // Sets uvs for a specifc tile
    // it is expected that the uvs parameter will have 4 elements in it
    public void SetTileUVs(Vector2[] UVs, int x, int y)
    {
        int index = x * height + y;

        for (int i = 0; i < 4; i++)
        {
            m_uvArray[index * 4 + i] = UVs[i];
        }
    }

    public void UpdateUVs()
    {
        m_mesh.uv = m_uvArray;
    }

    void CreateEmptyMeshArrays(out Vector3[] vertices, out int[] triangles, int width, int height)
    {
        int tileArea = width * height;

        vertices = new Vector3[4 * tileArea];
        m_uvArray = new Vector2[4 * tileArea];
        triangles = new int[6 * tileArea];
    }

    // Creates quads for each tile
    void SetVertices(Vector3[] vertices, Vector3 origin)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int index = i * height + j;

                vertices[index * 4 + 0] = new Vector3(tileSize * i, 0.0f, tileSize * j) + origin;
                vertices[index * 4 + 1] = new Vector3(tileSize * i, 0.0f, tileSize * (j + 1)) + origin;
                vertices[index * 4 + 2] = new Vector3(tileSize * (i + 1), 0.0f, tileSize * (j + 1)) + origin;
                vertices[index * 4 + 3] = new Vector3(tileSize * (i + 1), 0.0f, tileSize * j) + origin;
            }
        }
    }

    // Creates quads for each tiles
    void SetTriangles(int[] triangles)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int index = i * height + j;

                triangles[index * 6 + 0] = index * 4 + 0;
                triangles[index * 6 + 1] = index * 4 + 1;
                triangles[index * 6 + 2] = index * 4 + 2;

                triangles[index * 6 + 3] = index * 4 + 0;
                triangles[index * 6 + 4] = index * 4 + 2;
                triangles[index * 6 + 5] = index * 4 + 3;
            }
        }
    }
}
