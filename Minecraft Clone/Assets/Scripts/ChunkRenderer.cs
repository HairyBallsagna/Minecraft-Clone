using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkRenderer : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private Mesh _mesh;
    
    public bool showGizmo = false;
    
    public ChunkData ChunkData { get; private set; }

    public bool ModifiedByThePlayer 
    {
        get { return ChunkData.modifiedByThePlayer; }
        set { ChunkData.modifiedByThePlayer = value; }
    }

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
        _mesh = _meshFilter.mesh;
    }

    public void InitializeChunk(ChunkData data)
    {
        this.ChunkData = data;
    }

    private void RenderMesh(MeshData meshData)
    {
        _mesh.Clear();

        _mesh.subMeshCount = 2;
        _mesh.vertices = meshData.vertices.Concat(meshData.waterMesh.vertices).ToArray();
        
        _mesh.SetTriangles(meshData.triangles.ToArray(), 0);
        _mesh.SetTriangles(meshData.waterMesh.triangles.Select(n => n + meshData.vertices.Count).ToArray(), 1);

        _mesh.uv = meshData.uv.Concat(meshData.waterMesh.uv).ToArray();
        _mesh.RecalculateNormals();

        _meshCollider.sharedMesh = null;
        Mesh collisionMesh = new Mesh();
        collisionMesh.vertices = meshData.colliderVertices.ToArray();
        collisionMesh.triangles = meshData.colliderTriangles.ToArray();
        collisionMesh.RecalculateNormals();

        _meshCollider.sharedMesh = collisionMesh;
    }

    public void UpdateChunk()
    {
        RenderMesh(Chunk.GetChunkMeshData(ChunkData));
    }

    public void UpdateChunk(MeshData data)
    {
        RenderMesh(data);
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (showGizmo)
        {
            if (Application.isPlaying && ChunkData != null)
            {
                if (Selection.activeObject == gameObject)
                    Gizmos.color = new Color(0, 1, 0, 0.4f);
                else
                    Gizmos.color = new Color(1, 0, 1, 0.4f);
                
                Gizmos.DrawCube(transform.position + new Vector3(ChunkData.chunkSize / 2f, ChunkData.chunkHeight / 2f, ChunkData
                    .chunkSize / 2f), new Vector3(ChunkData.chunkSize, ChunkData.chunkHeight, ChunkData.chunkSize));
            }
        }
    }

#endif    
}

