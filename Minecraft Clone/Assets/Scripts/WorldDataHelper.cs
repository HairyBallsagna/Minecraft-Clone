using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldDataHelper
{
    public static Vector3Int ChunkPositionFromBlockCoords(World world, Vector3Int pos)
    {
        return new Vector3Int
        {
            x = Mathf.FloorToInt(pos.x / (float) world.chunkSize) * world.chunkSize,
            y = Mathf.FloorToInt(pos.y / (float) world.chunkHeight) * world.chunkHeight,
            z = Mathf.FloorToInt(pos.z / (float) world.chunkSize) * world.chunkSize
        };
    }

    public static List<Vector3Int> SelectPositionsToCreate(World.WorldData worldData, List<Vector3Int> allChunkPositionsNeeded, Vector3Int playerPos)
    {
        throw new System.NotImplementedException();
    }

    public static List<Vector3Int> SelectDataPositionsToCreate(World.WorldData worldData, List<Vector3Int> allChunkDataPositionsNeeded, Vector3Int playerPos)
    {
        throw new System.NotImplementedException();
    }

    public static List<Vector3Int> GetChunkPositionsAroundPlayer(World world, Vector3Int playerPos)
    {
        throw new System.NotImplementedException();
    }
}
