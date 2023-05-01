using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;
using Unity.Physics.Aspects;
using Unity.Jobs;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(LockPlayerRotationSystem))]
public partial struct TestRaycastSystem : ISystem
{
    public void OnCreate(ref SystemState state) {}
    public void OnDestroy(ref SystemState state) {}
    public void OnUpdate(ref SystemState state) {
        JobHandle handle = new ResetIsGrounded{}.ScheduleParallel(state.Dependency);
        handle.Complete();

        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        state.Dependency = new TestRaycastJob {
            physicsWorldSingleton = physicsWorldSingleton
        }.ScheduleParallel(state.Dependency);
    }

    partial struct ResetIsGrounded : IJobEntity {
        public void Execute(ref MovementData data) {
            data.isGrounded = false;
        }
    }

    partial struct TestRaycastJob : IJobEntity {
        [ReadOnly] public PhysicsWorldSingleton physicsWorldSingleton;
        
        public void Execute(in LocalTransform localTransform,
                            in PhysicsMass mass,
                            in PhysicsCollider collider,
                            in ColliderAspect colliderAspect,
                            ref MovementData movementData
                            ) 
        {

            bool isGrounded;
            float3 pos = localTransform.Position + new float3(0,-0.01f,0);
            NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);
            //Cast collider slightly down and get collision data
            unsafe {
                ColliderCastInput input = new ColliderCastInput {
                    Collider = collider.ColliderPtr,
                    Start = pos,
                    End = pos
                };
                isGrounded = physicsWorldSingleton.CastCollider(input, ref hits);
            }
            
            //If didn't collide, skip this entity
            if(!isGrounded)
                return;
            
            //Loop through all hits
            foreach(ColliderCastHit hit in hits) {
                //If the entity hit was the current entity this job is on, skip this hit
                if(hit.Entity == colliderAspect.Entity)
                    continue;
                //If the hit happened below this entity, then it is a ground.
                if(hit.Position.y > localTransform.Position.y)
                    continue;
                
                movementData.isGrounded = true;
                Debug.Log("isGrounded");
                break;
            }
            if(!movementData.isGrounded)
                Debug.Log("isNotgrounded");
            hits.Dispose();
        }
    }
}
