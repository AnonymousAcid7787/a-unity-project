using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderInformation
{
    public Material material;
    public Mesh mesh;
    private ComputeBuffer argsBuffer;
    private ComputeBuffer matrixBuffer;
    public List<Matrix4x4> transformMatrices;
    public Bounds renderBounds;
    private MaterialPropertyBlock propertyBlock;

    public RenderInformation(Material baseMaterial, Mesh mesh, Bounds renderBounds) {
        this.mesh = mesh;
        this.renderBounds = renderBounds;
        transformMatrices = new List<Matrix4x4>();
        material = new Material(baseMaterial);
        argsBuffer = new ComputeBuffer(1, 5*sizeof(uint), ComputeBufferType.IndirectArguments);
        propertyBlock = new MaterialPropertyBlock();
    }

    public void AddTransform(Matrix4x4 transformMatrix) {
        transformMatrices.Add(transformMatrix);

        RefreshMatrixBuffer();
        argsBuffer.SetData(new uint[5] {
            mesh.GetIndexCount(0),
            (uint)transformMatrices.Count,
            0,0,0
        });
    }

    public void RefreshMatrixBuffer() {
        if(matrixBuffer != null)
            matrixBuffer.Release();
        matrixBuffer = new ComputeBuffer(transformMatrices.Count, sizeof(float) * 4*4);
        matrixBuffer.SetData(transformMatrices);
        material.SetBuffer("transforms", matrixBuffer);
    }

    public void ReleaseBuffers() {
        if(argsBuffer != null)
            argsBuffer.Release();
        argsBuffer = null;

        if(matrixBuffer != null)
            matrixBuffer.Release();
        matrixBuffer = null;
    }

    public void Draw() {
        Graphics.DrawMeshInstancedIndirect(
            mesh, 0,
            material,
            renderBounds,
            argsBuffer, 0,
            propertyBlock, 
            UnityEngine.Rendering.ShadowCastingMode.Off, true
        );
    }
}
