using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Entities;

public class Testing : MonoBehaviour
{
    public Sprite sprite;
    public List<Texture2D> textures;
    public Material baseMaterial;

    private DrawData data;
    private InstanceData instanceData;

    void Start() {
        data = new DrawData(new Material(baseMaterial), DrawData.NewQuadMesh(), new MaterialPropertyBlock(), new Bounds(Vector3.zero, Vector3.one*10));
        instanceData = data.AddInstance(transform.position, transform.rotation, Vector3.one);
    }

    void Update() {
        transform.position = new Vector3(
            UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f)
        );
        UpdateInstanceData();
        data.Draw();
    }

    public void UpdateInstanceData() {
        instanceData.worldMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        instanceData.worldMatrixInverse = Matrix4x4.Inverse(instanceData.worldMatrix);
        data.instanceData[0] = instanceData;
        data.UpdateBuffers();
        data.UpdateMaterialBuffer();
    }
}