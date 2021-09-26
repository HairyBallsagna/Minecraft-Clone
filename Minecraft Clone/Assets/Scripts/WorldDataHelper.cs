using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WorldDataHelper
{
    public static Vector3Int ChunkPositionFromBlockCoords(World world, Vector3Int pos)
    {
        return new Vector3Int
        {
            x = Mathf.FloorToInt(pos.x / (float) world.chunkSize) * world.chunkSize,
            y = Mathf.FloorToInt(pos.y / (float) world.chunkHeight) * world.chunkHeight,
            z = Mathf.FloorToInt(pos.z / (float) world.chunkSize) * world.chunkSize
        };
    }

    public static List<Vector3Int> GetChunkPositionsAroundPlayer(World world, Vector3Int playerPos)
    {
        int startX = playerPos.x - (world.chunkDrawDistance) * world.chunkSize;
        int startZ = playerPos.z - (world.chunkDrawDistance) * world.chunkSize;
        int endX = playerPos.x + (world.chunkDrawDistance) * world.chunkSize;
        int endZ = playerPos.z + (world.chunkDrawDistance) * world.chunkSize;

        List<Vector3Int> chunkPositionsToCreate = new List<Vector3Int>();

        for (int x = startX; x <= endX; x += world.chunkSize)
        {
            for (int z = startZ; z <= endZ; z += world.chunkSize)
            {
                Vector3Int chunkPos = ChunkPositionFromBlockCoords(world, new Vector3Int(x, 0, z));
                chunkPositionsToCreate.Add(chunkPos);

                // if (x >= playerPos.x - world.chunkSize && x <= playerPos.x + world.chunkSize &&
                //     z >= playerPos.z - world.chunkSize && z <= playerPos.z + world.chunkSize)
                // {
                //     for (int y = -world.chunkHeight; y >= playerPos.y - world.chunkSize * 2; y -= world.chunkHeight)
                //     {
                //         chunkPos = ChunkPositionFromBlockCoords(world, new Vector3Int(x, y, z));
                //         chunkPositionsToCreate.Add(chunkPos);
                //     }
                // }
            }
        }

        return chunkPositionsToCreate;
    }

    public static List<Vector3Int> GetDataPositionsAroundPlayer(World world, Vector3Int playerPos)
    {
        int startX = playerPos.x - (world.chunkDrawDistance + 1) * world.chunkSize;
        int startZ = playerPos.z - (world.chunkDrawDistance + 1) * world.chunkSize;
        int endX = playerPos.x + (world.chunkDrawDistance + 1) * world.chunkSize;
        int endZ = playerPos.z + (world.chunkDrawDistance + 1) * world.chunkSize;

        List<Vector3Int> chunkDataPositionsToCreate = new List<Vector3Int>();

        for (int x = startX; x <= endX; x += world.chunkSize)
        {
            for (int z = startZ; z <= endZ; z += world.chunkSize)
            {
                Vector3Int chunkPos = ChunkPositionFromBlockCoords(world, new Vector3Int(x, 0, z));
                chunkDataPositionsToCreate.Add(chunkPos);

                // if (x >= playerPos.x - world.chunkSize && x <= playerPos.x + world.chunkSize &&
                //     z >= playerPos.z - world.chunkSize && z <= playerPos.z + world.chunkSize)
                // {
                //     for (int y = -world.chunkHeight; y >= playerPos.y - world.chunkSize * 2; y -= world.chunkHeight)
                //     {
                //         chunkPos = ChunkPositionFromBlockCoords(world, new Vector3Int(x, y, z));
                //         chunkDataPositionsToCreate.Add(chunkPos);
                //     }
                // }
            }
        }

        return chunkDataPositionsToCreate;
    }

    public static List<Vector3Int> SelectPositionsToCreate(World.WorldData worldData, List<Vector3Int> allChunkPositionsNeeded, Vector3Int playerPos)
    {
        return allChunkPositionsNeeded
            .Where(pos => worldData.chunks.ContainsKey(pos) == false)
            .OrderBy(pos => Vector3.Distance(playerPos, pos))
            .ToList();
    }

    public static List<Vector3Int> SelectDataPositionsToCreate(World.WorldData worldData, List<Vector3Int> allChunkDataPositionsNeeded, Vector3Int playerPos)
    {
        return allChunkDataPositionsNeeded
            .Where(pos => worldData.chunkDatas.ContainsKey(pos) == false)
            .OrderBy(pos => Vector3.Distance(playerPos, pos))
            .ToList();
    }

    public static List<Vector3Int> GetUnneededChunks(World.WorldData worldData, List<Vector3Int> allChunkPositionsNeeded)
    {
        List<Vector3Int> positionsToRemove = new List<Vector3Int>();

        foreach (var pos in worldData.chunks.Keys.Where(pos => allChunkPositionsNeeded.Contains(pos) == false))
        {
            if (worldData.chunks.ContainsKey(pos)) positionsToRemove.Add(pos);
        }

        return positionsToRemove;
    }

    public static List<Vector3Int> GetUnneededData(World.WorldData worldData, List<Vector3Int> allChunkDataPositionsNeeded)
    {
        return worldData.chunkDatas.Keys.Where(pos =>
            allChunkDataPositionsNeeded.Contains(pos) == false &&
            worldData.chunkDatas[pos].modifiedByThePlayer == false).ToList();
    }

    public static void RemoveChunk(World world, Vector3Int pos)
    {
        ChunkRenderer chunk = null;
        if (world.worldData.chunks.TryGetValue(pos, out chunk))
        {
            world.RemoveChunk(chunk);
            world.worldData.chunks.Remove(pos);
        }
    }

    public static void RemoveChunkData(World world, Vector3Int pos)
    {
        world.worldData.chunkDatas.Remove(pos);
    }
}
