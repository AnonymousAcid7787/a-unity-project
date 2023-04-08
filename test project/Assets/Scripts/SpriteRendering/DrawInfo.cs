using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSheetDrawInfo
{
    public Material spriteSheetMaterial;
    public Mesh mesh;

    public ComputeBuffer argsBuffer;
    public ComputeBuffer instancesBuffer;
    public Dictionary<int, InstanceData> instanceDatas;
    public Bounds renderBounds;
    public MaterialPropertyBlock materialPropertyBlock;
    public UnityEngine.Rendering.ShadowCastingMode shadowCastingMode;
    public bool recieveShadows;
    public int frameCount;

    public SpriteSheetDrawInfo(Material material, Mesh mesh, Bounds renderBounds) {
        this.spriteSheetMaterial = material;
        this.mesh = mesh;
        this.renderBounds = renderBounds;
        materialPropertyBlock = new MaterialPropertyBlock();
        shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        recieveShadows = true;
        instanceDatas = new Dictionary<int, InstanceData>();
    }

    public SpriteSheetDrawInfo(RenderArgs args) {
        this.spriteSheetMaterial = args.material;
        this.materialPropertyBlock = args.materialPropertyBlock;
        this.mesh = args.mesh;
        this.recieveShadows = args.recieveShadows;
        this.renderBounds = args.renderBounds;
        this.shadowCastingMode = args.shadowCastingMode;
        this.instanceDatas = new Dictionary<int, InstanceData>();
    }

    public int AddInstance(InstanceData data) {
        int num = Random.Range(int.MinValue, int.MaxValue);
        while(instanceDatas.ContainsKey(num))
            num = Random.Range(int.MinValue, int.MaxValue);
        
        instanceDatas.Add(num, data);
        return num;
    }

    public InstanceData RemoveInstance(int key) {
        InstanceData data = instanceDatas[key];

        instanceDatas.Remove(key);

        return data;
    }

    public void UpdateAllBuffers() {
        UpdateArgsBuffer();
        UpdateInstancesBuffer();//update instance buffer first, because material buffer relies on it.
        UpdateMaterialBuffer();
    }

    public void UpdateInstancesBuffer() {
        instancesBuffer?.Release();

        instancesBuffer = new ComputeBuffer(instanceDatas.Count, InstanceData.Size());
        instancesBuffer.SetData(new List<InstanceData>(instanceDatas.Values));
    }

    public void UpdateArgsBuffer() {
        argsBuffer?.Release();

        argsBuffer = new ComputeBuffer(1, sizeof(uint)*5, ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(new uint[] {
            mesh.GetIndexCount(0),
            (uint)instanceDatas.Count,
            mesh.GetIndexStart(0),
            mesh.GetBaseVertex(0),
            0
        });
    }

    public void UpdateMaterialBuffer() {
        spriteSheetMaterial.SetBuffer("_PerInstanceData", instancesBuffer);
    }

    public void DestroyBuffers() {
        argsBuffer?.Release();
        instancesBuffer?.Release();
    }

    public void Draw() {
        if(instanceDatas.Count == 0)
            return;
        UpdateAllBuffers();

        Graphics.DrawMeshInstancedIndirect(
            mesh, 0,
            spriteSheetMaterial, renderBounds,
            argsBuffer, 0,
            materialPropertyBlock,
            shadowCastingMode,
            recieveShadows
        );
    }
}

public struct RenderArgs {
    public Material material;
    public Mesh mesh;
    public Bounds renderBounds;
    public MaterialPropertyBlock materialPropertyBlock;
    public UnityEngine.Rendering.ShadowCastingMode shadowCastingMode;
    public bool recieveShadows;

    public RenderArgs(Material material,Mesh mesh,
                      Bounds renderBounds,
                      MaterialPropertyBlock materialPropertyBlock,
                      UnityEngine.Rendering.ShadowCastingMode shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                      bool recieveShadows = true
                     )
    {
        this.material = material;
        this.mesh = mesh;
        this.renderBounds = renderBounds;
        this.materialPropertyBlock = materialPropertyBlock;
        this.shadowCastingMode = shadowCastingMode;
        this.recieveShadows = recieveShadows;
    }
}