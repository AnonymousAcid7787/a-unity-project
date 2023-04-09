using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;

public partial struct SpriteInstanceAdder : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        JobHandle handle =  new InstanceAdderJob{}.ScheduleParallel(state.Dependency);
        handle.Complete();
    }
}
public partial struct InstanceAdderJob : IJobEntity {
    public void Execute(ref SpriteRenderAspect aspect) {
        if(aspect.animationData.ValueRW.hasAddedInstance)
            return;
        
        SpriteSheetDrawInfo drawInfo = SpriteSheetCache.cache[aspect.animationData.ValueRW.drawInfoHashCode];
        drawInfo.instances.Add(aspect.animationData);
        aspect.animationData.ValueRW.hasAddedInstance = true;
    }
}