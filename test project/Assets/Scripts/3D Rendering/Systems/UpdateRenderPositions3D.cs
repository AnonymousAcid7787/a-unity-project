using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Transforms;

//Update positions based on entities' "LocalToWorld" component
[BurstCompile]
public partial struct UpdateRenderPositions3D : ISystem
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
        JobHandle handle = new RenderPositionUpdater3D{}.ScheduleParallel(state.Dependency);
        handle.Complete();
    }
}

partial struct RenderPositionUpdater3D : IJobEntity {
    public void Execute(LocalToWorld worldMatrix, ref RenderData3D data) {
       data.instanceData.worldMatrix = worldMatrix.Value;
       data.instanceData.worldMatrixInverse = Matrix4x4.Inverse(worldMatrix.Value);
    }
}