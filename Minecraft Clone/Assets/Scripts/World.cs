using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

public class World : MonoBehaviour
{
    public int mapSizeInChunks = 6;
    public int chunkSize = 16;
    public int chunkHeight = 100;
    public Vector2Int offset;
    
    public GameObject chunkPrefab;
    public TerrainGenerator terrainGenerator;

    public bool autoUpdate = false;

    public UnityEvent OnWorldCreated;
    public UnityEvent OnNewChunksGenerated;

    [HideInInspector]
    public bool biomeSettingsFoldout;
    [HideInInspector]
    public bool noiseSettingsFoldout;
    
    // private Dictionary<Vector3Int, ChunkData> chunkDatas = new Dictionary<Vector3Int, ChunkData>();
    // private Dictionary<Vector3Int, ChunkRenderer> chunks = new Dictionary<Vector3Int, ChunkRenderer>();

    public WorldData worldData { get; private set; }
    
    private void Awake()
    {
        worldData = new WorldData
        {
            chunkHeight = this.chunkHeight,
            chunkSize = this.chunkSize,
            chunkDatas = new Dictionary<Vector3Int, ChunkData>(),
            chunks = new Dictionary<Vector3Int, ChunkRenderer>()
        };
    }

    public void GenerateWorld()
    {
        FindObjectOfType<VoxelDataManager>().Initialize();
        // chunkDatas.Clear();
        // foreach (ChunkRenderer chunk in chunks.Values)
        // {
        //     if (chunk != null) { SafeDestroy(chunk.gameObject); }
        // }
        //
        // if (transform.childCount > 0)
        // {
        //     foreach (Transform child in transform)
        //     {
        //         SafeDestroy(child.gameObject);
        //     }
        // }
        // chunks.Clear();

        worldData = new WorldData
        {
            chunkHeight = this.chunkHeight,
            chunkSize = this.chunkSize,
            chunkDatas = new Dictionary<Vector3Int, ChunkData>(),
            chunks = new Dictionary<Vector3Int, ChunkRenderer>()
        };
        
        WorldGenerationData worldGenerationData = GetPositionsWithinCamsFrustumPlane(Vector3Int.zero);
        for (int x = 0; x < mapSizeInChunks; x++)
        {
            for (int z = 0; z < mapSizeInChunks; z++)
            {
                ChunkData data = new ChunkData(chunkSize, chunkHeight, this, new Vector3Int(x * chunkSize, 0, z * chunkSize));
                //GenerateVoxels(data);
                ChunkData newData = terrainGenerator.GenerateChunkData(data, offset);
                worldData.chunkDatas.Add(newData.worldPos, newData);
            }
        }

        foreach (ChunkData data in worldData.chunkDatas.Values)
        {
            MeshData meshData = Chunk.GetChunkMeshData(data);
            GameObject chunkObj = Instantiate(chunkPrefab, data.worldPos, Quaternion.identity);
            chunkObj.transform.SetParent(this.transform);
            ChunkRenderer chunkRenderer = chunkObj.GetComponent<ChunkRenderer>();
            worldData.chunks.Add(data.worldPos, chunkRenderer);
            chunkRenderer.InitializeChunk(data);
            chunkRenderer.UpdateChunk(meshData);
        }
        OnWorldCreated?.Invoke();
    }

    private WorldGenerationData GetPositionsWithinCamsFrustumPlane(Vector3Int playerPos)
    {
        List<Vector3Int> allChunkPositionsNeeded = WorldDataHelper.GetChunkPositionsAroundPlayer(this, playerPos);
        List<Vector3Int> allChunkDataPositionsNeeded = WorldDataHelper.GetChunkPositionsAroundPlayer(this, playerPos);

        List<Vector3Int> chunkPositionsToCreate = 
            WorldDataHelper.SelectPositionsToCreate(worldData, allChunkPositionsNeeded, playerPos);
        List<Vector3Int> chunkDataPositionsToCreate = 
            WorldDataHelper.SelectDataPositionsToCreate(worldData, allChunkDataPositionsNeeded, playerPos);

        WorldGenerationData data = new WorldGenerationData
        {
            chunkPositionsToCreate = chunkPositionsToCreate,
            chunkDataPositionsToCreate = chunkDataPositionsToCreate,
            chunkPositionsToRemove = new List<Vector3Int>(),
            chunkDataToRemove = new List<Vector3Int>()
        };

        return data;
    }

    public void DestroyChunks()
    {
        // foreach (Transform child in transform) SafeDestroy(child.gameObject);
        //
        // foreach (ChunkRenderer chunk in chunks.Values)
        // {
        //     if (chunk != null) {SafeDestroy(chunk.gameObject);}
        // }
        //
        // chunks.Clear();
    }

    public VoxelType GetVoxelFromChunkCoords(ChunkData chunkData, int x, int y, int z)
    {
        Vector3Int pos = Chunk.ChunkPositionFromVoxelCoords(this, x, y, z);
        ChunkData containerChunk = null;

        worldData.chunkDatas.TryGetValue(pos, out containerChunk);

        if (containerChunk == null) return VoxelType.Nothing;

        Vector3Int voxelInChunkCoords = Chunk.GetVoxelInChunkCoords(containerChunk, new Vector3Int(x, y, z));
        return Chunk.GetVoxelFromChunkCoords(containerChunk, voxelInChunkCoords);
    }

    public void LoadAdditionalChunksRequest(GameObject player)
    {
        Debug.Log("load more chunks asshole");
        OnNewChunksGenerated?.Invoke();
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

    public struct WorldGenerationData
    {
        public List<Vector3Int> chunkPositionsToCreate;
        public List<Vector3Int> chunkDataPositionsToCreate;
        public List<Vector3Int> chunkPositionsToRemove;
        public List<Vector3Int> chunkDataToRemove;
    }

    public struct WorldData
    {
        public Dictionary<Vector3Int, ChunkData> chunkDatas;
        public Dictionary<Vector3Int, ChunkRenderer> chunks;
        public int chunkSize;
        public int chunkHeight;
    }
}
