using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneLayerHandler : VoxelLayerHandler
{
    [Range(0, 1)] public float stoneThreshold = 0.5f;
    [SerializeField] private NoiseData stoneNoiseData;
    public DomainWarping domainWarping;
    
    protected override bool TryHandling(ChunkData data, int x, int y, int z, int surfaceHeightNoise, Vector2Int offset)
    {
        if (data.worldPos.y > surfaceHeightNoise) return false;

        stoneNoiseData.worldOffset = offset;
        //float stoneNoise = Noise.OctavePerlin(data.worldPos.x + x, data.worldPos.z + z, stoneNoiseData);
        float stoneNoise = domainWarping.GenerateDomainNoise(data.worldPos.x + x, data.worldPos.z + z, stoneNoiseData);

        int endPos = surfaceHeightNoise;
        if (data.worldPos.y < 0) endPos = data.worldPos.y + data.chunkHeight;

        if (stoneNoise > stoneThreshold)
        {
            for (int sampleY = data.worldPos.y; sampleY <= endPos; sampleY++)
            {
                Vector3Int pos = new Vector3Int(x, sampleY, z);
                Chunk.SetVoxel(data, pos, VoxelType.Stone);
            }

            return true;
        }

        return false;
    }
}
