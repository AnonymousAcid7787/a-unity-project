using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpriteInstanceAdder : SystemBase
{
    private EntityQuery query;
    protected override void OnCreate()
    {
        query = GetEntityQuery(typeof(SpriteRenderAspect));
    }
    protected override void OnUpdate()
    {
        foreach(SpriteRenderAspect aspect in SystemAPI.Query<SpriteRenderAspect>()) {
            if(aspect.animationData.ValueRW.hasAddedInstance)
                continue;
            
            SpriteSheetDrawInfo drawInfo = SpriteSheetCache.cache[aspect.animationData.ValueRW.drawInfoHashCode];
            drawInfo.instances.Add(aspect);
            aspect.animationData.ValueRW.hasAddedInstance = true;
        }
    }
}

// public partial struct InstanceAdderJob : IJobEntity {
//     public void Execute(ref SpriteRenderAspect aspect) {
//         if(aspect.animationData.ValueRW.hasAddedInstance)
//             return;

//         SpriteSheetDrawInfo drawInfo =  SpriteSheetCache.cache[aspect.animationData.ValueRW.drawInfoHashCode];
//         drawInfo.instances.add
        
//         aspect.animationData.ValueRW.hasAddedInstance = true;
//     }
// }


