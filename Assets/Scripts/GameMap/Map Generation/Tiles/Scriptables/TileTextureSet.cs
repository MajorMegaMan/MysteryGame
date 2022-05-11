using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTileTextureSet", menuName = "TileTextureSet")]
public class TileTextureSet : ScriptableObject
{
    [SerializeField] Texture2D m_texture = null;
    [SerializeField] Material m_material = null;

    [SerializeField] int m_textureTileWidth = 1;
    [SerializeField] int m_textureTileHeight = 1;

    [SerializeField] TileProfile m_initialTileProfile = null;
    [SerializeField] TileProfile m_openTileProfile = null;

    [SerializeField] TileProfile[] m_otherTileProfiles = null;

    // getters
    public TileProfile initialTileProfile { get { return m_initialTileProfile; } }
    public TileProfile openTileProfile { get { return m_openTileProfile; } }
    public TileProfile[] otherTileProfiles { get { return m_otherTileProfiles; } }


    // First 2 elements are min UVs, the last 2 are max UVs
    // x = U0, y = V0, z = U1, w = V1
    public Vector4 GetUVs(int x, int y)
    {
        Vector4 UVs = Vector4.zero;
        UVs.x = x * (1.0f / m_textureTileWidth);
        UVs.y = y * (1.0f / m_textureTileHeight);
        UVs.z = (x + 1) * (1.0f / m_textureTileWidth);
        UVs.w = (y + 1) * (1.0f / m_textureTileHeight);

        return UVs;
    }

    public Material CreateMaterial()
    {
        Material result = new Material(m_material);
        result.mainTexture = m_texture;
        result.SetVector("_TileCount", new Vector2(m_textureTileWidth, m_textureTileHeight));
        return result;
    }

    private void OnValidate()
    {
        ValidateTilingValue(ref m_textureTileWidth);
        ValidateTilingValue(ref m_textureTileHeight);
        SetProfileOwner(initialTileProfile);
        SetProfileOwner(m_openTileProfile);
        for(int i = 0; i < m_otherTileProfiles.Length; i++)
        {
            SetProfileOwner(m_otherTileProfiles[i]);
        }
    }

    void ValidateTilingValue(ref int integer)
    {
        if (integer < 1)
        {
            integer = 1;
        }
    }

    void SetProfileOwner(TileProfile tileProfile)
    {
        if (tileProfile != null)
        {
            tileProfile.owner = this;
        }
    }
}
