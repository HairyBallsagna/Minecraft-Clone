using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
    [Range(0, 100)]
    public int waterThreshold = 50;

    public NoiseData biomeNoiseData;

    public DomainWarping domainWarping;

    public bool useDomainWarping = false;

    public VoxelLayerHandler startLayer;
    public List<VoxelLayerHandler> additionalLayers;
    
    public ChunkData ProcessChunkColumn(ChunkData data, int x, int z, Vector2Int offset)
    {
        biomeNoiseData.worldOffset = offset;
        int surfacePos = GetSurfaceHeightNoise(data.worldPos.x + x, data.worldPos.z + z, data.chunkHeight);
    
        for (int y = 0; y < data.chunkHeight; y++)
        {
            startLayer.Handle(data, x, y, z, surfacePos, offset);
        }

        foreach (var layer in additionalLayers)
        {
            layer.Handle(data, x, data.worldPos.y, z, surfacePos, offset);
        }
        return data;
    }

    private int GetSurfaceHeightNoise(int x, int z, int chunkHeight)
    {
        float terrainHeight;
        
        if (useDomainWarping)
            terrainHeight = domainWarping.GenerateDomainNoise(x, z, biomeNoiseData);
        else 
            terrainHeight = Noise.OctavePerlin(x, z, biomeNoiseData);
        
        terrainHeight = Noise.Redistribution(terrainHeight, biomeNoiseData);
        int surfaceHeight = Noise.RemapValue01Int(terrainHeight, 0, chunkHeight);
        return surfaceHeight;
    }
}
