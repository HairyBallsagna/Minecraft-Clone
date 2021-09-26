using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceLayerHandler : VoxelLayerHandler
{
    public VoxelType surfaceVoxelType;
    
    protected override bool TryHandling(ChunkData data, int x, int y, int z, int surfaceHeightNoise, Vector2Int offset)
    {
        if (y == surfaceHeightNoise)
        {
            Vector3Int pos = new Vector3Int(x, y, z);
            Chunk.SetVoxel(data, pos, surfaceVoxelType);
            return true;
        }

        return false;
    }
}
