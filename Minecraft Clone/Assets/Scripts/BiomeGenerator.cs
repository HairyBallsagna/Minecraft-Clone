using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
    [Range(0, 100)]
    public int waterThreshold = 50;

    public NoiseData biomeNoiseData;
    
    public ChunkData ProcessChunkColumn(ChunkData data, int x, int z, Vector2Int offset)
    {
        biomeNoiseData.worldOffset = offset;
        int groundPos = GetSurfaceHeightNoise(data.worldPos.x + x, data.worldPos.z + z, data.chunkHeight);
    
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

    private int GetSurfaceHeightNoise(int x, int z, int chunkHeight)
    {
        float terrainHeight = Noise.OctavePerlin(x, z, biomeNoiseData);
        terrainHeight = Noise.Redistribution(terrainHeight, biomeNoiseData);
        int surfaceHeight = Noise.RemapValue01Int(terrainHeight, 0, chunkHeight);
        return surfaceHeight;
    }
}
