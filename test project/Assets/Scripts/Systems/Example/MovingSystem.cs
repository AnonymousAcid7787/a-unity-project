using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Burst;

[BurstCompile]
public partial struct MovingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state) {}

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {}

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        return;
        RefRW<RandomComponent> randomComponent = SystemAPI.GetSingletonRW<RandomComponent>();

        float deltaTime = SystemAPI.Time.DeltaTime;
        
        //Move entities
        JobHandle handle = new MoveJob {
            deltaTime = deltaTime
        }.ScheduleParallel(state.Dependency);

        handle.Complete();
        //Once done moving them, check if they reached their destination. if they did, then move set another random position as the target
        new TestReachedPositionJob {
            randomComponent = randomComponent
        }.Run();
    }

}

[BurstCompile]
public partial struct MoveJob : IJobEntity {
    public float deltaTime;
    public void Execute(MoveToPositionAspect aspect) {
        aspect.Move(deltaTime);
    }
}

[BurstCompile]
public partial struct TestReachedPositionJob : IJobEntity {
    [NativeDisableUnsafePtrRestriction]
    public RefRW<RandomComponent> randomComponent;
    public void Execute(MoveToPositionAspect aspect) {
        aspect.TestReachedPosition(randomComponent);
    }
}