using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTileTextureSet", menuName = "TileTextureSet")]
public class TileTextureSet : ScriptableObject
{
    [SerializeField] Texture2D m_texture = null;
    [SerializeField] int texturePadding = 4;
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
        Vector2 texelSize = CalcTexelSize();

        Vector4 UVs = Vector4.zero;
        UVs.x = x * (1.0f / m_textureTileWidth) + texelSize.x * (x * 2 + 1);
        UVs.y = y * (1.0f / m_textureTileHeight) + texelSize.y * (y * 2 + 1);
        UVs.z = (x + 1) * (1.0f / m_textureTileWidth) - texelSize.x;
        UVs.w = (y + 1) * (1.0f / m_textureTileHeight) - texelSize.y;

        return UVs;
    }

    Vector2 CalcTexelSize()
    {
        int width = m_texture.width + texturePadding * 2 * m_textureTileWidth;
        int height = m_texture.height + texturePadding * 2 * m_textureTileHeight;

        Vector2 result = Vector2.one;
        result.x /= width;
        result.y /= height;

        return result;
    }

    public Texture2D PadTileTexture()
    {
        int width = m_texture.width + texturePadding * 2 * m_textureTileWidth;
        int height = m_texture.height + texturePadding * 2 * m_textureTileHeight;

        int tilePixelWidth = m_texture.width / m_textureTileWidth;
        int tilePixelHeight = m_texture.height / m_textureTileHeight;

        Texture2D paddedTexture = new Texture2D(width, height, TextureFormat.RGBA32, true);
        paddedTexture.anisoLevel = m_texture.anisoLevel;
        for(int i = 0; i < m_textureTileWidth; i++)
        {
            for (int j = 0; j < m_textureTileHeight; j++)
            {
                int startX = tilePixelWidth * i;
                int endX = tilePixelWidth * (i + 1);

                int startY = tilePixelHeight * j;
                int endY = tilePixelHeight * (j + 1);

                int paddingX = texturePadding * (i * 2 + 1);
                int paddingY = texturePadding * (j * 2 + 1);

                // Set colour of texture
                for (int x = startX; x < endX; x++)
                {
                    for (int y = startY; y < endY; y++)
                    {
                        Color colour = m_texture.GetPixel(x, y);
                        paddedTexture.SetPixel(x + paddingX, y + paddingY, colour);
                    }
                }

                // Set colour of padding
                for (int y = startY; y < endY; y++)
                {
                    for (int x = startX + paddingX - texturePadding; x < startX + paddingX; x++)
                    {
                        Color colour = m_texture.GetPixel(startX, y);
                        paddedTexture.SetPixel(x, y + paddingY, colour);
                        colour = m_texture.GetPixel(endX - 1, y);
                        paddedTexture.SetPixel(x + tilePixelWidth + texturePadding, y + paddingY, colour);
                    }
                }
                
                for (int x = startX; x < endX; x++)
                {
                    for (int y = startY + paddingY - texturePadding; y < startY + paddingY; y++)
                    {
                        Color colour = m_texture.GetPixel(x, startY);
                        paddedTexture.SetPixel(x + paddingX, y, colour);
                        colour = m_texture.GetPixel(x, endY - 1);
                        paddedTexture.SetPixel(x + paddingX, y + tilePixelHeight + texturePadding, colour);
                    }
                }

                // Set corners of padding
                int cornerStartX = startX + paddingX - texturePadding;
                int cornerStartY = startY + paddingY - texturePadding;

                Color cornerColour = m_texture.GetPixel(startX, startY);
                SetCornerPixels(paddedTexture, cornerStartX, cornerStartY, cornerColour);

                cornerColour = m_texture.GetPixel(endX - 1, startY);
                SetCornerPixels(paddedTexture, cornerStartX + tilePixelWidth + texturePadding, cornerStartY, cornerColour);

                cornerColour = m_texture.GetPixel(startX, endY - 1);
                SetCornerPixels(paddedTexture, cornerStartX, cornerStartY + tilePixelHeight + texturePadding, cornerColour);

                cornerColour = m_texture.GetPixel(endX - 1, endY - 1);
                SetCornerPixels(paddedTexture, cornerStartX + tilePixelWidth + texturePadding, cornerStartY + tilePixelHeight + texturePadding, cornerColour);
            }
        }

        paddedTexture.Apply();
        return paddedTexture;
    }

    void SetCornerPixels(Texture2D texture, int startX, int startY, Color colour)
    {
        SetPixels(texture, startX, startY, texturePadding, texturePadding, colour);
    }

    void SetPixels(Texture2D texture, int startX, int startY, int width, int height, Color colour)
    {
        for (int x = startX; x < startX + width; x++)
        {
            for (int y = startY; y < startY + height; y++)
            {
                texture.SetPixel(x, y, colour);
            }
        }
    }

    public Material CreateMaterial()
    {
        Material result = new Material(m_material);
        result.mainTexture = PadTileTexture();
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
