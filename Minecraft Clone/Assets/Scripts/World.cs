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
    public int chunkDrawDistance = 8;
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
        if (!Application.isPlaying)
            FindObjectOfType<VoxelDataManager>().Initialize();
        GenerateWorld(Vector3Int.zero);
    }
    
    private void GenerateWorld(Vector3Int position)
    {
        if (!Application.isPlaying)
        {
            worldData = new WorldData
            {
                chunkHeight = this.chunkHeight,
                chunkSize = this.chunkSize,
                chunkDatas = new Dictionary<Vector3Int, ChunkData>(),
                chunks = new Dictionary<Vector3Int, ChunkRenderer>()
            };
        }
        
        WorldGenerationData worldGenerationData = GetPositionsWithinCamsFrustumPlane(position);

        foreach (Vector3Int pos in worldGenerationData.chunkPositionsToRemove)
        {
            WorldDataHelper.RemoveChunk(this, pos);
        }

        foreach (Vector3Int pos in worldGenerationData.chunkDataToRemove)
            WorldDataHelper.RemoveChunkData(this, pos);
        
        foreach (var pos in worldGenerationData.chunkDataPositionsToCreate)
        {
            ChunkData data = new ChunkData(chunkSize, chunkHeight, this, pos);
            ChunkData newData = terrainGenerator.GenerateChunkData(data, offset);
            worldData.chunkDatas.Add(pos, newData);
        }

        foreach (var pos in worldGenerationData.chunkPositionsToCreate)
        {
            ChunkData data = worldData.chunkDatas[pos];
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
        List<Vector3Int> allChunkDataPositionsNeeded = WorldDataHelper.GetDataPositionsAroundPlayer(this, playerPos);

        List<Vector3Int> chunkPositionsToCreate = 
            WorldDataHelper.SelectPositionsToCreate(worldData, allChunkPositionsNeeded, playerPos);
        List<Vector3Int> chunkDataPositionsToCreate = 
            WorldDataHelper.SelectDataPositionsToCreate(worldData, allChunkDataPositionsNeeded, playerPos);

        List<Vector3Int> chunkPositionsToRemove = WorldDataHelper.GetUnneededChunks(worldData, allChunkPositionsNeeded);
        List<Vector3Int> chunkDataToRemove = WorldDataHelper.GetUnneededData(worldData, allChunkDataPositionsNeeded);

        WorldGenerationData data = new WorldGenerationData
        {
            chunkPositionsToCreate = chunkPositionsToCreate,
            chunkDataPositionsToCreate = chunkDataPositionsToCreate,
            chunkPositionsToRemove = chunkPositionsToRemove,
            chunkDataToRemove = chunkDataToRemove
        };

        return data;
    }

    public void DestroyChunks()
    {
        foreach (Transform child in transform) SafeDestroy(child.gameObject);
        if (worldData.chunks == null) return;
        foreach (ChunkRenderer chunk in worldData.chunks.Values)
        {
            if (chunk != null) {SafeDestroy(chunk.gameObject);}
        }
        
        worldData.chunks.Clear();
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
        GenerateWorld(Vector3Int.RoundToInt(player.transform.position));
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

    public void RemoveChunk(ChunkRenderer chunk)
    {
        chunk.gameObject.SetActive(false);
    }
}
