using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName ="Block Data" ,menuName ="Data/Block Data")]
public class VoxelDataSO : ScriptableObject
{
    public float textureSizeX;
    public float textureSizeY;
    public List<TextureData> textureDataList;
}

[Serializable]
public class TextureData
{
    [FormerlySerializedAs("blockType")] public VoxelType voxelType;
    public Vector2Int up;
    public Vector2Int down;
    public Vector2Int side;
    public bool isSolid = true;
    public bool generatesCollider = true;
}

