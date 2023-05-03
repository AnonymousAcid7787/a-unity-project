using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using UnityEngine;
using Unity.Physics.Aspects;
using Unity.Physics.Systems;

//This system updates key presses made by the player such as movement, jumping, skill activation etc.
//It also makes the player move accordingly
//In the future, a separate system might be made for each type of input.
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial class PlayerInputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((
            ref PhysicsVelocity physicsVelocity,
            ref PlayerInputData playerInputData,
            ref MovementData movementData,
            ref LocalTransform localTransform,
            ref RigidBodyAspect rigidBodyAspect,
            in PlayerInputKeys keys
        ) => {
            #region x & z movement
            //Get movement directions
            int xDirection = Convert.ToInt32(Input.GetKey(keys.rightKey)) - Convert.ToInt32(Input.GetKey(keys.leftKey));
            int zDirection = Convert.ToInt32(Input.GetKey(keys.upKey)) - Convert.ToInt32(Input.GetKey(keys.downKey));
            float3 targetVelocity = new float3(xDirection, 0, zDirection);
            targetVelocity *= movementData.movementSpeed;
            
            //Align direction
            targetVelocity = localTransform.TransformDirection(targetVelocity);

            //Calculate forces
            float3 velocityChange = targetVelocity - physicsVelocity.Linear;

            //Make sure that the player doesn't exceed its max movement force
            Vector3.ClampMagnitude(velocityChange, movementData.maxForce);
            
            velocityChange.y = 0;
            rigidBodyAspect.ApplyLinearImpulseLocalSpace(velocityChange);

            playerInputData.movementDirection.x = xDirection;
            playerInputData.movementDirection.y = zDirection;
            #endregion x & z movement
    
            #region jumping
            //get bool stating whether jump key is pressed
            bool jump = Input.GetKey(keys.jump);

            playerInputData.jump = jump;

            //Jump if button is pressed & on the ground 
            if(jump && movementData.isGrounded) 
                rigidBodyAspect.ApplyLinearImpulseLocalSpace(Vector3.up * movementData.jumpHeight);
            
            #endregion jumping
        }).WithoutBurst().Run();
    }
}