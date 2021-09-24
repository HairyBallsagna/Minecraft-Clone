using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Chunk
{
    public static MeshData GetChunkMeshData(ChunkData chunkData)
    {
        MeshData meshData = new MeshData(true);
        
        // TODO fill later
        
        return meshData;
    }

    public static void LoopThroughTheVoxels(ChunkData chunkData, Action<int, int, int> actionToPerform)
    {
        for (int i = 0; i < chunkData.voxels.Length; i++)
        {
            Vector3Int pos = GetPosFromIndex(chunkData, i);
            actionToPerform(pos.x, pos.y, pos.z);
        }
    }

    public static VoxelType GetBlockFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
    {
        if (InRange(chunkData, x) && InRangeHeight(chunkData, y) && InRange(chunkData, z))
        {
            int index = GetIndexFromPosition(chunkData, x, y, z);
            return chunkData.voxels[index];
        }
        
        throw new Exception("need to ask world for appropriate chunk");
    }

    public static VoxelType GetBlockFromChunkCoordinates(ChunkData chunkData, Vector3Int chunkCoords)
    {
        return GetBlockFromChunkCoordinates(chunkData, chunkCoords.x, chunkCoords.y, chunkCoords.z);
    }

    public static void SetBlock(ChunkData chunkData, Vector3Int localPos, VoxelType voxel)
    {
        if (InRange(chunkData, localPos.x) && InRangeHeight(chunkData, localPos.y) && InRange(chunkData, localPos.z))
        {
            int index = GetIndexFromPosition(chunkData, localPos.x, localPos.y, localPos.z);
            chunkData.voxels[index] = voxel;
        }
        else throw new Exception("need to ask world for apporpriate chunk mf");
    }

    public static Vector3Int GetBlockInChunkCoordinates(ChunkData chunkData, Vector3Int pos)
    {
        return new Vector3Int
        {
            x = pos.x - chunkData.worldPos.x,
            y = pos.y - chunkData.worldPos.y,
            z = pos.z - chunkData.worldPos.z
        };
    }

    private static Vector3Int GetPosFromIndex(ChunkData chunkData, int index)
    {
        int x = index % chunkData.chunkSize;
        int y = (index / chunkData.chunkSize) % chunkData.chunkHeight;
        int z = index / (chunkData.chunkSize * chunkData.chunkHeight);
        return new Vector3Int(x, y ,z);
    }

    private static int GetIndexFromPosition(ChunkData chunkData, int x, int y, int z)
    {
        return x + chunkData.chunkSize * y + chunkData.chunkSize * chunkData.chunkHeight * z;
    }

    private static bool InRange(ChunkData chunkData, int axisCoord)
    {
        if (axisCoord < 0 || axisCoord >= chunkData.chunkSize) return false;
        else return true;
    }

    private static bool InRangeHeight(ChunkData chunkData, int yCoord)
    {
        if (yCoord < 0 || yCoord >= chunkData.chunkHeight) return false;
        else return true;
    }
}
