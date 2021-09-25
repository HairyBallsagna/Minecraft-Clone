using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class World : MonoBehaviour
{
    public int mapSizeInChunks = 6;
    public int chunkSize = 16;
    public int chunkHeight = 100;
    public int waterThreshold = 50;
    public float noiseScale = 0.03f;
    public GameObject chunkPrefab;

    public bool autoUpdate = false;
    
    private Dictionary<Vector3Int, ChunkData> chunkDatas = new Dictionary<Vector3Int, ChunkData>();
    private Dictionary<Vector3Int, ChunkRenderer> chunks = new Dictionary<Vector3Int, ChunkRenderer>();

    private void GenerateVoxels(ChunkData data)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                float noiseValue = Mathf.PerlinNoise((data.worldPos.x + x) * noiseScale, (data.worldPos.z + z) * noiseScale);
                int groundPos = Mathf.RoundToInt(noiseValue * chunkHeight);

                for (int y = 0; y < chunkHeight; y++)
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
            }
        }
    }

    public void GenerateWorld()
    {
        chunkDatas.Clear();
        foreach (ChunkRenderer chunk in chunks.Values) Destroy(chunk.gameObject);
        chunks.Clear();

        for (int x = 0; x < mapSizeInChunks; x++)
        {
            for (int z = 0; z < mapSizeInChunks; z++)
            {
                ChunkData data = new ChunkData(chunkSize, chunkHeight, this, new Vector3Int(x * chunkSize, 0, z * chunkSize));
                GenerateVoxels(data);
                chunkDatas.Add(data.worldPos, data);
            }
        }

        foreach (ChunkData data in chunkDatas.Values)
        {
            MeshData meshData = Chunk.GetChunkMeshData(data);
            GameObject chunkObj = Instantiate(chunkPrefab, data.worldPos, Quaternion.identity);
            ChunkRenderer chunkRenderer = chunkObj.GetComponent<ChunkRenderer>();
            chunks.Add(data.worldPos, chunkRenderer);
            chunkRenderer.InitializeChunk(data);
            chunkRenderer.UpdateChunk(meshData);
        }
    }

    public VoxelType GetVoxelFromChunkCoords(ChunkData chunkData, int x, int y, int z)
    {
        Vector3Int pos = Chunk.ChunkPositionFromVoxelCoords(this, x, y, z);
        ChunkData containerChunk = null;

        chunkDatas.TryGetValue(pos, out containerChunk);

        if (containerChunk == null) return VoxelType.Nothing;

        Vector3Int voxelInChunkCoords = Chunk.GetVoxelInChunkCoords(containerChunk, new Vector3Int(x, y, z));
        return Chunk.GetVoxelFromChunkCoords(containerChunk, voxelInChunkCoords);
    }
}
