using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
[BurstCompile]
public partial struct IsGroundedSystem : ISystem {
    internal ComponentDataHandles componentDataHandles;

    [BurstCompile]
    internal struct ComponentDataHandles
    {
        public ComponentLookup<MovementData> movementData;

        public ComponentDataHandles(ref SystemState systemState)
        {
            movementData = systemState.GetComponentLookup<MovementData>(false);
        }

        public void Update(ref SystemState systemState)
        {
            movementData.Update(ref systemState);
        }
    }
    
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        componentDataHandles = new ComponentDataHandles(ref state);
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        return;
        componentDataHandles.Update(ref state);

        //First reset the isGrounded bool.
        JobHandle handle = new ResetIsGroundedJob{}.ScheduleParallel(state.Dependency);
        handle.Complete();

        //Then update the isGrounded values
        state.Dependency = new IsGroundedJob{
            // movementData = componentDataHandles.movementData
        }.Schedule(/*SystemAPI.GetSingleton<SimulationSingleton>(),*/ state.Dependency);

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

    #region commented
    // [BurstCompile]
    // partial struct IsGroundedJob : ICollisionEventsJob {
        
    //     public ComponentLookup<MovementData> movementData;

    //     [BurstCompile]
    //     public void Execute(CollisionEvent collisionEvent) {
    //         Entity entityA = collisionEvent.EntityA;
    //         if(!movementData.HasComponent(entityA))
    //             return;
    //         bool isGrounded = collisionEvent.Normal.y > 0;

    //         //Set isGrounded bool
    //         MovementData dataCopy = movementData[entityA];
    //             dataCopy.isGrounded = isGrounded;
    //         movementData[entityA] = dataCopy;
    //     }
    // }
    #endregion commented
    
    [BurstCompile]
    partial struct IsGroundedJob : IJobEntity {
        public void Execute(in PhysicsCollider col) {

        }
    }

}