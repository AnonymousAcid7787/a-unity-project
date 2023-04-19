using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct IsGroundedSystem : ISystem {
    public void OnCreate(ref SystemState state) {

    }
    public void OnDestroy(ref SystemState state) {

    }
    public void OnUpdate(ref SystemState state) {

        state.Dependency = 
            new IsGroundedJob{
                
            }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }

    struct IsGroundedJob : ICollisionEventsJob {
        public void Execute(CollisionEvent collisionEvent) {
            // Debug.Log(collisionEvent.Normal);
        }
    }
}