using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public partial class SpriteDrawingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        foreach(SpriteComponent spriteComponent in SystemAPI.Query<SpriteComponent>()) {
            RenderInfo info = RenderCache.renderCache[spriteComponent.renderCacheIndex];
                info.instanceDatas[spriteComponent.instanceDataIndex] = spriteComponent.instanceData;
            info.Draw();
        }
    }
}
