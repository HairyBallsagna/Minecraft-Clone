using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class World : MonoBehaviour
{
    public int mapSizeInChunks = 6;
    public int chunkSize = 16;
    public int chunkHeight = 100;
    public Vector2Int offset;
    
    public GameObject chunkPrefab;
    public TerrainGenerator terrainGenerator;

    public bool autoUpdate = false;

    [HideInInspector]
    public bool biomeSettingsFoldout;
    [HideInInspector]
    public bool noiseSettingsFoldout;
    
    private Dictionary<Vector3Int, ChunkData> chunkDatas = new Dictionary<Vector3Int, ChunkData>();
    private Dictionary<Vector3Int, ChunkRenderer> chunks = new Dictionary<Vector3Int, ChunkRenderer>();

    public void GenerateWorld()
    {
        FindObjectOfType<VoxelDataManager>().Initialize();
        chunkDatas.Clear();
        foreach (ChunkRenderer chunk in chunks.Values)
        {
            if (chunk != null) { SafeDestroy(chunk.gameObject); }
        }
        if (transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                SafeDestroy(child.gameObject);
            }
        }
        chunks.Clear();

        for (int x = 0; x < mapSizeInChunks; x++)
        {
            for (int z = 0; z < mapSizeInChunks; z++)
            {
                ChunkData data = new ChunkData(chunkSize, chunkHeight, this, new Vector3Int(x * chunkSize, 0, z * chunkSize));
                //GenerateVoxels(data);
                ChunkData newData = terrainGenerator.GenerateChunkData(data, offset);
                chunkDatas.Add(newData.worldPos, newData);
            }
        }

        
        foreach (ChunkData data in chunkDatas.Values)
        {
            MeshData meshData = Chunk.GetChunkMeshData(data);
            GameObject chunkObj = Instantiate(chunkPrefab, data.worldPos, Quaternion.identity);
            chunkObj.transform.SetParent(this.transform);
            ChunkRenderer chunkRenderer = chunkObj.GetComponent<ChunkRenderer>();
            chunks.Add(data.worldPos, chunkRenderer);
            chunkRenderer.InitializeChunk(data);
            chunkRenderer.UpdateChunk(meshData);
        }
    }
    
    public void DestroyChunks()
    {
        foreach (Transform child in transform) SafeDestroy(child.gameObject);

        foreach (ChunkRenderer chunk in chunks.Values)
        {
            if (chunk != null) {SafeDestroy(chunk.gameObject);}
        }
        
        chunks.Clear();
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
    
    public static T SafeDestroy<T>(T obj) where T : Object
    {
        if (Application.isEditor)
            Object.DestroyImmediate(obj);
        else
            Object.Destroy(obj);
     
        return null;
    }
    public static T SafeDestroyGameObject<T>(T component) where T : Component
    {
        if (component != null)
            SafeDestroy(component.gameObject);
        return null;
    }

}
