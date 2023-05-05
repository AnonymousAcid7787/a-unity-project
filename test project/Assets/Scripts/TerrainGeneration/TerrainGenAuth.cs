using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class TerrainGenAuth : MonoBehaviour 
{
    public int xSize;
    public int zSize;
}

public class TerrainGenBaker : Baker<TerrainGenAuth>
{
    public override void Bake(TerrainGenAuth authoring)
    {
        int vertexCount = (authoring.xSize + 1) * (authoring.zSize + 1);
        Vector3[] vertices = new Vector3[vertexCount];
        
        //Vertices
        for(int i = 0,z = 0; z <= authoring.zSize; z++) {
            for(int x = 0; x <= authoring.xSize; x++) {
                float y = Mathf.PerlinNoise(x * 0.3f, z * 0.3f) * 2f;
                vertices[i] = new float3(x, y, z);
                i++;
            }
        }

        //Triangles
        int vert = 0;
        int tris = 0;
        int triangleCount = authoring.xSize * authoring.zSize * 6;
        int[] triangles = new int[triangleCount];
        for (int z = 0; z < authoring.zSize; z++) {
            for(int x = 0; x < authoring.xSize; x++) {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + authoring.xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + authoring.xSize + 1;
                triangles[tris + 5] = vert + authoring.xSize + 2;

                vert++;
                tris += 6;
            }
            vert++; 
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        authoring.gameObject.GetComponent<MeshFilter>().mesh = mesh;

        AddComponent(GetEntity(TransformUsageFlags.None), new TerrainGenTag{});
    }
}

public struct TerrainGenTag : IComponentData {}

public partial class TerrainGenSystem : SystemBase
{
    protected override void OnUpdate()
    {
        
    }
}