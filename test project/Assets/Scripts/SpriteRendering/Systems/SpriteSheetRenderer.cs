using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[UpdateAfter(typeof(SpriteSheetAnimationSystem))]
public partial class SpriteSheetRenderer : SystemBase
{
    protected override void OnUpdate()
    {
        foreach(SpriteSheetAnimationData data in SystemAPI.Query<SpriteSheetAnimationData>()) {
            SpriteSheetCache.cache[data.drawInfoHashCode].Draw();
        }
    }
}
