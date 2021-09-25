using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelDataManager : MonoBehaviour
{
    public static float textureOffset = 0.001f;
    public static float tileSizeX;
    public static float tileSizeY;
    public static Dictionary<VoxelType, TextureData> voxelTextureDatas = new Dictionary<VoxelType, TextureData>();
    public VoxelDataSO textureData;

    public void Initialize()
    {
        foreach (TextureData item in textureData.textureDataList)
        {
            if (!voxelTextureDatas.ContainsKey(item.voxelType))
                voxelTextureDatas.Add(item.voxelType, item);
        }

        tileSizeX = textureData.textureSizeX;
        tileSizeY = textureData.textureSizeY;
    }
}
