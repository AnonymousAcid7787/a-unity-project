using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[UpdateAfter(typeof(SpriteInstanceUpdate))]
public partial class SpriteSheetRenderer : SystemBase
{
    protected override void OnCreate()
    {
        
    }
    protected override void OnUpdate()
    {
        foreach(SpriteSheetAnimationData data in SystemAPI.Query<SpriteSheetAnimationData>()) {
            SpriteSheetCache.cache[data.drawInfoHashCode].Draw();
        }
    }
}
