using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRoomProfile", menuName = "RoomProfile")]
public class RoomProfile : ScriptableObject
{
    public int minRoomWidth = 1;
    public int maxRoomWidth = 10;
    public int minRoomHeight = 1;
    public int maxRoomHeight = 10;

    public int GetRandomWidth()
    {
        return Random.Range(minRoomWidth, maxRoomWidth);
    }

    public int GetRandomHeight()
    {
        return Random.Range(minRoomHeight, maxRoomHeight);
    }
}
