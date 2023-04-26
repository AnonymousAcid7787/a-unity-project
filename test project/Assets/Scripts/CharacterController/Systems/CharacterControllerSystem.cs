using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Aspects;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct CharacterControllerSystem : ISystem
{
    private PhysicsWorldSingleton physicsWorldSingleton;
    private const float Epsilon = 0.001f;
    public void OnUpdate(ref SystemState state) {
        physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        float deltaTime = SystemAPI.Time.DeltaTime;

        state.Dependency = new CharacterControllerJob{
            physicsWorldSingleton = physicsWorldSingleton,
            deltaTime = deltaTime
        }.ScheduleParallel(state.Dependency);
    }

    partial struct CharacterControllerJob : IJobEntity {
        public PhysicsWorldSingleton physicsWorldSingleton;
        public float deltaTime;
        
        public void Execute(ref CharacterControllerComponent controller,
                            in LocalTransform localTransform,
                            in ColliderAspect colliderAspect,
                            in PhysicsCollider collider)
        {
            #region vertical movement
            //Offset current position by a tiny bit in order to prevent unwanted collisions from being registered
            //(the object will be hovering slightly over the floor because of this. The hovering is practiaclly not visible to the eye)
            float3 epsilon = new float3(0, Epsilon, 0) * -math.normalize(controller.Gravity);
            float3 currPos = localTransform.Position + epsilon;

            quaternion currRot = localTransform.Rotation;

            //Gravity velocity will be increased unless it hits the ground
            float3 gravityVelocity = controller.Gravity * deltaTime * (controller.IsGrounded ? 0:1);
            float3 verticalVelocity = new float3();
            float3 jumpVelocity = new float3();

            float3 totalVelocity = jumpVelocity + gravityVelocity;

            //Cast collider from the current positon of the object, to its future position when gravity is added.
            NativeList<ColliderCastHit> verticalHits = new NativeList<ColliderCastHit>(Allocator.Temp);
            float3 toPos = currPos + totalVelocity;
            PhysicsUtils.ColliderCastAll(
                ref verticalHits,
                in physicsWorldSingleton,
                in currPos,
                in toPos,
                in collider
            );
            PhysicsUtils.TrimByEntity(ref verticalHits, colliderAspect.Entity); 

            
            //If there were hits other than the entity its self
            if(verticalHits.Length != 0) {
                RigidTransform transform = new RigidTransform {
                    pos = currPos+totalVelocity,
                    rot = currRot
                };
                PhysicsUtils.ColliderDistance(
                    out DistanceHit verticalPenetration,
                    in collider,
                )
            }
            #endregion vertical movement


        }
    }
}
