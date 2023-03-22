using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class RenderInfo 
{
    public Material material;
    public Mesh mesh;
    public ComputeBuffer argsBuf;
    public List<Matrix4x4> matrices;
    private Matrix4x4[] matrixArray;
    private uint instanceCount = 0;
    public Bounds renderBounds;

    public RenderInfo(Material material, Vector3 position, Quaternion rotation, Vector3 scale, Bounds renderBounds) {
        mesh = QuadMesh();
        this.renderBounds = renderBounds;
        matrices = new List<Matrix4x4>();
        AddInstance(position, rotation, scale, 1);
    }

    private void UpdateMatrixArray() {
        matrixArray = matrices.ToArray();
    }

    public void AddInstance(Vector3 position, Quaternion rotation, Vector3 scale, int count = 1) {
        for(var i=0; i<count; i++) 
            matrices.Add(Matrix4x4.TRS(position, rotation, scale));

        instanceCount += (uint)count;
        
        //Free previous memory
        if(argsBuf != null && argsBuf.IsValid()) {
            argsBuf.Release();
        }
        
        argsBuf = new ComputeBuffer(1, 5*sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuf.SetData(
            new uint[] {
                mesh.GetIndexCount(0),
                instanceCount,
                mesh.GetIndexStart(0),
                mesh.GetBaseVertex(0),
                0,
            }
        );

        UpdateMatrixArray();
    }

    public void DestroyBuffers() {
        argsBuf.Release();
        argsBuf = null;
    }

    public static Mesh QuadMesh() {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(1, 0, 0);
        vertices[2] = new Vector3(0, 1, 0);
        vertices[3] = new Vector3(1, 1, 0);
        mesh.vertices = vertices;

        int[] tri = new int[6];
        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;
        tri[3] = 2;
        tri[4] = 3;
        tri[5] = 1;
        mesh.triangles = tri;

        Vector3[] normals = new Vector3[4];
        normals[0] = -Vector3.forward;
        normals[1] = -Vector3.forward;
        normals[2] = -Vector3.forward;
        normals[3] = -Vector3.forward;
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4];
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);
        mesh.uv = uv;

        return mesh;
    }
}
