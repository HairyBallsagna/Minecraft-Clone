using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
    [Range(0, 100)]
    public int waterThreshold = 50;

    public NoiseData biomeNoiseData;

    public VoxelLayerHandler startLayerHandler;
    
    public ChunkData ProcessChunkColumn(ChunkData data, int x, int z, Vector2Int offset)
    {
        biomeNoiseData.worldOffset = offset;
        int surfacePos = GetSurfaceHeightNoise(data.worldPos.x + x, data.worldPos.z + z, data.chunkHeight);
    
        for (int y = 0; y < data.chunkHeight; y++)
        {
            startLayerHandler.Handle(data, x, y, z, surfacePos, offset);
            // VoxelType type = VoxelType.Dirt;
            // if (y > surfacePos)
            // {
            //     if (y < waterThreshold) type = VoxelType.Water;
            //     else type = VoxelType.Air;
            // } 
            // else if (y == surfacePos && y < waterThreshold)
            //     type = VoxelType.Sand;
            // else if (y == surfacePos)
            //     type = VoxelType.Grass_Dirt;
            //
            // Chunk.SetVoxel(data, new Vector3Int(x, y, z), type);
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
