using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct TestRaycastSystem : ISystem
{
    public void OnCreate(ref SystemState state) {}
    public void OnDestroy(ref SystemState state) {}
    public void OnUpdate(ref SystemState state) {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        state.Dependency = new TestRaycastJob{}.ScheduleParallel(state.Dependency);
    }

    partial struct TestRaycastJob : IJobEntity {
        public PhysicsWorldSingleton physicsWorldSingleton;
        public void Execute(in LocalTransform localTransform,
                            in PhysicsMass mass,
                            in PhysicsCollider collider,
                            ref MovementData movementData
                            ) 
        {
            if(mass.IsKinematic)
                return;
            float3 pos = localTransform.Position;
            movementData.isGrounded = physicsWorldSingleton.CastRay(new RaycastInput {
                Filter = collider.Value.Value.GetCollisionFilter(),
                Start = pos,
                End = pos + new float3(0,-0.1f,0)
            });
        }
    }
}
