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
        Entities.ForEach((ref PhysicsVelocity velocity, ref PlayerMovementData data,  in PlayerInputKeys keys) => {
            int xDirection = Convert.ToInt32(Input.GetKey(keys.rightKey)) - Convert.ToInt32(Input.GetKey(keys.leftKey));
            int zDirection = Convert.ToInt32(Input.GetKey(keys.upKey)) - Convert.ToInt32(Input.GetKey(keys.downKey));

            velocity.Linear.x = xDirection * data.movementSpeed;
            velocity.Linear.z = zDirection * data.movementSpeed;
            
            data.jump = Input.GetKey(keys.jump);

            if(data.jump)
                velocity.Linear.y = 1;
        }).WithoutBurst().Run();
    }
}