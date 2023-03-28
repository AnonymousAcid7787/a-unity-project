using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System.Runtime.InteropServices;

public class DrawingTest : MonoBehaviour
{
    public Sprite sprite;
    public Material material;
    public Mesh mesh;

    private ComputeBuffer argsBuffer;
    private ComputeBuffer instancesBuffer;
    private MaterialPropertyBlock propertyBlock;
    private uint[] args;
    private Bounds bounds;
    private List<InstanceDataClass> instanceData;

    // Start is called before the first frame update
    void Start()
    {
        material = new Material(material);
        material.mainTexture = sprite.texture;
        instanceData = new List<InstanceDataClass>();
        propertyBlock = new MaterialPropertyBlock();
        bounds = new Bounds(Vector3.zero, Vector3.one*10);

        //Transform matrices
        for(var i=0; i<3800; i++) {
            Vector3 offset = new Vector3(
                UnityEngine.Random.Range(-1f, 1f),
                UnityEngine.Random.Range(-1f, 1f),
                UnityEngine.Random.Range(-1f, 1f)
            );
            Matrix4x4 worldMatrix = Matrix4x4.TRS(transform.position+offset, transform.rotation, Vector3.one);
            InstanceDataClass data = new InstanceDataClass(worldMatrix, Matrix4x4.Inverse(worldMatrix));
            instanceData.Add(data);
        }

        UpdateBuffers();
        UpdateMaterialBuffer();
    }

    void Update() {
        Graphics.DrawMeshInstancedIndirect(
            mesh, 0,
            material,
            bounds,
            argsBuffer, 0,
            propertyBlock,
            UnityEngine.Rendering.ShadowCastingMode.Off,
            true
        );
    }

    void OnDestroy() {
        argsBuffer?.Release();
        instancesBuffer?.Release();
    }

    public void UpdateBuffers() {
        
        //Instance buffer
        instancesBuffer = new ComputeBuffer(instanceData.Count, InstanceDataStruct.Size());
        instancesBuffer.SetData(instanceData.ToArray());

        SetupArgsBuffer();
    }

    public void SetupArgsBuffer() {
        //Args buffer
        argsBuffer = new ComputeBuffer(1, sizeof(uint)*5, ComputeBufferType.IndirectArguments);

        args = new uint[] { 
            mesh.GetIndexCount(0),
            (uint)instanceData.Count,
            mesh.GetIndexStart(0),
            mesh.GetBaseVertex(0),
            0,
        };
        argsBuffer.SetData(args);
    }

    public void UpdateMaterialBuffer() {
        material.SetBuffer("_PerInstanceData", instancesBuffer);
    }
}