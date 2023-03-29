using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawData
{
    public Material material;
    public Mesh mesh;
    public MaterialPropertyBlock mpb;
    public List<InstanceDataClass> instanceDataObjs;
    public Bounds renderBounds;
    public UnityEngine.Rendering.ShadowCastingMode shadowCastingMode;
    public int layer;
    public bool receiveShadows;

    private uint[] args;
    private ComputeBuffer argsBuffer;
    private ComputeBuffer instancesBuffer;


    //
    // Summary:
    //     Creates new draw data that doesn't contain any instancess yet
    public DrawData(Material material, Mesh mesh, MaterialPropertyBlock mpb, Bounds renderBounds) {
        this.material = material;
        this.mesh = mesh;
        this.mpb = mpb;
        this.renderBounds = renderBounds;
        instanceDataObjs = new List<InstanceDataClass>();
        shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        layer = 0;
        receiveShadows = true;
    }

    public InstanceDataClass AddInstance(Vector3 position, Quaternion rotation, Vector3 scale) {
        Matrix4x4 worldMatrix = Matrix4x4.TRS(position, rotation, scale);
        InstanceDataClass data = new InstanceDataClass(
            worldMatrix,
            Matrix4x4.Inverse(worldMatrix)
        );
        instanceDataObjs.Add(data);

        UpdateBuffers();
        UpdateMaterialBuffer();
        return data;
    }

    public void Draw() {
        if(argsBuffer == null || instancesBuffer == null)
            return;
        
        Graphics.DrawMeshInstancedIndirect(
            mesh, 0,
            material, renderBounds,
            argsBuffer, 0,
            mpb,
            shadowCastingMode, receiveShadows,
            layer
        );
    }

    public void UpdateBuffers() {
        instancesBuffer?.Release();

        //Instance buffer
        InstanceDataStruct[] instanceDataStructs = new InstanceDataStruct[instanceDataObjs.Count];
        for(var i=0; i<instanceDataObjs.Count; i++) {
            instanceDataStructs[i] = instanceDataObjs[i].bufferStruct;
        }
        instancesBuffer = new ComputeBuffer(instanceDataObjs.Count, InstanceDataStruct.Size());
        instancesBuffer.SetData(instanceDataStructs);

        SetupArgsBuffer();
    }

    public void SetupArgsBuffer() {
        argsBuffer?.Release();

        //Args buffer
        argsBuffer = new ComputeBuffer(1, sizeof(uint)*5, ComputeBufferType.IndirectArguments);

        args = new uint[] { 
            mesh.GetIndexCount(0),
            (uint)instanceDataObjs.Count,
            mesh.GetIndexStart(0),
            mesh.GetBaseVertex(0),
            0,
        };

        argsBuffer.SetData(args);
    }

    public void UpdateMaterialBuffer() {
        material.SetBuffer("_PerInstanceData", instancesBuffer);
    }

    public void DestroyBuffers() {
        argsBuffer?.Release();
            argsBuffer = null;

        instancesBuffer?.Release();
            instancesBuffer = null;
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
