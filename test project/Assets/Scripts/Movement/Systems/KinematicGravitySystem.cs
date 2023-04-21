using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(IsGroundedSystem))] 
public partial struct KinematicGravitySystem : ISystem
{
    public void OnUpdate(ref SystemState state) {
        float deltaTime = SystemAPI.Time.DeltaTime;

        state.Dependency = new KinematicGravityJob{
            deltaTime = deltaTime
        }.ScheduleParallel(state.Dependency);
    }

    partial struct KinematicGravityJob : IJobEntity {
        public float deltaTime;
        public void Execute(ref PhysicsVelocity velocity, in PhysicsMass mass, in MovementData data) {
            if(!mass.IsKinematic)
                return;
            velocity.Linear.y -= 9.81f * data.gravityFactor * deltaTime;
        }
    }
}
