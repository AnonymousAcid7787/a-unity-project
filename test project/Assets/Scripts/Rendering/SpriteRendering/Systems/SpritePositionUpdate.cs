using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Transforms;

//Instance data modifier
[BurstCompile]
public partial struct SpritePositionUpdate : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        JobHandle handle =  new SpritePositionUpdateJob{}.ScheduleParallel(state.Dependency);
        handle.Complete();
    }
}


[BurstCompile]
public partial struct SpritePositionUpdateJob : IJobEntity {

    [BurstCompile]
    public void Execute(ref LocalToWorld localToWorld, ref SpriteSheetAnimationData data) {
        data.instanceData.worldMatrix = localToWorld.Value;
        data.instanceData.worldMatrixInverse = Matrix4x4.Inverse(localToWorld.Value);
    }
}