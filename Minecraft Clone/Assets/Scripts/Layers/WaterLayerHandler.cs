using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLayerHandler : VoxelLayerHandler
{
    public int waterLevel = 1;
    
    protected override bool TryHandling(ChunkData data, int x, int y, int z, int surfaceHeightNoise, Vector2Int offset)
    {
        if (y > surfaceHeightNoise && y <= waterLevel)
        {
            Vector3Int pos = new Vector3Int(x, y, z);
            Chunk.SetVoxel(data, pos, VoxelType.Water);
            if (y == surfaceHeightNoise + 1)
            {
                pos.y = surfaceHeightNoise + 1;
                Chunk.SetVoxel(data, pos, VoxelType.Sand);
            }
            return true;
        }

        return false;
    }
}
