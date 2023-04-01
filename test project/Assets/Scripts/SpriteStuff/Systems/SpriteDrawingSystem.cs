using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public partial struct DrawSystem : ISystem
{
    private ComponentLookup<LocalTransform> lookup;
    public void OnCreate(ref SystemState state)
    {
        lookup = state.GetComponentLookup<LocalTransform>(false);
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        lookup.Update(ref state);

        JobHandle handle = new UpdateDrawPositions{
            lookup = lookup
        }.ScheduleParallel(state.Dependency);
        handle.Complete();

    }
}


public partial struct UpdateDrawPositions : IJobEntity {
    [ReadOnly]
    public ComponentLookup<LocalTransform> lookup;
    public void Execute(ref SpriteComponent spriteComponent, ref LocalToWorld localToWorld) {
        spriteComponent.instanceData.worldMatrix = localToWorld.Value;
        spriteComponent.instanceData.worldMatrixInverse = Matrix4x4.Inverse(localToWorld.Value);
    }
}