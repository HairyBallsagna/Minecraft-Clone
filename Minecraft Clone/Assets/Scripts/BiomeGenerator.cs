using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
    [Range(0, 100)]
    public int waterThreshold = 50;
    [Range(0, 1)]
    public float noiseScale = 0.03f;
    
    public ChunkData ProcessChunkColumn(ChunkData data, int x, int z, Vector2Int offset)
    {
        float noiseValue = Mathf.PerlinNoise
            ((offset.x + data.worldPos.x + x) * (noiseScale / 10), (offset.y + data.worldPos.z + z) * (noiseScale / 10));
        int groundPos = Mathf.RoundToInt(noiseValue * data.chunkHeight);
    
        for (int y = 0; y < data.chunkHeight; y++)
        {
            VoxelType type = VoxelType.Dirt;
            if (y > groundPos)
            {
                if (y < waterThreshold) type = VoxelType.Water;
                else type = VoxelType.Air;
            } 
            else if (y == groundPos) type = VoxelType.Grass_Dirt;
                    
            Chunk.SetVoxel(data, new Vector3Int(x, y, z), type);
        }

        return data;
    }
}
