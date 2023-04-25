using System.Collections;
using System.Collections.Generic;
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
                            in ColliderAspect colliderAspect)
        {
            float3 epsilon = new float3(0, Epsilon, 0) * -math.normalize(controller.Gravity);
            float3 currPos = localTransform.Position + epsilon;
            quaternion currRot = localTransform.Rotation;
            float3 gravityVel = controller.Gravity * deltaTime * (controller.IsGrounded ? 0:1);
            float3 verticalVelocity = new float3();
            float3 gravityVelocity = new float3();
            float3 jumpVelocity = new float3();

            
            float3 totalVelocity = 
        }
    }
}
