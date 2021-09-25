using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelHelper
{
    private static Direction[] directions =
    {
        Direction.down,
        Direction.up,
        Direction.back,
        Direction.forward,
        Direction.left,
        Direction.right
    };

    public static Vector2Int TexturePos(Direction dir, VoxelType type)
    {
        return dir switch
        {
            Direction.up => VoxelDataManager.voxelTextureDatas[type].up,
            Direction.down => VoxelDataManager.voxelTextureDatas[type].down,
            _ => VoxelDataManager.voxelTextureDatas[type].side
        };
    }

    public static Vector2[] FaceUVs(Direction dir, VoxelType type)
    {
        Vector2[] UVs = new Vector2[4];
        Vector2Int tilePos = TexturePos(dir, type);
        
        UVs[0] = new Vector2(VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.tileSizeX - VoxelDataManager.textureOffset, 
            VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.textureOffset);
        
        UVs[1] = new Vector2(VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.tileSizeX - VoxelDataManager.textureOffset, 
            VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.tileSizeY - VoxelDataManager.textureOffset);
        
        UVs[2] = new Vector2(VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.textureOffset, 
            VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.tileSizeY - VoxelDataManager.textureOffset);
        
        UVs[3] = new Vector2(VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.textureOffset, 
            VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.textureOffset);

        return UVs;
    }
    
    public static void GetFaceVertices(Direction direction, int x, int y, int z, MeshData meshData, VoxelType blockType)
    {
        bool generatesCollider = VoxelDataManager.voxelTextureDatas[blockType].generatesCollider;
        //order of vertices matters for the normals and how we render the mesh
        switch (direction)
        {
            case Direction.back:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                break;
            case Direction.forward:
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                break;
            case Direction.left:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                break;

            case Direction.right:
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                break;
            case Direction.down:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                break;
            case Direction.up:
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                break;
            default:
                break;
        }
    }

    public static MeshData GetFaceDataIn(Direction dir, ChunkData chunk, int x, int y, int z, MeshData data,
        VoxelType type)
    {
        GetFaceVertices(dir, x, y, z, data, type);
        data.AddQuadTriangle(VoxelDataManager.voxelTextureDatas[type].generatesCollider);
        data.uv.AddRange(FaceUVs(dir, type));

        return data;
    }

    public static MeshData GetMeshData(ChunkData chunk, int x, int y, int z, MeshData data, VoxelType type)
    {
        if (type == VoxelType.Air || type == VoxelType.Nothing) return data;

        foreach (Direction dir in directions)
        {
            Vector3Int neighbourVoxelCoords = new Vector3Int(x, y, z) + dir.GetVector();
            VoxelType neighbourVoxelType = Chunk.GetVoxelFromChunkCoords(chunk, neighbourVoxelCoords);

            if (neighbourVoxelType != VoxelType.Nothing && !VoxelDataManager.voxelTextureDatas[neighbourVoxelType].isSolid)
            {
                if (type == VoxelType.Water)
                {
                    if (neighbourVoxelType == VoxelType.Air)
                        data.waterMesh = GetFaceDataIn(dir, chunk, x, y, z, data.waterMesh, type);
                }
                else data = GetFaceDataIn(dir, chunk, x, y, z, data, type);
            }
        }

        return data;
    }
}
