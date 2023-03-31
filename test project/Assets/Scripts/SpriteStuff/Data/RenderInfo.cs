using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderInfo
{
    public Material material;
    public Mesh mesh;

    public ComputeBuffer argsBuffer;
    public ComputeBuffer instancesBuffer;
    public List<InstanceData> instanceDatas;

    public RenderInfo(Material material, Mesh mesh) {
        this.material = material;
        this.mesh = mesh;
        instanceDatas = new List<InstanceData>();
    }

    public void AddInstance(InstanceData instanceData) {
        instanceDatas.Add(instanceData);
        UpdateBuffers();
    }

    public void UpdateBuffers() {
        instancesBuffer?.Release();

        instancesBuffer = new ComputeBuffer(instanceDatas.Count, InstanceData.Size());
        
        instancesBuffer.SetData(instanceDatas);
    }

    public void UpdateMaterialBuffer() {
        material.SetBuffer("_PerInstanceData", instancesBuffer);
    }

    public void UpdateArgsBuffer() {
        argsBuffer?.Release();

        argsBuffer = new ComputeBuffer(1, sizeof(uint)*5);
        argsBuffer.SetData(new uint[] {
            mesh.GetIndexCount(0),
            (uint)instanceDatas.Count,
            mesh.GetIndexStart(0),
            mesh.GetBaseVertex(0),
            0
        });
    }

    public static Mesh NewQuadMesh() {
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