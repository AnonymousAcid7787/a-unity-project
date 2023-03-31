using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Mathematics;

public partial struct DrawSystems : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        JobHandle handle = new UpdateDrawPositions{}.ScheduleParallel(state.Dependency);
        handle.Complete();
    }
}

public partial struct UpdateDrawPositions : IJobEntity {
    public void Execute(ref SpriteComponent spriteComponent) {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        LocalTransform parentTransform = entityManager.GetComponentData<LocalTransform>(spriteComponent.parentEntity);
        spriteComponent.instanceData.worldMatrix = Matrix4x4.TRS(
            parentTransform.Position,
            parentTransform.Rotation,
            new float3(1,1,1)
        );
        spriteComponent.instanceData.worldMatrixInverse = Matrix4x4.Inverse(spriteComponent.instanceData.worldMatrix);
    }
}