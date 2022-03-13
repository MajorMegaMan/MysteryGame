using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newTileProfile", menuName = "Tile Profile")]
public class TileProfile : ScriptableObject
{
    public Tile.Access access = 0;

    // This needs to be serialised so that the data is carried over to the build. Very weird and there is most likely a better solution.
    [HideInInspector, SerializeField] TileTextureSet m_owner = null;
    public int m_UVIndexX = 0;
    public int m_UVIndexY = 0;

    public TileTextureSet owner { get { return m_owner; } set { m_owner = value; } }

    public Vector2[] UVArray 
    { 
        get 
        {
            Vector4 UVs = m_owner.GetUVs(m_UVIndexX, m_UVIndexY);

            Vector2[] result = new Vector2[4];
            // bottom Left
            result[0].x = UVs.x;
            result[0].y = UVs.y;

            // Top Left
            result[1].x = UVs.x;
            result[1].y = UVs.w;

            // Top Right
            result[2].x = UVs.z;
            result[2].y = UVs.w;

            // Bottom right
            result[3].x = UVs.z;
            result[3].y = UVs.y;

            return result; 
        } 
    }
}
