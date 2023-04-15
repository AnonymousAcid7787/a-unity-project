using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[UpdateAfter(typeof(SpriteInstanceDataUpdate))]
public partial class SpriteSheetRenderer : SystemBase
{
    protected override void OnCreate()
    {
        SpriteSheetCache.cache = new Dictionary<int, SpriteSheetDrawInfo>();
        SpriteSheetCache.cachedSpriteSheets = new Dictionary<Texture, Sprite[]>();
    }
    protected override void OnDestroy()
    {
        SpriteSheetCache.ClearCache();
    }
    protected override void OnUpdate()
    {
        foreach(SpriteSheetAnimationData data in SystemAPI.Query<SpriteSheetAnimationData>()) {
            SpriteSheetCache.cache[data.drawInfoHashCode].Draw();
        }
    }
}
