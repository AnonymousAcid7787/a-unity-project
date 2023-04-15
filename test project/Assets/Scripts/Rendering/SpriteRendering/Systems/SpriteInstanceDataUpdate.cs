using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
[UpdateAfter(typeof(SpriteSheetAnimationSystem))]
public partial struct SpriteInstanceDataUpdate : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        JobHandle handle =  new SpriteInstanceDataUpdateJob{}.ScheduleParallel(state.Dependency);
        handle.Complete();
    }
}
public partial struct SpriteInstanceDataUpdateJob : IJobEntity {
    public void Execute(SpriteRenderAspect aspect) {
        SpriteSheetDrawInfo drawInfo = SpriteSheetCache.cache[aspect.animationData.ValueRW.drawInfoHashCode];
        drawInfo.instances[aspect.animationData.ValueRW.instanceKey] = aspect.animationData.ValueRW.instanceData;
    }
}