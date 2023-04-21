using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Burst;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(IsGroundedSystem))] 
[BurstCompile]
public partial struct KinematicGravitySystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        float deltaTime = SystemAPI.Time.DeltaTime;

        state.Dependency = new KinematicGravityJob{
            deltaTime = deltaTime
        }.ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    partial struct KinematicGravityJob : IJobEntity {
        public float deltaTime;
        
        [BurstCompile]
        public void Execute(ref PhysicsVelocity velocity, in PhysicsMass mass, in MovementData data) {
            if(!mass.IsKinematic)
                return;
            velocity.Linear.y -= 9.81f * data.gravityFactor * deltaTime;
        }
    }
}
