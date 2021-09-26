using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirLayerHandler : VoxelLayerHandler
{
    protected override bool TryHandling(ChunkData data, int x, int y, int z, int surfaceHeightNoise, Vector2Int offset)
    {
        if (y > surfaceHeightNoise)
        {
            Vector3Int pos = new Vector3Int(x, y, z);
            Chunk.SetVoxel(data, pos, VoxelType.Air);
            return true;
        }

        return false;
    }
}
