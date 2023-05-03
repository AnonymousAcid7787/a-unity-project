using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Aspects;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
[BurstCompile]
public partial struct IsGroundedSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        //First reset the isGrounded booleans.
        JobHandle handle = new ResetIsGroundedJob{}.ScheduleParallel(state.Dependency);
        handle.Complete();

        //Then update the isGrounded values
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        state.Dependency = new IsGroundedJob{
            physicsWorldSingleton = physicsWorldSingleton,
        }.Schedule(state.Dependency);

        //Why do this? because this way of ground checking uses collision events. If the player is in the middle of the air, you want to reset the isGrounded bool
        //When they are in the air after their isGrounded got reset, then it won't update to being true/false, leaving it as "false" which is accurate.
    }

    [BurstCompile]
    partial struct ResetIsGroundedJob : IJobEntity {
        [BurstCompile]
        public void Execute(ref MovementData movementData) {
            movementData.isGrounded = false;
        }
    }
    
    [BurstCompile]
    partial struct IsGroundedJob : IJobEntity {
        public PhysicsWorldSingleton physicsWorldSingleton;
        public void Execute(in PhysicsCollider collider,
                            in LocalTransform localTransform,
                            in ColliderAspect aspect,
                            ref MovementData movementData) {
            float3 pos = localTransform.Position - new float3(0, 0.2f, 0);
            NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);
            unsafe {
                //right now its just casting to its self
                physicsWorldSingleton.CastCollider(
                    new ColliderCastInput {
                        Collider = collider.ColliderPtr,
                        Start = pos,
                        End = pos,
                        Orientation = localTransform.Rotation,
                        QueryColliderScale = localTransform.Scale-0.2f,
                    },
                    ref hits);
            }

            PhysicsUtils.RemoveEntityFromList(ref hits, aspect.Entity);
            
            movementData.isGrounded = hits.Length != 0;
            hits.Dispose();
        }
    }

}