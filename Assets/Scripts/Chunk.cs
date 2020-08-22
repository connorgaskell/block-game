using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Chunk : MonoBehaviour {

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private int vertexIndex = 0;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();

    private bool[,,] voxelMap = new bool[BlockData.chunkWidth, BlockData.chunkHeight, BlockData.chunkWidth];

    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }

    private void Start() {
        PopulateMap();
        CreateMeshData();
        

        CreateMesh();
    }

    private void CreateMeshData() {
        for(int x = 0; x < BlockData.chunkWidth; x++) {
            for(int y = 0; y < BlockData.chunkHeight; y++) {
                for(int z = 0; z < BlockData.chunkWidth; z++) {
                    AddToChunk(new Vector3(x, y, z));
                }
            }
        }
    }

    private void PopulateMap() {
        for(int x = 0; x < BlockData.chunkWidth; x++) {
            for(int y = 0; y < BlockData.chunkHeight; y++) {
                for(int z = 0; z < BlockData.chunkWidth; z++) {
                    voxelMap[x, y, z] = true;
                }
            }
        }
    }

    private bool CheckBlock(Vector3 pos) {
        int x = Mathf.FloorToInt(pos.x), y = Mathf.FloorToInt(pos.y), z = Mathf.FloorToInt(pos.z);
        if(x < 0 || x > BlockData.chunkWidth - 1 || y < 0 || y > BlockData.chunkHeight - 1 || z < 0 || z > BlockData.chunkWidth - 1) return false;
        return voxelMap[x, y, z];
    }

    private void AddToChunk(Vector3 pos) {
        for(int i = 0; i < 6; i++) {
            if(!CheckBlock(pos + BlockData.faces[i])) {
                for(int j = 0; j < 6; j++) {
                    int triangleIndex = BlockData.triangles[i, j];
                    vertices.Add(BlockData.vertices[triangleIndex] + pos);
                    triangles.Add(vertexIndex++);
                    uvs.Add(BlockData.uvs[j]);
                }
            }
        }
    }

    private void CreateMesh() {
        Mesh mesh = new Mesh() {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = uvs.ToArray()
        };

        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

}
