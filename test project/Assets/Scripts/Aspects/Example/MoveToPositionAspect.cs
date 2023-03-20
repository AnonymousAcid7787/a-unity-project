using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public readonly partial struct MoveToPositionAspect : IAspect
{
    
    private readonly Entity entity;

    private readonly TransformAspect transformAspect;
    private readonly RefRO<Speed> speed;
    private readonly RefRW<TargetPosition> targetPosition;

    public void Move(float deltaTime) {
        float3 dir = math.normalize(targetPosition.ValueRW.value - transformAspect.LocalPosition);

        transformAspect.LocalPosition += dir * deltaTime * speed.ValueRO.value;
    }

    public void TestReachedPosition(RefRW<RandomComponent> randomComponent) {
        float reachedDist = 0.5f;
        if(math.distancesq(transformAspect.LocalPosition, targetPosition.ValueRW.value) < reachedDist) {
            targetPosition.ValueRW.value = GetRandPos(randomComponent);
        }
    }

    private float3 GetRandPos(RefRW<RandomComponent> randomComponent) {
        return new float3 (
            randomComponent.ValueRW.random.NextFloat(0f, 15f),
            0,
            randomComponent.ValueRW.random.NextFloat(0f, 15f)
        );  
    }
}
