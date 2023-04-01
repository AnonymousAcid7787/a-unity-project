using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
public partial struct DrawPositionSystem : ISystem
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
        JobHandle handle = new UpdateDrawPositions{}.ScheduleParallel(state.Dependency);
        handle.Complete();
    }
}


[BurstCompile]
public partial struct UpdateDrawPositions : IJobEntity {    
    [BurstCompile]
    public void Execute(ref SpriteComponent spriteComponent, ref LocalTransform localTransform) {
        spriteComponent.instanceData.worldMatrix = Matrix4x4.TRS(
            localTransform._Position + spriteComponent.positionOffset,
            localTransform._Rotation,
            spriteComponent.scale
        );
        spriteComponent.instanceData.worldMatrixInverse = Matrix4x4.Inverse(spriteComponent.instanceData.worldMatrix);
    }
}