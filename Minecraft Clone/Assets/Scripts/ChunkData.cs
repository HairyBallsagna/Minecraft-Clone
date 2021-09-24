using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData
{
    public VoxelType[] voxels;
    public int chunkSize = 16;
    public int chunkHeight = 100;
    public World worldRef;
    public Vector3Int worldPos;

    public bool modifiedByThePlayer = false;

    public ChunkData(int chunkSize, int chunkHeight, World world, Vector3Int worldPos)
    {
        this.chunkSize = chunkSize;
        this.chunkHeight = chunkHeight;
        this.worldRef = world;
        this.worldPos = worldPos;
        voxels = new VoxelType[chunkSize * chunkHeight * chunkSize];
    }
}
