using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;

public partial class DrawingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        foreach(RenderComponent component in SystemAPI.Query<RenderComponent>()) {
            RenderInfo info = MaterialCache.cachedRenderInfo[component.materialIndex];
            Graphics.DrawMeshInstancedIndirect(
                info.mesh,0,
                info.material,
                info.renderBounds,
                info.argsBuf
            );
        }
    }

    protected override void OnDestroy()
    {
        foreach(RenderInfo info in MaterialCache.cachedRenderInfo) {
            info.DestroyBuffers();
        }
        MaterialCache.cachedRenderInfo = new List<RenderInfo>();
    }
}