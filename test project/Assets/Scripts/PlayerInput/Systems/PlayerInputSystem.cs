using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using UnityEngine;

public partial class PlayerInputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((
            ref PhysicsVelocity velocity,
            ref PlayerMovementData playerMovementData,
            ref MovementData movementData,
            in PlayerInputKeys keys,
            in PhysicsCollider collider,
            in LocalTransform localTransform
        ) => {
            #region x & z movement
            int xDirection = Convert.ToInt32(Input.GetKey(keys.rightKey)) - Convert.ToInt32(Input.GetKey(keys.leftKey));
            int zDirection = Convert.ToInt32(Input.GetKey(keys.upKey)) - Convert.ToInt32(Input.GetKey(keys.downKey));

            velocity.Linear.x = xDirection * playerMovementData.movementSpeed;
            velocity.Linear.z = zDirection * playerMovementData.movementSpeed;
            playerMovementData.movementDirection.x = xDirection;
            playerMovementData.movementDirection.z = zDirection;
            #endregion x & z movement
            
            #region jumping
            // //Cast the collider down by 0.1 to set isGrounded bool
            // unsafe {
            //     ColliderCastHit colliderCastHit;
            //     bool hasCollided = collider.Value.Value.CastCollider(new ColliderCastInput {
            //         Collider = collider.ColliderPtr,
            //         Start = localTransform.Position,
            //         End = localTransform.Position
            //     }, out colliderCastHit);
            //     movementData.isGrounded = 
            //         hasCollided && colliderCastHit.Position.y < localTransform.Position.y;
            // }
            // playerMovementData.jump = Input.GetKey(keys.jump);
            if(movementData.isGrounded && playerMovementData.jump) {
                velocity.Linear.y += 5;
            }
            #endregion jumping
        }).WithoutBurst().Run();
    }
}