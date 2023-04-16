using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct PlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        new PlayerMovementJob{}.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct PlayerMovementJob : IJobEntity {

    [BurstCompile]
    public void Execute(in PlayerMovementData data, ref LocalTransform localTransform) {
        localTransform.Position.x += data.movementDirection.x * data.movementSpeed;
        localTransform.Position.z += data.movementDirection.z * data.movementSpeed;
    }
}